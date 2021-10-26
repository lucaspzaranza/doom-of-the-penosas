using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
using SharedData.Enumerations;
using Mirror;
using UnityEngine.EventSystems;

public class PlayerSelectionController : NetworkBehaviour
{
    #region Vars

    [SerializeField] private PlayerSelectionData[] playersSelectionData;
    [SerializeField] private float arrowXOffset;
    [SerializeField] private Transform arrowParent;
    [SerializeField] private GameObject _2PArrow;
    [SerializeField] private List<RectTransform> _arrowPositions;
    [SerializeField] private PlayerConnection[] _playerConnections = new PlayerConnection[2];

    [SyncVar] private int _playerCount = 1;
    private SyncList<int> _playersIndexes = new SyncList<int>();

    private PlayerInput playerInputActions;
    private InputAction navigation;
    private InputAction selectPlayer;
    private InputAction cancelOrBack;

    #endregion

    #region Props

    // Alias pra encurtar o comprimento das linhas.
    public PlayerSelectionMenuState MenuState => PlayerSelectionUIController.instance.MenuState;
    public int PlayerCount
    {
        get => _playerCount;
        set => _playerCount = value;
    }

    public PlayerSelectionData[] PlayersSelectionData => playersSelectionData;

    public List<RectTransform> ArrowPositions => _arrowPositions;

    /// <summary>
    /// O índice em que cada ovo-seta do jogador está.
    /// </summary>
    /// <returns>0 pra galinha da esquerda, 1 pra da direita.</returns>
    public SyncList<int> SelectedPlayersIndexes => _playersIndexes;

    /// <summary>
    /// Se for o servidor, será o Player 1 (índice 0); senão, será o cliente como Player 2 (índice 1).
    /// </summary>
    public int NetworkPlayerIndex => isServer ? 0 : 1;

    public PlayerConnection[] PlayerConnections
    {
        get => _playerConnections;
        set => _playerConnections = value;
    }

    #endregion

    #region Local

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.performed += ChangePlayerArrowPosition;
        navigation.Enable();

        selectPlayer = playerInputActions.PlayerSelectionMenu.SelectPlayer;
        selectPlayer.performed += Select;
        selectPlayer.Enable();

