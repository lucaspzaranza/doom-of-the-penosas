using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class NetworkArrowPosition : NetworkBehaviour
{
    [SyncVar] [SerializeField] private GameObject _selectedButton;
    [SyncVar] [SerializeField] private GameObject _previousSelectedButton;

    [SerializeField] private int _index;

    private RectTransform lobbyMenuTransform;
    private PlayerInput playerInputActions;
    private InputAction navigation;
    private bool isPressing;

    public static event Action<UpdateArrowPositionEventArgs> OnChangeArrowPositionButtonPressed;

    public int Index => NetworkClient.isHostClient? 0 : 1;

    public GameObject SelectedButton => _selectedButton;
    public GameObject PreviousSelectedButton => _previousSelectedButton;

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.started += UpdateArrowPositionWrapper;
        navigation.Enable();
    }

    private void OnDisable()
    {
        navigation.started -= UpdateArrowPositionWrapper;
        navigation.Disable();
    }

    void Update()
    {
        if (isPressing)
        {
            Vector2 pressed = navigation.ReadValue<Vector2>();
            if (pressed != Vector2.zero)
                StartCoroutine(nameof(UpdateArrowPosition));
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (_selectedButton == null)
            _selectedButton = EventSystem.current.currentSelectedGameObject;

        if (hasAuthority && isClientOnly) 
            CmdUpdateArrowPosition(EventSystem.current.currentSelectedGameObject);
        // Quando rodar no cliente também rodará no host, então farei essa chamada pra atualizar
        // a posiçao no host de acordo com o que foi selecionado no host .
        else
            CmdUpdateArrowPosition(_selectedButton);
    }

    private void UpdateArrowPositionWrapper(CallbackContext callbackContext)
    {
        if(hasAuthority)
        {
            var eventArgs = new UpdateArrowPositionEventArgs(this, _selectedButton);
            StartCoroutine(nameof(UpdateArrowPosition));
            isPressing = callbackContext.started;
            OnChangeArrowPositionButtonPressed?.Invoke(eventArgs);
        }
    }

    private IEnumerator UpdateArrowPosition()
    {
        yield return new WaitForEndOfFrame();
        var selectedBtn = EventSystem.current.currentSelectedGameObject;
        CmdUpdateArrowPosition(selectedBtn);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateArrowPosition(GameObject buttonToNavigate)
    {
        RpcUpdateArrowPosition(buttonToNavigate);
    }

    [ClientRpc]
    public void RpcUpdateArrowPosition(GameObject buttonToNavigate)
    {
        _previousSelectedButton = _selectedButton;
        _selectedButton = buttonToNavigate;

        var btnTransform = buttonToNavigate.transform;
        var arrowPosition = btnTransform.Find(PlayerSelectionUIController.ArrowPositionName).GetComponent<RectTransform>().localPosition;
        gameObject.transform.SetParent(btnTransform, false);
        gameObject.transform.localPosition = arrowPosition;
    }
}
