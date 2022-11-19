using Mirror;
using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class NetworkArrow : NetworkBehaviour
{
    #region Vars

    [SerializeField] [SyncVar] private PlayerConnection _localPlayerConnection;
    [SerializeField] private GameObject _selectedButton;
    [SerializeField] private GameObject _previousSelectedButton;
    [SerializeField] private GameObject _arrowText;

    [SerializeField] private int _index;

    private RectTransform lobbyMenuTransform;
    private PlayerInput playerInputActions;
    private InputAction navigation;
    private InputAction selection;
    private InputAction cancelation;

    [SyncVar] [SerializeField] private bool _canChangePosition;

    public static event Action<NetworkArrow> OnChangeArrowPositionButtonPressed;
    public static event Action<NetworkArrow> OnSelectButtonPressed;
    public static event Action<NetworkArrow> OnCancelButtonPressed;
    public static event Action<NetworkArrow> OnArrowPositionChanged;

    #endregion

    #region Props
    public bool CanChangePosition 
    {
        get => _canChangePosition;
        set => _canChangePosition = value;
    }
    public int Index => NetworkClient.isHostClient ? 0 : 1;
    public GameObject SelectedButton => _selectedButton;
    public GameObject PreviousSelectedButton => _previousSelectedButton;
    public PlayerConnection LocalPlayerConnection => _localPlayerConnection;
    public GameObject ArrowText => _arrowText;

    #endregion

    #region Local

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.started += UpdateArrowPositionWrapper;
        navigation.Enable();

        selection = playerInputActions.PlayerSelectionMenu.SelectPlayer;
        selection.canceled += SelectButtonPressed;
        selection.Enable();

        cancelation = playerInputActions.PlayerSelectionMenu.CancelorBack;
        cancelation.started += Cancel;
        cancelation.Enable();
    }

    private void OnDisable()
    {
        navigation.started -= UpdateArrowPositionWrapper;
        navigation.Disable();

        selection.canceled -= SelectButtonPressed;
        selection.Disable();

        cancelation.started -= Cancel;
        cancelation.Disable();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerSelectionUIController.instance.NetworkArrows.Add(this);
        _canChangePosition = true;
    }

    private void Update()
    {
        if(_selectedButton == null && transform.parent?.gameObject != null)
            _selectedButton = transform.parent.gameObject;
    }

    private void UpdateArrowPositionWrapper(CallbackContext callbackContext)
    {
        if(CanChangePosition && LocalPlayerConnection.isLocalPlayer)
            StartCoroutine(nameof(UpdateArrowPositionCoroutine));
    }

    private IEnumerator UpdateArrowPositionCoroutine()
    {
        _previousSelectedButton = EventSystem.current.currentSelectedGameObject;
        yield return new WaitForEndOfFrame();
        _selectedButton = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(_selectedButton);
        CmdUpdateArrowPosition(_selectedButton.name);
        OnChangeArrowPositionButtonPressed?.Invoke(this);
        OnArrowPositionChanged?.Invoke(this);
        CmdSetPrevAndCurrentSelectedButtons(_previousSelectedButton.name, _selectedButton.name);
    }

    private void SelectButtonPressed(CallbackContext callbackContext)
    {
        OnSelectButtonPressed?.Invoke(this);
        CmdSetPrevAndCurrentSelectedButtons(_selectedButton.name, EventSystem.current.currentSelectedGameObject.name);
    }

    private void Cancel(CallbackContext callbackContext)
    {
        //print(LocalPlayerConnection + " is Local Player? " + LocalPlayerConnection.isLocalPlayer);
        if (LocalPlayerConnection.isLocalPlayer)
            OnCancelButtonPressed?.Invoke(this);
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcSetPrevAndCurrentSelectedButtons(string prevButtonName, string currentButtonName)
    {
        _previousSelectedButton = GameObject.Find(prevButtonName);
        _selectedButton = GameObject.Find(currentButtonName);
    }

    [ClientRpc]
    public void RpcUpdateArrowPosition(int buttonIndex)
    {
        GameObject buttonToNavigate = PlayerSelectionUIController.instance.MenuButtons[buttonIndex];

        var btnTransform = buttonToNavigate.transform;
        var arrowPosition = btnTransform.Find(PlayerSelectionUIController.ArrowPositionName).GetComponent<RectTransform>().localPosition;
        gameObject.transform.SetParent(btnTransform, false);
        gameObject.transform.localPosition = arrowPosition;
    }

    [ClientRpc]
    private void RpcFlipArrowHorizontal()
    {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        Vector3 localScale = rectTransform.localScale;
        rectTransform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
        _arrowText.transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
    }

    [ClientRpc]
    private void RpcSetArrowPlayerConnection(PlayerConnection playerConn)
    {
        _localPlayerConnection = playerConn;
    }

    [ClientRpc]
    private void RpcUpdateArrowPositionByPosition(Vector2 newPosition)
    {
        transform.localPosition = newPosition;
    }

    #endregion

    #region Server

    [Command(requiresAuthority = false)]
    private void CmdSetPrevAndCurrentSelectedButtons(string prevButtonName, string currentButtonName)
    {
        RpcSetPrevAndCurrentSelectedButtons(prevButtonName, currentButtonName);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateArrowPositionByPosition(Vector2 newPosition)
    {
        RpcUpdateArrowPositionByPosition(newPosition);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetArrowPlayerConnection(PlayerConnection playerConn)
    {
        RpcSetArrowPlayerConnection(playerConn);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateSelectedGameObject(GameObject newGObj)
    {
        PlayerSelectionUIController.instance.RpcSetSelectedGameObject(newGObj);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateArrowPosition(string buttonName)
    {
        int btnIndex = PlayerSelectionUIController.instance.MenuButtons.FindIndex(button => button.name == buttonName);
        RpcUpdateArrowPosition(btnIndex);
    }

    #endregion
}
