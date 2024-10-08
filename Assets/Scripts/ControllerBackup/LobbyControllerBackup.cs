using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor;
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
    [SerializeField] private Button _languageMenuBtn;   
    [SerializeField] private TextMeshProUGUI _whichLangTxt;   
    [SerializeField] private Button _englishBtn;   
    [SerializeField] private Button _portugueseBtn;   
    [SerializeField] private Button _languageBackBtn; 
    
    [SerializeField] private TextMeshProUGUI _selectYourPenosa;
    [SerializeField] private TextMeshProUGUI _selectInputDevice;
    [SerializeField] private TextMeshProUGUI _newGameSubtitle;

    [Header("Device Popup Menu - General Controls")]
    [SerializeField] private TextMeshProUGUI _inputControlsGuideTitle;
    [SerializeField] private TextMeshProUGUI _closeButton;

    [Header("Device Popup Menu - Keyboard")]
    [SerializeField] private TextMeshProUGUI _wasd;
    [SerializeField] private TextMeshProUGUI _enterConfirm;
    [SerializeField] private TextMeshProUGUI _enterPause;
    [SerializeField] private TextMeshProUGUI _alt;
    [SerializeField] private TextMeshProUGUI _qr;
    [SerializeField] private TextMeshProUGUI _ctrl;
    [SerializeField] private TextMeshProUGUI _arrows;
    [SerializeField] private TextMeshProUGUI _space;
    [SerializeField] private TextMeshProUGUI _shift;

    [Header("Device Popup Menu - Joystick")]
    [SerializeField] private TextMeshProUGUI _leftStick;
    [SerializeField] private TextMeshProUGUI _buttonSouthJump;
    [SerializeField] private TextMeshProUGUI _buttonSouthConfirm;
    [SerializeField] private TextMeshProUGUI _buttonNorth;
    [SerializeField] private TextMeshProUGUI _rightAndLeftButtons;
    [SerializeField] private TextMeshProUGUI _buttonWest;
    [SerializeField] private TextMeshProUGUI _dPad;
    [SerializeField] private TextMeshProUGUI _buttonEast;
    [SerializeField] private TextMeshProUGUI _startButtonTxt;

    [Space]
    [Header("Scene GameObjects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameModeMenu;
    [SerializeField] private GameObject _languageMenu;
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

        _languageMenuBtn.onClick.AddListener(() =>
        {
            _mainMenu.SetActive(false);
            _languageMenu.SetActive(true);
        });

        _englishBtn.onClick.AddListener(() =>
        {
            lobbyController.SetGameLanguage(Language.English);
            UpdateLanguageTexts();
        });

        _portugueseBtn.onClick.AddListener(() =>
        {
            lobbyController.SetGameLanguage(Language.Portuguese);
            UpdateLanguageTexts();
        });

        _languageBackBtn.onClick.AddListener(() =>
        {
            _mainMenu.SetActive(true);
            _languageMenu.SetActive(false);
        });
    }

    public override void UpdateLanguageTexts()
    {
        LanguageSO selectedLang = _controller.GetSelectedLanguage();

        // Main Menu
        _newGameBtn.GetComponentInChildren<Text>().text = selectedLang.NewGameButton;
        _continueBtn.GetComponentInChildren<Text>().text = selectedLang.ContinueButton;
        _languageMenuBtn.GetComponentInChildren<Text>().text = selectedLang.LanguageButton;
        _quitGameBtn.GetComponentInChildren<Text>().text = selectedLang.QuitButton;

        // Language Menu
        _whichLangTxt.GetComponent<TextMeshProUGUI>().text = selectedLang.WhichLanguageTxt;
        _englishBtn.GetComponentInChildren<Text>().text = selectedLang.English;
        _portugueseBtn.GetComponentInChildren<Text>().text = selectedLang.Portuguese;
        _languageBackBtn.GetComponentInChildren<Text>().text = selectedLang.BackFromLangMenu;

        // Game Mode
        _singlePlayerBtn.GetComponentInChildren<Text>().text = selectedLang.SinglePlayer;
        _multiplayerBtn.GetComponentInChildren<Text>().text = selectedLang.Multiplayer;
        _backToMainMenuBtnFromGameMode.GetComponentInChildren<Text>().text = selectedLang.BackFromLangMenu;
        _newGameSubtitle.text = selectedLang.NewGameSubtitle;

        // Player Selection Menu
        _selectYourPenosa.text = selectedLang.SelectYourPenosa;
        _selectInputDevice.text = selectedLang.SelectInputDevice;
        _startButton.GetComponentInChildren<Text>().text = selectedLang.Start;
        _backToGameModeMenuFromCharacterSelectionButton.GetComponentInChildren<Text>().text = selectedLang.BackToGameModeSelection;
        LobbyMessages.text = selectedLang.SelectYourCharacterToPlay;
        CancelCharacterSelectionButton.GetComponentInChildren<Text>().text = selectedLang.CancelCharacterSelectionBtn;

        // Device Popup Menu - General Controls
        _inputControlsGuideTitle.text = selectedLang.InputControlsGuideTitle;
        _closeButton.text = selectedLang.CloseButton;

        // Device Popup Menu - Keyboard
        _wasd.text = selectedLang.WASD;
        _enterConfirm.text = selectedLang.EnterConfirm;
        _enterPause.text = selectedLang.EnterPause;
        _alt.text = selectedLang.Alt;
        _qr.text = selectedLang.QR;
        _ctrl.text = selectedLang.CTRL;
        _arrows.text = selectedLang.Arrows;
        _space.text = selectedLang.Space;
        _shift.text = selectedLang.Shift;

        // Device Popup Menu - Joystick
        _leftStick.text = selectedLang.LeftStick;
        _buttonSouthJump.text = selectedLang.ButtonSouthJump;
        _buttonSouthConfirm.text = selectedLang.ButtonSouthConfirm;
        _buttonNorth.text = selectedLang.ButtonNorth;
        _rightAndLeftButtons.text = selectedLang.RightAndLeftButtons;
        _buttonWest.text = selectedLang.ButtonWest;
        _dPad.text = selectedLang.DPad;
        _buttonEast.text = selectedLang.ButtonEast;
        _startButtonTxt.text = selectedLang.StartButton;
    }
}
