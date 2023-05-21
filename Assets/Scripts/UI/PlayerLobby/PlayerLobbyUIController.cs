using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class PlayerLobbyUIController : ControllerUnit, IUIController
{
    public Action<GameMode> OnGameModeButtonPressed;
    public Action<GameObject> OnGameReadyToStart;
    public Action<IReadOnlyList<Penosas>> OnLobbySelectedCharacters;
    public Action OnCancelSelection;

    [Header("Chosen Characters")]
    [SerializeField] private List<Penosas> _characterSelectionList;
    public List<Penosas> CharacterSelectionList => _characterSelectionList;

    // Vars
    [Space]
    [SerializeField] private LobbyState _lobbyState;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _backToMainMenuButton;
    [SerializeField] private GameObject _cancelCharacterSelectionButton;
    [SerializeField] private List<Button> _characterButtons;
    [SerializeField] private List<TMP_Text> _charactersTexts;
    [SerializeField] private Color _defaultTextColor;
    [SerializeField] private List<Color> _selectedColors;

    private List<CursorPosition> _cursors;
    
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
        print("Disposing Lobby UI Controller...");
        _characterSelectionList = null;
        ResetLobbyState();
        
        gameObject.SetActive(false);
    }

    public override void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup)
    {
        LobbyControllerBackup lobbyBackup = backup as LobbyControllerBackup;

        _startButton = lobbyBackup.StartButton;
        _backToMainMenuButton = lobbyBackup.BackToMainMenuButton;
        _cancelCharacterSelectionButton = lobbyBackup.CancelCharacterSelectionButton;
        _characterButtons = lobbyBackup.CharacterButtons;
        _charactersTexts = lobbyBackup.CharacterTexts;
        _defaultTextColor = lobbyBackup.DefaultTextColor;
        _selectedColors = lobbyBackup.SelectedColors;
    }

    public void SetGameMode(int gameMode)
    {
        OnGameModeButtonPressed?.Invoke((GameMode)gameMode);
    }

    public void SetGameMode(GameMode gameMode)
    {
        OnGameModeButtonPressed?.Invoke(gameMode);
    }

    public void SetLobbyState(int lobbyState)
    {
        _lobbyState = (LobbyState)lobbyState;
    }

    public void SetLobbyState(LobbyState lobbyState)
    {
        _lobbyState = lobbyState;
    }

    public void ChooseCharacter(int characterIndex)
    {
        print("characterIndex: " + characterIndex);
        if (_lobbyState != LobbyState.PlayerSelection)
            return;

        if (_characterSelectionList == null)
            ResetSelection();

        _characterSelectionList.Add((Penosas)characterIndex);
        SetCharacterTextColors(characterIndex, _selectedColors[0]);

        // Considering a 2-player multiplayer logic
        if (GetGameMode() == GameMode.Multiplayer)
        {
            int otherPlayer = GetComplementaryPlayerIndex(characterIndex);
            _characterSelectionList.Add((Penosas)otherPlayer);
            SetCharacterTextColors(otherPlayer, _selectedColors[1]);
        }

        _lobbyState = LobbyState.ReadyToStart;
        SetGameActivation(true);
    }

    private void SetCharacterTextColors(int characterIndex, Color color)
    {
        //print(_charactersTexts.Count);
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
        // Only valid for 2 cursors at screen
        if(GetGameMode() == GameMode.Multiplayer && _lobbyState == LobbyState.PlayerSelection)
        {
            int index = _cursors.IndexOf(cursor);
            int complementaryCursorIndex = GetComplementaryPlayerIndex(index);
            CursorPosition _2ndCursor = _cursors[complementaryCursorIndex];

            Button parentBtn = cursor.transform.parent.GetComponent<Button>();
            int btnIndex = _characterButtons.IndexOf(parentBtn);
            int complementaryBtnIndex = GetComplementaryPlayerIndex(btnIndex);
            GameObject complementaryButton = _characterButtons[complementaryBtnIndex].gameObject;

            if (coordinates.y == 0)
                _2ndCursor.UpdateCursorPosition(complementaryButton);
            else if (cursor.transform.parent.Equals(_2ndCursor.transform.parent))
            {
                SelectButton(complementaryButton);
                cursor.UpdateCursorPosition(complementaryButton);
            }
        }
    }
   
    public void SelectPlayersCharacters()
    {
        OnLobbySelectedCharacters?.Invoke(CharacterSelectionList);
    }
}