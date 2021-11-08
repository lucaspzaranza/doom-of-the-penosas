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
using System;

public class PlayerSelectionUIController : NetworkBehaviour
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
    [SerializeField] private List<NetworkArrow> _networkArrows;
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
    public List<NetworkArrow> NetworkArrows => _networkArrows;  

    /// <summary>
    /// Escolhe a seta do jogador da sua conex�o.
    /// </summary>
    public NetworkArrow CurrentPlayerArrow => FindObjectsOfType<NetworkArrow>()
            .SingleOrDefault(arrow => arrow.netIdentity.hasAuthority);

    /// <summary>
    /// Escolhe a seta do outro jogador que est� jogando com voc�. Se for single player, retornar� nulo.
    /// </summary>
    public NetworkArrow OtherPlayerArrow => FindObjectsOfType<NetworkArrow>()
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
        cancelBtnComponent = BackToMainMenuButton.GetComponent<Button>();

        NetworkArrow.OnChangeArrowPositionButtonPressed += HandleOnChangeArrowPositionButtonPressed;
        NetworkArrow.OnArrowPositionChanged += HandleOnArrowPositionChanged;
        NetworkArrow.OnSelectButtonPressed += HandleOnSelectButtonPressed;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSetCancelCharacterSelectionButtonActivation(false);
    }

    void OnDestroy()
    {
        NetworkArrow.OnChangeArrowPositionButtonPressed -= HandleOnChangeArrowPositionButtonPressed;
        NetworkArrow.OnSelectButtonPressed -= HandleOnSelectButtonPressed;
    }

    private void HandleOnArrowPositionChanged(NetworkArrow networkArrow)
    {
        if (MenuState == PlayerSelectionMenuState.ReadyToStart && isClientOnly)
            CmdFlip2PArrowPosition(networkArrow);
    }

    private void HandleOnChangeArrowPositionButtonPressed(NetworkArrow networkArrow)
    {
        if (MenuState == PlayerSelectionMenuState.PlayerSelection)
            CmdAlternateSelectCharactersArrowPositions();
    }

    private void HandleOnSelectButtonPressed(NetworkArrow networkArrow)
    {
        bool isPlayer2 = !Equals(NetworkArrows[0], networkArrow);
        
        if (MenuState == PlayerSelectionMenuState.ReadyToStart)
        {
            // For 2P only
            if(isPlayer2)
                CmdFlip2PArrowPosition(networkArrow);
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
        NetworkManager.singleton.networkAddress = !string.IsNullOrEmpty(networkAdress.text) ? networkAdress.text : "localhost";
        NetworkManager.singleton.StartClient();
    }

    public void DisconnectAndBackToMainMenu()
    {
        if (isClientOnly)
        {
            NetworkManager.singleton.StopClient();
            connectionTypeMenu.SetActive(true);
            lobbyMenu.SetActive(false);
        }
        else if (isServer)
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
        if (!val && startBtnComponent.interactable)
            startBtnComponent.interactable = !val;

        cancelBtnComponent.interactable = !val;
        DisconnectConfirmationPanel.SetActive(val);
    }

    public bool TryGetSelectedPenosaFromButton(NetworkArrow networkArrow, out Penosas selectedPenosa)
    {
        selectedPenosa = Penosas.None;
        Transform parent = networkArrow.gameObject.transform.parent;
        if (parent != null)
        {
            TextMeshProUGUI buttonText = parent.GetComponentInChildren<TextMeshProUGUI>();
            return Enum.TryParse(buttonText.text, out selectedPenosa);
        }
        return false;
    }

    #endregion

    #region Client

    [TargetRpc]
    public void TargetGetServerPlayerCharacterSelectionData(NetworkConnection target, List<GameObject> playerCharacterOrderList)
    {
        _characterButtons = new List<GameObject>(playerCharacterOrderList);
    }

    [TargetRpc]
    public void TargetGetServerPlayerConnectionsData(NetworkConnection target, List<PlayerConnection> newData)
    {
        var networkManager = FindObjectOfType<PenosasNetworkManager>();
        networkManager.SetPlayersConnection(newData);
    }

    [ClientRpc]
    private void RpcAlternateSelectCharactersArrowPositions()
    {
        int index = CurrentPlayerIndex;
        int complementary = GetComplementaryPlayerIndex(index);

        var charBtnsCopy = new List<GameObject>(CharacterButtons);
        CharacterButtons[index] = charBtnsCopy[complementary];
        CharacterButtons[complementary] = charBtnsCopy[index];

        if (NetworkArrows.Count > 1)
            NetworkArrows[complementary].CmdUpdateArrowPosition(CharacterButtons[complementary].name);
        EventSystem.current.SetSelectedGameObject(CharacterButtons[CurrentPlayerIndex]);
    }

    [ClientRpc]
    public void RpcSetState(PlayerSelectionMenuState newState)
    {
        _menuState = newState;
    }

    public void SetState(int newState)
    {
        RpcSetState((PlayerSelectionMenuState)newState);
    }

    public void SetState(PlayerSelectionMenuState newState)
    {
        RpcSetState(newState);
    }

    [ClientRpc]
    public void RpcSetPlayerTextColor(int index, Color newColor)
    {
        PenosasTexts[index].color = newColor;
    }

    [ClientRpc]
    public void RpcUpdateSelectPlayerDataList(int index, int chosenCharacterIndex)
    {
        // Índice complementar para escolher o outro jogador.
        int complementaryPlayerIndex = GetComplementaryPlayerIndex(index);
        // Índice pra escolher a galinha que sobrou.
        int complementaryPenosaIndex = GetComplementaryPlayerIndex(chosenCharacterIndex);

        Penosas newCharacter = (Penosas)chosenCharacterIndex;
        Penosas remainingCharacter = (Penosas)complementaryPenosaIndex;

        // Trocar pra conexão do jogador
        var playerConnections = FindObjectsOfType<PlayerConnection>().OrderBy(x => x.netId).ToList();

        playerConnections[index].PlayerSelectionData.SelectedPenosa = newCharacter;

        if (playerConnections.Count > 1)
            playerConnections[complementaryPlayerIndex].PlayerSelectionData.SelectedPenosa = remainingCharacter;
    }

    [ClientRpc]
    private void RpcCancelCharacterSelection()
    {
        var arrows = FindObjectsOfType<NetworkArrow>();
        foreach (var arrow in arrows)
        {
            arrow.CmdUpdateArrowPosition(arrow.PreviousSelectedButton.name);
        }
    }

    [ClientRpc]
    private void RpcSetBackToMainMenuButtonActivation(bool value)
    {
        BackToMainMenuButton.SetActive(value);
    }

    [ClientRpc]
    private void RpcSetBackToMainMenuButtonInteractable(bool value)
    {
        BackToMainMenuButton.GetComponent<Button>().interactable = value;
    }

    [ClientRpc]
    private void RpcSetCancelCharacterSelectionButtonInteractable(bool value)
    {
        CancelCharacterSelectionButton.GetComponent<Button>().interactable = value;
    }

    [ClientRpc]
    private void RpcSetCancelCharacterSelectionButtonActivation(bool value)
    {
        CancelCharacterSelectionButton.SetActive(value);
    }

    [ClientRpc]
    public void RpcSetStartButtonGameObjectInteractable(bool value)
    {
        StartButton.GetComponent<Button>().interactable = value;
    }

    [ClientRpc]
    public void RpcSetChooseCharacterButtonInteractable(bool value)
    {
        _1stCharacterButton.GetComponent<Button>().interactable = value;
        _2ndCharacterButton.GetComponent<Button>().interactable = value;
    }

    [ClientRpc]
    public void RpcSetSelectedGameObject(GameObject objToSet)
    {
        EventSystem.current.SetSelectedGameObject(objToSet);
    }

    #endregion

    #region Server

    [Command(requiresAuthority = false)]
    public void CmdFlip2PArrowPosition(NetworkArrow networkArrow)
    {
        // Flip Position
        float inverseX = NetworkArrows[0].transform.localPosition.x * -1f;
        Vector2 newPos = new Vector2(inverseX, transform.localPosition.y);
        networkArrow.CmdUpdateArrowPositionByPosition(newPos);

        // Flip Scale if it is in normal (positive) scale
        if(networkArrow.transform.localScale.x > 0)
        {
            RectTransform rectTransform = networkArrow.transform.GetComponent<RectTransform>();
            Vector3 localScale = rectTransform.localScale;
            rectTransform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
            networkArrow.ArrowText.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdAlternateSelectCharactersArrowPositions()
    {
        RpcAlternateSelectCharactersArrowPositions();
    }

    [Command(requiresAuthority = false)]
    public void CmdCancelCharacterSelection()
    {
        RpcCancelCharacterSelection();
    }

    [Command(requiresAuthority = false)]
    public void CmdSetBackToMainMenuButtonActivation(bool value)
    {
        RpcSetBackToMainMenuButtonActivation(value);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateCharactersNameColors(int index, int characterIndex)
    {
        Color baseColor = index == 0 ? Color.red : Color.yellow;
        RpcSetPlayerTextColor(characterIndex, baseColor);
        int complementaryPlayerIndex = GetComplementaryPlayerIndex(characterIndex);

        if (NetworkManager.singleton.numPlayers > 1)
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

    [Command(requiresAuthority = false)]
    public void CmdSetBackToMainMenuButtonInteractable(bool value)
    {
        RpcSetBackToMainMenuButtonInteractable(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetCancelCharacterSelectionButtonInteractable(bool value)
    {
        RpcSetCancelCharacterSelectionButtonInteractable(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetCancelCharacterSelectionButtonActivation(bool value)
    {
        RpcSetCancelCharacterSelectionButtonActivation(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetStartButtonGameObjectInteractable(bool value)
    {
        RpcSetStartButtonGameObjectInteractable(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetChooseCharacterButtonInteractable(bool value)
    {
        _1stCharacterButton.GetComponent<Button>().interactable = value;
        _2ndCharacterButton.GetComponent<Button>().interactable = value;
        RpcSetChooseCharacterButtonInteractable(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdNetworkSelectCharacterButton(GameObject buttonToNavigate)
    {
        NetworkArrow playerArrow = CurrentPlayerArrow;

        if (playerArrow != null)
        {
            RpcSetSelectedGameObject(buttonToNavigate);
            playerArrow.CmdUpdateArrowPosition(buttonToNavigate.name);

            if (NetworkManager.singleton.numPlayers > 1)
            {
                var otherArrow = OtherPlayerArrow;
                otherArrow?.CmdUpdateArrowPosition(buttonToNavigate.name);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSelectCharacter(int characterID)
    {
        SetState(PlayerSelectionMenuState.ReadyToStart);
        CmdSetStartButtonGameObjectInteractable(true);
        SelectCharacter(characterID);
        CmdNetworkSelectCharacterButton(StartButton);
        CmdSetChooseCharacterButtonInteractable(false);
        CmdSetBackToMainMenuButtonActivation(false);
        CmdSetCancelCharacterSelectionButtonActivation(true);
        CmdSetCancelCharacterSelectionButtonInteractable(true);
    }

    #endregion
}