using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class NetworkArrowPosition : NetworkBehaviour
{
    private RectTransform lobbyMenuTransform;
    private PlayerInput playerInputActions;
    private InputAction navigation;
    private GameObject selectedButton;
    private bool isPressing;

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.started += UpdateArrowPositionWrapper;
        navigation.canceled += UpdateArrowPositionWrapper;
        navigation.Enable();
    }

    private void OnDisable()
    {
        navigation.started -= UpdateArrowPositionWrapper;
        navigation.canceled -= UpdateArrowPositionWrapper;
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

        print("CurrentSelectedGameObject: " + EventSystem.current.currentSelectedGameObject);
        if(hasAuthority)
            StartCoroutine(nameof(UpdateArrowPosition));
        else
        {
            // Tudo errado!
            //print(connectionToServer == null);
            //CmdUpdateArrowPosition(selectedButton);
        }
    }

    private void UpdateArrowPositionWrapper(CallbackContext callbackContext)
    {
        if(hasAuthority)
        {
            StartCoroutine(nameof(UpdateArrowPosition));
            isPressing = callbackContext.started;
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
}
