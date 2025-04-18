using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.InputSystem;

public class PlayerLobbyUIController : ControllerUnit, IUIController
{
    public Action<GameMode> OnGameModeButtonPressed;
    public Action<GameObject> OnGameReadyToStart;
    public Action<IReadOnlyList<Penosas>> OnLobbySelectedCharacters;
    public Action<IReadOnlyList<InputDevice>> OnLobbySelectedDevices;
    public Action<Language> OnLobbySelectedLanguage;
    public Action<bool> OnLobbySetNewGame;
    public Action OnCancelSelection;

    [Header("Chosen Characters")]
    [SerializeField] private List<Penosas> _characterSelectionList;
    public List<Penosas> CharacterSelectionList => _characterSelectionList;
    
    // Vars
    [Space]
    [SerializeField] private LobbyState _lobbyState;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _backToMainMenuButton;
    [SerializeField] private GameObject _devicePopupCloseButton;
    [SerializeField] private GameObject _cancelCharacterSelectionButton;
    [SerializeField] private GameObject _deviceSelectorPanel;
    [SerializeField] private List<Button> _characterButtons;
    [SerializeField] private List<TMP_Text> _charactersTexts;
    [SerializeField] private Color _defaultTextColor;
    [SerializeField] private List<Color> _selectedColors;
    [SerializeField] private List<InputDevicesSelector> _deviceSelectors;
    [SerializeField] private TextMeshProUGUI _lobbyMessages;
    [SerializeField] private InputControlsPanel _inputControlsPanel;

    private List<CursorPosition> _cursors;
    private DevicePopupPanelUI _devicePanel;

