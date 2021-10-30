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
    [SyncVar] [SerializeField]
    private GameObject selectedButton;

    [SerializeField] private int _index;

    private RectTransform lobbyMenuTransform;
    private PlayerInput playerInputActions;
    private InputAction navigation;
    //private InputAction selection;
    private bool isPressing;

    public static event Action<UpdateArrowPositionEventArgs> OnChangeArrowPositionButtonPressed;
    //public static event Action<NetworkArrowPosition, Button> OnArrowSelectButtonPressed;
    //public static event Action<SelectButtonEventArgs> OnArrowSelectButtonPressed;

    public int Index => NetworkClient.isHostClient? 0 : 1;

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.started += UpdateArrowPositionWrapper;
        navigation.Enable();

        //selection = playerInputActions.PlayerSelectionMenu.SelectPlayer;
        //selection.started += OnArrowSelectedWrapper;
        //selection.Enable();
    }

    private void OnDisable()
    {
        navigation.started -= UpdateArrowPositionWrapper;
        navigation.Disable();

        //selection.started -= OnArrowSelectedWrapper;
        //selection.Disable();
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

        if (selectedButton == null)
            selectedButton = EventSystem.current.currentSelectedGameObject;

        if (hasAuthority && isClientOnly) 
            CmdUpdateArrowPosition(EventSystem.current.currentSelectedGameObject);
        // Quando rodar no cliente também rodará no host, então farei essa chamada pra atualizar
        // a posiçao no host de acordo com o que foi selecionado no host .
        else
            CmdUpdateArrowPosition(selectedButton);
    }

    private void UpdateArrowPositionWrapper(CallbackContext callbackContext)
    {
        if(hasAuthority)
        {
            var eventArgs = new UpdateArrowPositionEventArgs(this, selectedButton);
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
        selectedButton = buttonToNavigate;

        var btnTransform = buttonToNavigate.transform;
        var arrowPosition = btnTransform.Find(PlayerSelectionUIController.ArrowPositionName).GetComponent<RectTransform>().localPosition;
        gameObject.transform.SetParent(btnTransform, false);
        gameObject.transform.localPosition = arrowPosition;
    }

    //public void OnArrowSelectedWrapper(CallbackContext context)
    //{
    //    CmdOnArrowSelected(Index);
    //}

    //[Command]
    //public void CmdOnArrowSelected(int index)
    //{
    //    //print("Calling event with index: " + index);
    //    SelectButtonEventArgs eventArgs = new SelectButtonEventArgs(index, transform.parent.GetComponent<Button>());
    //    //OnArrowSelectButtonPressed?.Invoke(eventArgs);
    //}
}
