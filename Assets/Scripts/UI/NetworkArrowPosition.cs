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

    public static event Action OnChangeArrowPositionButtonPressed;

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

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerSelectionUIController.instance.NetworkArrows.Add(this);

        if (_selectedButton == null)
            _selectedButton = EventSystem.current.currentSelectedGameObject;

        if (hasAuthority && isClientOnly)
        {
            var arrowSibling = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentInChildren<NetworkArrowPosition>();
            print(arrowSibling);
            if (arrowSibling == null)
                CmdUpdateArrowPosition(EventSystem.current.currentSelectedGameObject);
            else
                print("tem que botar na outra galinha, ÓRR...");
        }
        // Quando rodar no cliente também rodará no host, então farei essa chamada pra atualizar
        // a posiçao no host de acordo com o que foi selecionado no host .
        else // HOST
            CmdUpdateArrowPosition(_selectedButton);
    }

    private void UpdateArrowPositionWrapper(CallbackContext callbackContext)
    {
        if (hasAuthority)
            StartCoroutine(nameof(UpdateArrowPositionCoroutine));
    }

    private IEnumerator UpdateArrowPositionCoroutine()
    {
        yield return new WaitForEndOfFrame();
        var selectedBtn = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(selectedBtn);
        CmdUpdateArrowPosition(selectedBtn);
        OnChangeArrowPositionButtonPressed?.Invoke();
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