    public void SelectButton(GameObject buttonToSelect)
    {
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    private void OnEnable()
    {
        MenuWithCursor.OnMenuEnabled += HandleOnLobbySelectionMenuEnabled;
        CursorPosition.OnCursorMoved += HandleOnCursorMoved;
    }

    private void OnDisable()
    {
        MenuWithCursor.OnMenuEnabled -= HandleOnLobbySelectionMenuEnabled;
        CursorPosition.OnCursorMoved -= HandleOnCursorMoved;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void Setup()
    {
        SetControllerFromParent<UIController>();
    }

    public void ResetLobbyState()
    {
        _lobbyState = LobbyState.GameModeSelection;
    }

    public override void Dispose()
    {
        _characterSelectionList = null;
        ResetLobbyState();
        
        _devicePanel.OnDeviceSelectorsAdded -= HandleOnDeviceSelectorsAdded;
        gameObject.SetActive(false);
    }

    public override void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup)
    {
        LobbyControllerBackup lobbyBackup = backup as LobbyControllerBackup;

        _startButton = lobbyBackup.StartButton;
        _backToMainMenuButton = lobbyBackup.BackToGameModeMenuFromCharacterSelectionButton;
        _cancelCharacterSelectionButton = lobbyBackup.CancelCharacterSelectionButton;
        _characterButtons = lobbyBackup.CharacterButtons;
        _charactersTexts = lobbyBackup.CharacterTexts;
        _defaultTextColor = lobbyBackup.DefaultTextColor;
        _selectedColors = lobbyBackup.SelectedColors;
        _deviceSelectorPanel = lobbyBackup.DeviceSelectorPanel;
        _lobbyMessages = lobbyBackup.LobbyMessages;
        _inputControlsPanel = lobbyBackup.InputControlsPanel;

        _devicePanel = _deviceSelectorPanel.GetComponent<DevicePopupPanelUI>();
        _devicePanel.OnDeviceSelectorsAdded += HandleOnDeviceSelectorsAdded;
        _devicePanel.OnInputControlsPanelActivation += HandleOnInputControlsPanelActivated;
        _devicePopupCloseButton = _inputControlsPanel.CloseButton;
    }

    private void HandleOnDeviceSelectorsAdded(List<InputDevicesSelector> devices) 
    {
        _deviceSelectors = devices.Where(device => device.gameObject.activeSelf).ToList();
    }

    private void HandleOnInputControlsPanelActivated(bool active)
    {
        var UIController = _parentController as UIController;
        if(active)
        {
            UIController.CursorPositions[0].UpdateCursorPosition(_devicePopupCloseButton);
            UIController.CursorPositions[0].LockCursor();
            EventSystem.current.SetSelectedGameObject(_devicePopupCloseButton);
        }
        else
        {
            UIController.CursorPositions[0].UnlockCursor();
            UIController.CursorPositions[0].UpdateCursorPosition(UIController.CursorPositions[0].LastPressed);
            EventSystem.current.SetSelectedGameObject(UIController.CursorPositions[0].LastPressed);
        }
    }

    public void FireSetNewGameEvent(bool newGame)
    {
        OnLobbySetNewGame?.Invoke(newGame);
    }

    public void FireSetGameModeEvent(GameMode gameMode)
    {
        OnGameModeButtonPressed?.Invoke(gameMode);
    }

    public void SetLobbyState(LobbyState lobbyState)
    {
        _lobbyState = lobbyState;
    }

    public void ChooseCharacter(int characterIndex)
    {
        if (_lobbyState != LobbyState.PlayerSelection)
            return;

        if(HasRepeatedDeviceSelected(out string duplicatedDevice))
        {
            LanguageSO lang = GetSelectedLanguage();

            _lobbyMessages.gameObject.SetActive(true);
            _lobbyMessages.text = lang.GetDuplicatedDeviceMessage(duplicatedDevice);
            return;
        }
        else
        {
            OnLobbySelectedDevices?.Invoke(_deviceSelectors.Select(device => device.SelectedDevice).ToList());
            _lobbyMessages.gameObject.SetActive(false);
        }

        if (_characterSelectionList == null)
            ResetSelection();

        _characterSelectionList.Add((Penosas)characterIndex);
        SetCharacterTextColors(characterIndex, _selectedColors[0]);

        // Considering a 2-player multiplayer logic
        if (GetGameMode() == GameMode.Multiplayer)
        {
            //int otherPlayer = GetComplementaryPlayerIndex(characterIndex);
            int otherPlayer = SharedFunctions.GetComplementaryIndex(characterIndex);
            _characterSelectionList.Add((Penosas)otherPlayer);
            SetCharacterTextColors(otherPlayer, _selectedColors[1]);
        }

        _lobbyState = LobbyState.ReadyToStart;
        _deviceSelectorPanel.SetActive(false);
        SetGameActivation(true);
    }

    private bool HasRepeatedDeviceSelected(out string duplicatedDevice)
    {
        duplicatedDevice = string.Empty;
        var selectedDevices = _deviceSelectors.Select(device => device.SelectedDevice).ToList();
        List<InputDevice> repeateDevices = new List<InputDevice>();

        for (int i = 0; i < _deviceSelectors.Count; i++)
        {
            for (int j = 0; j < _deviceSelectors.Count; j++)
            {
                if (i == j)
                    continue;
                
                if (_deviceSelectors[j].SelectedDevice == selectedDevices[i] && 
                    !repeateDevices.Contains(_deviceSelectors[j].SelectedDevice))
                {
                    duplicatedDevice = _deviceSelectors[j].SelectedDevice.displayName;
                    repeateDevices.Add(_deviceSelectors[j].SelectedDevice);
                    break;
                }
            }
        }

        return repeateDevices.Count > 0;
    }

    private void SetCharacterTextColors(int characterIndex, Color color)
    {
        _charactersTexts[characterIndex].color = color;
    }

    public void SetGameActivation(bool val)
    {
        _cancelCharacterSelectionButton.SetActive(val);
        _backToMainMenuButton.SetActive(!val);

        var startBtn = _startButton.GetComponent<Button>();
        startBtn.interactable = val;

        _characterButtons.ForEach(btn =>
        {
            var btnComponent = btn.interactable = !val;
        });

        if (val)
        {
            // _startButton is the button the cursor must go when the game is about to start
            OnGameReadyToStart?.Invoke(_startButton);
        }
        else
            OnCancelSelection?.Invoke();
    }
    
    public void ResetSelection()
    {
        _characterSelectionList = new List<Penosas>();
    }

    public void CancelCharacterSelection()
    {
        if (_lobbyState != LobbyState.ReadyToStart)
            return;

        ResetSelection();

        _lobbyState = LobbyState.PlayerSelection;
        _charactersTexts.ForEach(_text =>
        {
            SetCharacterTextColors(_charactersTexts.IndexOf(_text), _defaultTextColor);
        });
        _deviceSelectorPanel.SetActive(true);
        SetGameActivation(false);
    }

    private void HandleOnLobbySelectionMenuEnabled(IReadOnlyList<CursorPosition> cursors)
    {
        if (_lobbyState == LobbyState.PlayerSelection && GetGameMode() == GameMode.Multiplayer)
        {
            _cursors = new List<CursorPosition>(cursors);

            for (int i = 1; i < cursors.Count; i++)
            {
                cursors[i].gameObject.SetActive(true);
            }
        }
    }

    private void HandleOnCursorMoved(CursorPosition cursor, Vector2 coordinates)
    {
        if(!_lobbyMessages.gameObject.activeSelf)
            _lobbyMessages.gameObject.SetActive(true);

        if(_lobbyState == LobbyState.PlayerSelection)
        {
            // Only valid for 2 cursors at screen
            if(GetGameMode() == GameMode.Multiplayer)
            {
                int index = _cursors.IndexOf(cursor);
                int complementaryCursorIndex = SharedFunctions.GetComplementaryIndex(index);
                CursorPosition _2ndCursor = _cursors[complementaryCursorIndex];

                Button parentBtn = cursor.transform.parent.GetComponent<Button>();
                int btnIndex = _characterButtons.IndexOf(parentBtn);
                int complementaryBtnIndex = SharedFunctions.GetComplementaryIndex(btnIndex);
                GameObject complementaryButton = _characterButtons[complementaryBtnIndex].gameObject;

                if (coordinates.y == 0 && cursor.transform.parent.gameObject.IsCharacterSelectionButton()) // Horizontal Cursor Movement
                    _2ndCursor.UpdateCursorPosition(complementaryButton);
                else if (cursor.transform.parent.Equals(_2ndCursor.transform.parent)) // Changing the 1P and the 2P cursors position
                {
                    SelectButton(complementaryButton);
                    cursor.UpdateCursorPosition(complementaryButton);
                }
            }
        }

        UpdateLobbyMessages(cursor);
    }

    /// <summary>
    /// Update Lobby Messages based upon cursor current position.
    /// </summary>
    /// <param name="cursor"></param>
    private void UpdateLobbyMessages(CursorPosition cursor)
    {
        LanguageSO lang = GetSelectedLanguage();

        if (cursor.transform.parent.gameObject.IsCharacterSelectionButton())
            _lobbyMessages.text = lang.SelectYourCharacterToPlay;
        else if (cursor.transform.parent.gameObject.IsDeviceSelectionButton())
            _lobbyMessages.text = lang.SelectDeviceYouWishToUse;
    }

    public void SelectPlayersCharacters()
    {
        OnLobbySelectedCharacters?.Invoke(CharacterSelectionList);
    }

    public void SetGameLanguage(Language language)
    {
        OnLobbySelectedLanguage?.Invoke(language);
    }
}