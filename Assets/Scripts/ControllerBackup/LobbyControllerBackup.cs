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

    [SerializeField] private GameObject _backToMainMenuButton;
    public GameObject BackToMainMenuButton => _backToMainMenuButton;

    [SerializeField] private GameObject _cancelCharacterSelectionButton;
    public GameObject CancelCharacterSelectionButton => _cancelCharacterSelectionButton;

    [SerializeField] private List<Button> _characterButtons;
    public List<Button> CharacterButtons => _characterButtons;

    [SerializeField] private List<TMP_Text> _charactersTexts;
    public List<TMP_Text> CharacterTexts => _charactersTexts;

    [SerializeField] private Color _defaultTextColor;
    public Color DefaultTextColor => _defaultTextColor;

    [SerializeField] private List<Color> _selectedColors;
    public List<Color> SelectedColors => _selectedColors;

    [Space]
    [Header("Scene Buttons")]
    [SerializeField] private Button _singlePlayerBtn;
    [SerializeField] private Button _multiplayerBtn;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _quitGameBtn;

    [Space]
    [Header("Scene GameObjects")]
    [SerializeField] private GameObject _mainMenu;
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

        lobbyController.FireSetNewGameEvent(true);
        lobbyController.FireSetGameModeEvent(mode);
        lobbyController.SetLobbyState(LobbyState.PlayerSelection);
        _mainMenu.SetActive(false);
        _playerSelectionMenu.SetActive(true);
        lobbyController.SelectButton(_characterButtons[0].gameObject);
    }

    protected override void ListenersSetup()
    {
        var lobbyController = _controller as PlayerLobbyUIController;

        if (lobbyController == null)
        {
            ConstantStrings.ControllerNotFoundOnBackupMessage(nameof(PlayerLobbyUIController));
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

        _continueBtn.onClick.AddListener(() =>
        {
            // Load game progress logic here...
            lobbyController.FireSetNewGameEvent(false);
        });

        _quitGameBtn.onClick.AddListener(() =>
        {
            lobbyController.QuitGame();
        });

        // For some reason, a for loop didn't worked here ¬¬"
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

        _backToMainMenuButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            _mainMenu.SetActive(true);
            _playerSelectionMenu.SetActive(false);
            lobbyController.FireSetGameModeEvent(GameMode.Singleplayer);
            lobbyController.SetLobbyState(LobbyState.GameModeSelection);
            lobbyController.FireSetNewGameEvent(false);
            if(_2PCursor.activeSelf)
                _2PCursor.SetActive(false);
        });

        _cancelCharacterSelectionButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            lobbyController.CancelCharacterSelection();
        });
    }
}
