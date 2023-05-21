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

    [Space]
    [Header("Scene GameObjects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _playerSelection;

    protected override Type GetControllerType() => typeof(PlayerLobbyUIController);

    public override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void SetListeners()
    {
        var playerController = _controller as PlayerLobbyUIController;

        _singlePlayerBtn.onClick.AddListener(() =>
        {
            playerController.SetGameMode(GameMode.Singleplayer);
            playerController.SetLobbyState(LobbyState.PlayerSelection);
            _mainMenu.SetActive(false);
            _playerSelection.SetActive(true);
            playerController.SelectButton(_characterButtons[0].gameObject);
        });
    }
}
