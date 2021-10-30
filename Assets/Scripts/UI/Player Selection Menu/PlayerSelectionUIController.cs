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
    [SerializeField] private List<PlayerSelectionData> _playerDataList;

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
    public List<PlayerSelectionData> PlayerDataList => _playerDataList;

    /// <summary>
    /// Escolhe a seta do jogador da sua conexão.
    /// </summary>
    public NetworkArrowPosition CurrentPlayerArrow => FindObjectsOfType<NetworkArrowPosition>()
            .SingleOrDefault(arrow => arrow.netIdentity.hasAuthority);

    /// <summary>
    /// Escolhe a seta do outro jogador que está jogando com você. Se for single player, retornará nulo.
    /// </summary>
    public NetworkArrowPosition OtherPlayerArrow => FindObjectsOfType<NetworkArrowPosition>()
            .SingleOrDefault(arrow => !arrow.netIdentity.hasAuthority);

    public int CurrentPlayerIndex => NetworkClient.isHostClient ? 0 : 1;

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
        cancelBtnComponent = CancelButton.GetComponent<Button>();

        NetworkArrowPosition.OnChangeArrowPositionButtonPressed += HandleOnChangeArrowPositionButtonPressed;
    }

    void OnDestroy()
    {
        NetworkArrowPosition.OnChangeArrowPositionButtonPressed -= HandleOnChangeArrowPositionButtonPressed;
    }

    private void HandleOnChangeArrowPositionButtonPressed(UpdateArrowPositionEventArgs eventArgs)
    {
        //print($"Quem chamou: {eventArgs.NetworkArrowPosition.gameObject.name}. " +
        //    $"É pra ir pro botão: {eventArgs.PreviousButton.name}");

        NetworkArrowPosition playerArrow = OtherPlayerArrow;

        if (playerArrow != null)
        {
            EventSystem.current.SetSelectedGameObject(eventArgs.PreviousButton);
            playerArrow.CmdUpdateArrowPosition(eventArgs.PreviousButton);
        }
    }

    /// <summary>
    /// Se passar o índice 0, retorna o índice 1, e vice-versa.
    /// </summary>
    private int GetComplementaryPlayerIndex(int currentIndex)
    {
        return (currentIndex + 1) % 2;
    }

    public void SelectCharacter(int characterID)
    {
        int index = CurrentPlayerIndex;
        CmdUpdateSelectPlayerDataListWrapper(index, characterID);
    }

    public void SelectButton(GameObject buttonToSelect)
    {
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
        LocalArrow.UpdateArrowPosition(buttonToSelect);
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
    public void RpcSetPlayerTextColor(int index, Color newColor)
    {
        PenosasTexts[index].color = newColor;
    }

    #endregion

    #region Server

    [ClientRpc]
    public void RpcUpdatePlayerDataList(int index, PlayerSelectionData data)
    {
        PlayerDataList[index] = data;
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateCharactersNameColors(int index, int characterIndex)
    {
        Color baseColor = index == 0 ? Color.red : Color.yellow;
        RpcSetPlayerTextColor(characterIndex, baseColor);
        int complementaryPlayerIndex = GetComplementaryPlayerIndex(characterIndex);

        if(NetworkManager.singleton.numPlayers > 1)
        {
            Color complementaryColor = index == 0 ? Color.yellow : Color.red;
            RpcSetPlayerTextColor(complementaryPlayerIndex, complementaryColor);
        }
        else
            RpcSetPlayerTextColor(complementaryPlayerIndex, Color.white);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateSelectPlayerDataListWrapper(int index, int characterIndex)
    {
        RpcUpdateSelectPlayerDataList(index, characterIndex);
        CmdUpdateCharactersNameColors(index, characterIndex);
    }

    [ClientRpc]
    public void RpcUpdateSelectPlayerDataList(int index, int chosenCharacterIndex)
    {
        // Índice complementar para escolher o outro jogador.
        int complementaryPlayerIndex = GetComplementaryPlayerIndex(index);
        // Índice pra escolher a galinha que sobrou.
        int complementaryPenosaIndex = GetComplementaryPlayerIndex(chosenCharacterIndex);

        Penosas newCharacter = (Penosas)chosenCharacterIndex;
        Penosas remainingPenosa = (Penosas)complementaryPenosaIndex;

        PlayerDataList[index].SelectedPenosa = newCharacter;
        PlayerDataList[complementaryPlayerIndex].SelectedPenosa = remainingPenosa;
    }

    #endregion
}
