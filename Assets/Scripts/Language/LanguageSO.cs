using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language Texts", menuName = "ScriptableObjects/Language Texts")]
public class LanguageSO : ScriptableObject
{
    [Header("Main Menu")]
    [SerializeField] private string _newGameBtn;
    public string NewGameButton => _newGameBtn;

    [SerializeField] private string _continueBtn;
    public string ContinueButton => _continueBtn;

    [SerializeField] private string _languageBtn;
    public string LanguageButton => _languageBtn;

    [SerializeField] private string _quitBtn;
    public string QuitButton => _quitBtn;

    [Header("Language Menu")]
    [SerializeField] private string _whichLanguage;
    public string WhichLanguageTxt => _whichLanguage;

    [SerializeField] private string _english;
    public string English => _english;

    [SerializeField] private string _portuguese;
    public string Portuguese => _portuguese;

    [SerializeField] private string _backFromLangMenu;
    public string BackFromLangMenu => _backFromLangMenu;

    [Header("Game Mode Menu")]
    [SerializeField] private string _newGameSubtitle;
    public string NewGameSubtitle => _newGameSubtitle;

    [SerializeField] private string _continueSubtitle;
    public string ContinueSubtitle => _continueSubtitle;

    [SerializeField] private string _singlePlayer;
    public string SinglePlayer => _singlePlayer;

    [SerializeField] private string _multiplayer;
    public string Multiplayer => _multiplayer;

    [Header("Player Selection Menu")]
    [SerializeField] private string _selectYourPenosa;
    public string SelectYourPenosa => _selectYourPenosa;

    [SerializeField] private string _selectInputDevice;
    public string SelectInputDevice => _selectInputDevice;

    [SerializeField] private string _start;
    public string Start => _start;

    [SerializeField] private string _backToGameModeSelection;
    public string BackToGameModeSelection => _backToGameModeSelection;

    [SerializeField] private string _selectYourCharacterToPlay;
    public string SelectYourCharacterToPlay => _selectYourCharacterToPlay;

    [SerializeField] [TextArea] private string _selectDeviceYouWishToUse;
    public string SelectDeviceYouWishToUse => _selectDeviceYouWishToUse;

    [SerializeField] private string _cancelCharacterSelectionBtn;
    public string CancelCharacterSelectionBtn => _cancelCharacterSelectionBtn;

    [SerializeField] [TextArea] private string _duplicatedDeviceMessage;
    public string GetDuplicatedDeviceMessage(string deviceName) => _duplicatedDeviceMessage.Replace(ConstantStrings.DeviceNameKey, deviceName);

    [Header("Device Popup Menu - General Controls")]

    [SerializeField] private string _inputControlsGuideTitle;
    public string InputControlsGuideTitle => _inputControlsGuideTitle;

    [SerializeField] private string _closeButton;
    public string CloseButton => _closeButton;

    [Header("Device Popup Menu - Keyboard")]

    [SerializeField] private string _wasd;
    public string WASD => _wasd;

    [SerializeField] private string _enterConfirm;
    public string EnterConfirm => _enterConfirm;

    [SerializeField] private string _enterPause;
    public string EnterPause => _enterPause;

    [SerializeField] private string _alt;
    public string Alt => _alt;

    [SerializeField] private string _qr;
    public string QR => _qr;

    [SerializeField] private string _ctrl;
    public string CTRL => _ctrl;

    [SerializeField] private string _arrows;
    public string Arrows => _arrows;

    [SerializeField] private string _space;
    public string Space => _space;

    [SerializeField] private string _shift;
    public string Shift => _shift;

    [Header("Device Popup Menu - Joystick")]

    [SerializeField] private string _leftStick;
    public string LeftStick => _leftStick;

    [SerializeField] private string _buttonSouthJump;
    public string ButtonSouthJump => _buttonSouthJump;

    [SerializeField] private string _buttonSouthConfirm;
    public string ButtonSouthConfirm => _buttonSouthConfirm;

    [SerializeField] private string _buttonNorth;
    public string ButtonNorth => _buttonNorth;

    [SerializeField] private string _rightAndLeftButtons;
    public string RightAndLeftButtons => _rightAndLeftButtons;

    [SerializeField] private string _buttonWest;
    public string ButtonWest => _buttonWest;

    [SerializeField] private string _dPad;
    public string DPad => _dPad;

    [SerializeField] private string _buttonEast;
    public string ButtonEast => _buttonEast;

    [SerializeField] private string _startButton;
    public string StartButton => _startButton;
}
