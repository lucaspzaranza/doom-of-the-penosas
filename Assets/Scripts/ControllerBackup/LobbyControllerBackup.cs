using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyControllerBackup : ControllerBackup
{
    [SerializeField] private GameObject _startButton;
    public GameObject StartButton => _startButton;

    [SerializeField] private GameObject _backToGameModeMenuFromCharacterSelectionButton;
    public GameObject BackToGameModeMenuFromCharacterSelectionButton => _backToGameModeMenuFromCharacterSelectionButton;

    [SerializeField] private GameObject _cancelCharacterSelectionButton;
    public GameObject CancelCharacterSelectionButton => _cancelCharacterSelectionButton;

    [SerializeField] private GameObject _deviceSelectorPanel;
    public GameObject DeviceSelectorPanel => _deviceSelectorPanel;

    [SerializeField] private List<Button> _characterButtons;
    public List<Button> CharacterButtons => _characterButtons;

    [SerializeField] private List<TMP_Text> _charactersTexts;
    public List<TMP_Text> CharacterTexts => _charactersTexts;

    [SerializeField] private Color _defaultTextColor;
    public Color DefaultTextColor => _defaultTextColor;

    [SerializeField] private List<Color> _selectedColors;
    public List<Color> SelectedColors => _selectedColors;

    [SerializeField] private TextMeshProUGUI _lobbyMessages;
    public TextMeshProUGUI LobbyMessages => _lobbyMessages;

    [SerializeField] private InputControlsPanel _inputControlsPanel;
    public InputControlsPanel InputControlsPanel => _inputControlsPanel;

    [Space]
    [Header("Scene Buttons")]
    [SerializeField] private Button _singlePlayerBtn;
    [SerializeField] private Button _multiplayerBtn;
    [SerializeField] private Button _newGameBtn;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _backToMainMenuBtnFromGameMode;
    [SerializeField] private Button _quitGameBtn;   

    [Space]
    [Header("Scene GameObjects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameModeMenu;
    [SerializeField] private GameObject _playerSelectionMenu;
    [SerializeField] private GameObject _2PCursor;

    protected override Type GetControllerType() => typeof(PlayerLobbyUIController);

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private void SetGameMode(GameMode mode)
    {
        var lobbyController = _controller as PlayerLobbyUIController;

        lobbyController.FireSetGameModeEvent(mode);
        lobbyController.SetLobbyState(LobbyState.PlayerSelection);
        _gameModeMenu.SetActive(false);
        _playerSelectionMenu.SetActive(true);
        lobbyController.SelectButton(_characterButtons[0].gameObject);
    }

    protected override void ListenersSetup()
    {
        var lobbyController = _controller as PlayerLobbyUIController;

        if (lobbyController == null)
        {
            WarningMessages.ControllerNotFoundOnBackupMessage(nameof(PlayerLobbyUIController));
            return;
        }

        _singlePlayerBtn.onClick.AddListener(() =>
        {
            SetGameMode(GameMode.Singleplayer);
        });

        _multiplayerBtn.onClick.AddListener(() =>
        {
            SetGameMode(GameMode.Multiplayer);
        });

        _newGameBtn.onClick.AddListener(() =>
        {
            lobbyController.FireSetNewGameEvent(true);
            _mainMenu.SetActive(false);
            _gameModeMenu.SetActive(true);
        });

        _continueBtn.onClick.AddListener(() =>
        {
            lobbyController.FireSetNewGameEvent(false);
            _mainMenu.SetActive(false);
            _gameModeMenu.SetActive(true);
        });

        _backToMainMenuBtnFromGameMode.onClick.AddListener(() =>
        {
            _gameModeMenu.SetActive(false);
            _mainMenu.SetActive(true);
        });

        _quitGameBtn.onClick.AddListener(() =>
        {
            lobbyController.QuitGame();
        });

        // The for loop didn't worked here, it has to be hardcoded ¬¬"
        _characterButtons[0].onClick.AddListener(() =>
        {
            lobbyController.ChooseCharacter(0);
        });

        _characterButtons[1].onClick.AddListener(() =>
        {
            lobbyController.ChooseCharacter(1);
        });

        _startButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            lobbyController.SelectPlayersCharacters();
        });

        _backToGameModeMenuFromCharacterSelectionButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            //_mainMenu.SetActive(true);
            _gameModeMenu.SetActive(true);
            _playerSelectionMenu.SetActive(false);
            lobbyController.FireSetGameModeEvent(GameMode.Singleplayer);
            lobbyController.SetLobbyState(LobbyState.GameModeSelection);
            lobbyController.FireSetNewGameEvent(false);

            if(_2PCursor.activeSelf)
                _2PCursor.SetActive(false);

            if(_lobbyMessages.gameObject.activeSelf)
                _lobbyMessages.gameObject.SetActive(false);
        });

        _cancelCharacterSelectionButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            lobbyController.CancelCharacterSelection();
        });
    }
}
