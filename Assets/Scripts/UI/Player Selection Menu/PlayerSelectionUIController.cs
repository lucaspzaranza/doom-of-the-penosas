using Mirror;
using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using static UnityEngine.InputSystem.InputAction;

public class PlayerSelectionUIController : NetworkBehaviour
{
    #region Vars

    public static PlayerSelectionUIController instance;
    public const string ArrowPositionName = "ArrowPosition";

    [SerializeField] private PlayerSelectionMenuState _menuState;
    [SerializeField] private TMP_Text[] _penosasTexts;
    [SerializeField] private GameObject localArrow;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _cancelButton;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject connectionTypeMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject localLobbyMenu;
    [SerializeField] private GameObject _clientDisconnectedMenu;
    [SerializeField] private GameObject _clientDisconnectConfirmationMenu;
    [SerializeField] private Text networkAdress;
    [SerializeField] private LocalArrowPosition _localArrow;
    [SerializeField] private List<GameObject> _menuButtons;

    private Button startBtnComponent;
    private Button cancelBtnComponent;

    #endregion

    #region Props

    public TMP_Text[] PenosasTexts => _penosasTexts;
    public GameObject StartButton => _startButton;
    public GameObject CancelButton => _cancelButton;
    public GameObject DisconnectedMenuPanel => _clientDisconnectedMenu;
    public GameObject DisconnectConfirmationPanel => _clientDisconnectConfirmationMenu;
    public List<GameObject> MenuButtons => _menuButtons;
    public LocalArrowPosition LocalArrow => _localArrow;
    public PlayerSelectionMenuState MenuState => _menuState;

    #endregion

    #region Local

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        startBtnComponent = StartButton.GetComponent<Button>();
        cancelBtnComponent = CancelButton.GetComponent<Button>();

        NetworkArrowPosition.OnChangeArrowPositionButtonPressed += () => print("Aloha!");
    }

    private void OnEnable()
    {
        _menuState = PlayerSelectionMenuState.PlayerSelection;
    }

    private void HandleOnChangeArrowPositionButtonPressed(CallbackContext context)
    {

    }

    public void SelectButton(GameObject buttonToSelect)
    {
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
        LocalArrow.UpdateArrowPosition(buttonToSelect);
    }

    public void NetworkSelectButton(GameObject buttonToSelect)
    {
        print("Arrow Connection: " + connectionToClient.connectionId);
        print("Player Connection: " + NetworkClient.localPlayer.connectionToServer.connectionId);
        var networkArrow = FindObjectsOfType<NetworkArrowPosition>()
            .SingleOrDefault(arrow => arrow.connectionToClient.connectionId == NetworkClient.localPlayer.connectionToServer.connectionId);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
        if (networkArrow != null)
            networkArrow.CmdUpdateArrowPosition(buttonToSelect);
    }

    public void HostLobby()
    {
        NetworkManager.singleton.StartHost();
    }

    public void ClientLobby()
    {
        NetworkManager.singleton.networkAddress = !string.IsNullOrEmpty(networkAdress.text)? networkAdress.text : "localhost";
        NetworkManager.singleton.StartClient();
    }

    public void StopConnection()
    {
        if (isClientOnly)
        {
            NetworkManager.singleton.StopClient();
            connectionTypeMenu.SetActive(true);
            lobbyMenu.SetActive(false);
            print("StopClient...");
        }
        else if(isServer)
        {
            if (NetworkManager.singleton.numPlayers > 1)
                SetDisconnectConfirmationActivation(true);
            else
            {
                NetworkManager.singleton.StopHost();
                lobbyMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
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
        if(!val && startBtnComponent.interactable)
            startBtnComponent.interactable = !val;

        cancelBtnComponent.interactable = !val;
        DisconnectConfirmationPanel.SetActive(val);
    }

    #endregion

    #region Client

    [ClientRpc]
    public void SetState(PlayerSelectionMenuState newState)
    {
        _menuState = newState;
    }

    [ClientRpc]
    public void SetStartButtonGameObjectInteractable(bool val)
    {
        StartButton.GetComponent<Button>().interactable = val;
    }

    [ClientRpc]
    public void SetCancelButtonGameObjectInteractable(bool val)
    {
        CancelButton.GetComponent<Button>().interactable = val;
    }

    [ClientRpc]
    public void SetPlayerTextColor(int index, Color newColor)
    {
        PenosasTexts[index].color = newColor;
    }

    #endregion

    #region Server

    #endregion

}
