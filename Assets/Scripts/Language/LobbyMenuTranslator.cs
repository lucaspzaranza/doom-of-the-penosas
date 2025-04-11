using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuTranslator : MenuTranslator
{
    [SerializeField] private GameObject _startButton;
    public GameObject StartButton => _startButton;

    [SerializeField] private GameObject _backToGameModeMenuFromCharacterSelectionButton;
    public GameObject BackToGameModeMenuFromCharacterSelectionButton => _backToGameModeMenuFromCharacterSelectionButton;

    [SerializeField] private GameObject _cancelCharacterSelectionButton;
    public GameObject CancelCharacterSelectionButton => _cancelCharacterSelectionButton;

    [SerializeField] private TextMeshProUGUI _lobbyMessages;
    public TextMeshProUGUI LobbyMessages => _lobbyMessages;

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

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Translate()
    {
        // Main Menu
        _newGameBtn.GetComponentInChildren<Text>().text = lang.NewGameButton;
        _continueBtn.GetComponentInChildren<Text>().text = lang.ContinueButton;
        _languageMenuBtn.GetComponentInChildren<Text>().text = lang.LanguageButton;
        _quitGameBtn.GetComponentInChildren<Text>().text = lang.QuitButton;

        // Language Menu
        _whichLangTxt.GetComponent<TextMeshProUGUI>().text = lang.WhichLanguageTxt;
        _englishBtn.GetComponentInChildren<Text>().text = lang.English;
        _portugueseBtn.GetComponentInChildren<Text>().text = lang.Portuguese;
        _languageBackBtn.GetComponentInChildren<Text>().text = lang.BackFromLangMenu;

        // Game Mode
        _singlePlayerBtn.GetComponentInChildren<Text>().text = lang.SinglePlayer;
        _multiplayerBtn.GetComponentInChildren<Text>().text = lang.Multiplayer;
        _backToMainMenuBtnFromGameMode.GetComponentInChildren<Text>().text = lang.BackFromLangMenu;
        _newGameSubtitle.text = lang.NewGameSubtitle;

        // Player Selection Menu
        _selectYourPenosa.text = lang.SelectYourPenosa;
        _selectInputDevice.text = lang.SelectInputDevice;
        _startButton.GetComponentInChildren<Text>().text = lang.Start;
        _backToGameModeMenuFromCharacterSelectionButton.GetComponentInChildren<Text>().text = lang.BackToGameModeSelection;
        LobbyMessages.text = lang.SelectYourCharacterToPlay;
        CancelCharacterSelectionButton.GetComponentInChildren<Text>().text = lang.CancelCharacterSelectionBtn;

        // Device Popup Menu - General Controls
        _inputControlsGuideTitle.text = lang.InputControlsGuideTitle;
        _closeButton.text = lang.CloseButton;

        // Device Popup Menu - Keyboard
        _wasd.text = lang.WASD;
        _enterConfirm.text = lang.EnterConfirm;
        _enterPause.text = lang.EnterPause;
        _alt.text = lang.Alt;
        _qr.text = lang.QR;
        _ctrl.text = lang.CTRL;
        _arrows.text = lang.Arrows;
        _space.text = lang.Space;
        _shift.text = lang.Shift;

        // Device Popup Menu - Joystick
        _leftStick.text = lang.LeftStick;
        _buttonSouthJump.text = lang.ButtonSouthJump;
        _buttonSouthConfirm.text = lang.ButtonSouthConfirm;
        _buttonNorth.text = lang.ButtonNorth;
        _rightAndLeftButtons.text = lang.RightAndLeftButtons;
        _buttonWest.text = lang.ButtonWest;
        _dPad.text = lang.DPad;
        _buttonEast.text = lang.ButtonEast;
        _startButtonTxt.text = lang.StartButton;
    }
}
