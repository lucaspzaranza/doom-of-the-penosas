using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using static UnityEngine.InputSystem.InputAction;
using System;

public class PlayerSelectionUIController : ControllerUnit, IUIController
{
    public Action<GameMode> OnGameModeButtonPressed;

    // Vars
    [Space]
    [SerializeField] private PlayerSelectionMenuState _menuState;
    [SerializeField] private TMP_Text[] _penosasTexts;
    [SerializeField] private GameObject localArrow;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _backToMainMenuButton;
    [SerializeField] private GameObject _cancelCharacterSelectionButton; 
    [SerializeField] private GameObject _1stCharacterButton;
    [SerializeField] private GameObject _2ndCharacterButton;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private List<GameObject> _menuButtons;
    [SerializeField] private List<GameObject> _characterButtons;
    [SerializeField] private List<GameObject> _navButtons;

    private Button startBtnComponent;
    private Button cancelBtnComponent;

    // Props
    public TMP_Text[] PenosasTexts => _penosasTexts;
    public GameObject StartButton => _startButton;
    public GameObject BackToMainMenuButton => _backToMainMenuButton;
    public GameObject CancelCharacterSelectionButton => _cancelCharacterSelectionButton;
    public List<GameObject> MenuButtons => _menuButtons;
    public PlayerSelectionMenuState MenuState => _menuState;
    public List<GameObject> CharacterButtons => _characterButtons;
    public List<GameObject> NavButtons => _navButtons;
    public int CurrentPlayerIndex => 0;

    private void OnEnable()
    {
        _menuState = PlayerSelectionMenuState.PlayerSelection;
    }

    /// <summary>
    /// Se passar o índice 0, retorna o índice 1, e vice-versa.
    /// </summary>
    private int GetComplementaryPlayerIndex(int currentIndex)
    {
        return (currentIndex + 1) % 2;
    }
    
    public void SelectButton(GameObject buttonToSelect)
    {
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void Setup()
    {
        _parentController = GetComponentInParent<UIController>();

        startBtnComponent = StartButton.GetComponent<Button>();
        cancelBtnComponent = BackToMainMenuButton.GetComponent<Button>();
    }

    //public override void Dispose()
    //{

    //}

    public void SetGameMode(int gameMode)
    {
        OnGameModeButtonPressed?.Invoke((GameMode)gameMode);
    }
}