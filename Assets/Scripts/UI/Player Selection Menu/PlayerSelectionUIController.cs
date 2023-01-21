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

public class PlayerSelectionUIController : MonoBehaviour
{
    #region Vars

    public static PlayerSelectionUIController instance;
    public const string ArrowPositionName = "ArrowPosition";

    [SerializeField] private PlayerSelectionMenuState _menuState;
    [SerializeField] private TMP_Text[] _penosasTexts;
    [SerializeField] private GameObject localArrow;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _backToMainMenuButton;
    [SerializeField] private GameObject _cancelCharacterSelectionButton;
    [SerializeField] private GameObject _1stCharacterButton;
    [SerializeField] private GameObject _2ndCharacterButton;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject connectionTypeMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject localLobbyMenu;
    [SerializeField] private GameObject _clientDisconnectedMenu;
    [SerializeField] private GameObject _clientDisconnectConfirmationMenu;
    [SerializeField] private Text networkAdress;
    [SerializeField] private LocalArrowPosition _localArrow;
    [SerializeField] private List<GameObject> _menuButtons;
    [SerializeField] private List<GameObject> _characterButtons;
    [SerializeField] private List<GameObject> _navButtons;

    private Button startBtnComponent;
    private Button cancelBtnComponent;

    #endregion

    #region Props

    public TMP_Text[] PenosasTexts => _penosasTexts;
    public GameObject StartButton => _startButton;
    public GameObject BackToMainMenuButton => _backToMainMenuButton;
    public GameObject CancelCharacterSelectionButton => _cancelCharacterSelectionButton;
    public GameObject DisconnectedMenuPanel => _clientDisconnectedMenu;
    public GameObject DisconnectConfirmationPanel => _clientDisconnectConfirmationMenu;
    public List<GameObject> MenuButtons => _menuButtons;
    public LocalArrowPosition LocalArrow => _localArrow;
    public PlayerSelectionMenuState MenuState => _menuState;
    public List<GameObject> CharacterButtons => _characterButtons;
    public List<GameObject> NavButtons => _navButtons;
    //public int CurrentPlayerIndex => NetworkClient.isHostClient ? 0 : 1;
    public int CurrentPlayerIndex => 0;

    #endregion

    #region Local

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        _menuState = PlayerSelectionMenuState.PlayerSelection;
    }

    private void Start()
    {
        startBtnComponent = StartButton.GetComponent<Button>();
        cancelBtnComponent = BackToMainMenuButton.GetComponent<Button>();
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
        LocalArrow.UpdateArrowPosition(buttonToSelect);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetDisconnectedMenuActivation(bool val)
    {
        cancelBtnComponent.interactable = !val;
        DisconnectedMenuPanel.SetActive(val);
    }

    public void SetDisconnectConfirmationActivation(bool val)
    {
        if (!val && startBtnComponent.interactable)
            startBtnComponent.interactable = !val;

        cancelBtnComponent.interactable = !val;
        DisconnectConfirmationPanel.SetActive(val);
    }

    #endregion
}