        cancelOrBack = playerInputActions.PlayerSelectionMenu.CancelorBack;
        cancelOrBack.performed += Cancel;
        cancelOrBack.Enable();
    }

    private void OnDisable()
    {
        navigation.performed -= ChangePlayerArrowPosition;
        navigation.Disable();

        selectPlayer.performed -= Select;
        selectPlayer.Disable();

        cancelOrBack.performed -= Cancel;
        cancelOrBack.Disable();
    }

    /// <summary>
    /// Se passar o índice 0, retorna o índice 1, e vice-versa.
    /// </summary>
    private int GetComplementaryPlayerIndex(int currentIndex)
    {
        return (currentIndex + 1) % 2;
    }

    private void ChangePlayerArrowPosition(CallbackContext context)
    {
        //int index = NetworkPlayerIndex;
        //if (MenuState == PlayerSelectionMenuState.ReadyToStart) return;

        //float direction = navigation.ReadValue<float>();
        //if (direction < 0) // Left
        //{
        //    SetArrowPosition(index, ArrowPositions[0].localPosition);

        //    SetPlayerIndex(index, 0);
        //    SetPlayerIndex(GetComplementaryPlayerIndex(index), 1);

        //    if (PlayerCount == 2)
        //        SetArrowPosition(GetComplementaryPlayerIndex(index), ArrowPositions[1].localPosition);
        //}
        //else if (direction > 0) // Right
        //{
        //    SetArrowPosition(index, ArrowPositions[1].localPosition);
        //    SetPlayerIndex(index, 1);
        //    SetPlayerIndex(GetComplementaryPlayerIndex(index), 0);

        //    if (PlayerCount == 2)
        //        SetArrowPosition(GetComplementaryPlayerIndex(index), ArrowPositions[0].localPosition);
        //}
    }

    private void Cancel(CallbackContext context)
    {
        int index = SelectedPlayersIndexes[NetworkPlayerIndex];
        if (MenuState == PlayerSelectionMenuState.ReadyToStart)
        {
            SetPlayerNameTextColor(SelectedPlayersIndexes[index], Color.white);
            SetArrowPosition(index, PlayersSelectionData[index].PreviousCoordinate);

            if (PlayerCount == 2)
            {
                int complementary = GetComplementaryPlayerIndex(index);
                //if(isServer) LogSelectedPlayersIndexes();
                SetPlayerNameTextColor(SelectedPlayersIndexes[complementary], Color.white);
                SetArrowPosition(complementary, PlayersSelectionData[complementary].PreviousCoordinate);
            }

            PlayerSelectionUIController.instance.SetStartButtonGameObjectInteractable(false);
            PlayerSelectionUIController.instance.SetCancelButtonGameObjectInteractable(true);
            SetMenuState(PlayerSelectionMenuState.PlayerSelection);
        }
        else if (MenuState == PlayerSelectionMenuState.PlayerSelection)
            print("Voltar ao menu principal.");
    }

    private void Select(CallbackContext context)
    {
        CmdSelect();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void LogSelectedPlayersIndexes()
    {
        print("SelectedPlayersIndexes SyncList. Count: " + SelectedPlayersIndexes.Count);
        print("SelectedPlayersIndexes[0]: " + SelectedPlayersIndexes[0]);
        print("SelectedPlayersIndexes[1]: " + SelectedPlayersIndexes[1]);
    }

    #endregion

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        SelectedPlayersIndexes.Add(0);
        SelectedPlayersIndexes.Add(0);

        //PlayerSelectionUIController.instance.StartButton.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    private void SetPlayerIndex(int playerIndexKey, int playerIndexValue)
    {
        SelectedPlayersIndexes[playerIndexKey] = playerIndexValue;
    }

    [Command(requiresAuthority = false)]
    private void SetArrowPosition(int arrowIndex, Vector2 newPos)
    {
        PlayersSelectionData[arrowIndex].PreviousCoordinate = PlayersSelectionData[arrowIndex].Arrow.transform.localPosition;
        PlayersSelectionData[arrowIndex].Arrow.transform.localPosition = newPos;
    }

    [Command(requiresAuthority = false)]
    private void SetMenuState(PlayerSelectionMenuState newState)
    {
        PlayerSelectionUIController.instance.SetState(newState);
    }

    [Command(requiresAuthority = false)]
    private void SetPlayerNameTextColor(int index, Color newColor)
    {
        PlayerSelectionUIController.instance.SetPlayerTextColor(index, newColor);
    }

    [Command(requiresAuthority = false)]
    public void CmdSelect()
    {
        print("CmdSelect");
        int index = NetworkPlayerIndex;
        if (MenuState == PlayerSelectionMenuState.PlayerSelection)
        {
            PlayersSelectionData[index].SelectedPenosa = (Penosas)SelectedPlayersIndexes[index];
            SetPlayerNameTextColor(SelectedPlayersIndexes[index], Color.red);
            //Vector2 startTxtPos = new Vector2(
            //    PlayerSelectionUIController.instance.StartButton.transform.Find(PlayerSelectionUIController.ArrowPositionName).transform.localPosition.x,
            //    PlayerSelectionUIController.instance.StartButton.transform.localPosition.y);

            if (PlayerCount == 2)
            {
                int complementary = GetComplementaryPlayerIndex(index);
                PlayersSelectionData[complementary].SelectedPenosa = (Penosas)SelectedPlayersIndexes[complementary];
                SetPlayerNameTextColor(SelectedPlayersIndexes[complementary], Color.yellow);
                //SetArrowPosition(1, new Vector2(startTxtPos.x, startTxtPos.y));
            }

            PlayerSelectionUIController.instance.SetStartButtonGameObjectInteractable(true);
            PlayerSelectionUIController.instance.SetCancelButtonGameObjectInteractable(false);
            //SetArrowPosition(0, startTxtPos);
            //SetMenuState(PlayerSelectionMenuState.ReadyToStart);
        }
        else if (MenuState == PlayerSelectionMenuState.ReadyToStart)
        {
            var networkManager = FindObjectOfType<PenosasNetworkManager>();
            PlayerConnection playerConnection = PlayerConnections[NetworkPlayerIndex];
            playerConnection.playerPrefab = PlayersSelectionData[SelectedPlayersIndexes[NetworkPlayerIndex]].Prefab;
            playerConnection.InstantiatePlayerPrefab();
            //var player1 = Instantiate(PlayersSelectionData[SelectedPlayersIndexes[0]].Prefab, transform.position, Quaternion.identity);
            //var player1 = networkManager.InstantiatePlayer((Penosas)SelectedPlayersIndexes[0], connectionToClient);
            //if (PlayerCount == 2)
            //{
            //    //var player2 = Instantiate(PlayersSelectionData[SelectedPlayersIndexes[1]].Prefab, transform.position, Quaternion.identity);
            //    //DontDestroyOnLoad(player2);
            //    playerConnection = PlayerConnections[NetworkPlayerIndex];
            //    playerConnection.playerPrefab = PlayersSelectionData[SelectedPlayersIndexes[NetworkPlayerIndex]].Prefab;
            //    playerConnection.InstantiatePlayerPrefab();
            //}

            networkManager?.StartGame();
        }
    }

    #endregion
}
