using Mirror;
using SharedData.Enumerations;
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
    [SerializeField] private GameObject _selectedButton;
    [SerializeField] private GameObject _previousSelectedButton;

    [SerializeField] private int _index;

    private RectTransform lobbyMenuTransform;
    private PlayerInput playerInputActions;
    private InputAction navigation;
    private bool updatedPosition = false;

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

        //CmdUpdateArrowPosition(_selectedButton);
    }

    //[ClientCallback]
    //private void Update()
    //{
    //    if(isClientOnly)
    //    {
    //        var arrowSibling = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentInChildren<NetworkArrowPosition>();
    //        print(arrowSibling);
    //        if (arrowSibling != null && !updatedPosition)
    //        {
    //            //print("tira isso daqui");
    //            Get1stPlayerCharacterButton();
    //            updatedPosition = true;
    //        }
    //    }
    //}

    private void SetNetworkArrowInitialPosition()
    {
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

        var buttonNetworkArrow = buttonToNavigate.GetComponentInChildren<NetworkArrowPosition>();
        
        bool invertPosition = false;
        if(buttonNetworkArrow != null)
            invertPosition = !Equals(this, buttonNetworkArrow);

        var btnTransform = buttonToNavigate.transform;
        var arrowPosition = btnTransform.Find(PlayerSelectionUIController.ArrowPositionName).GetComponent<RectTransform>().localPosition;
        gameObject.transform.SetParent(btnTransform, false);

        if(invertPosition) // Fazer a inversão do sprite da 2P NetworkArrow
            print("Já tem gente aqui! Fazer a inversão do sprite da 2P NetworkArrow...");

        gameObject.transform.localPosition = !invertPosition? arrowPosition : arrowPosition * -1;
    }
}
