using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.EventSystems;
using System;

public class LocalArrowPosition : MonoBehaviour
{
    private PlayerInput playerInputActions;
    private InputAction navigation;
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
        if(isPressing)
        {
            Vector2 pressed = navigation.ReadValue<Vector2>();
            if (pressed != Vector2.zero)
                StartCoroutine(nameof(UpdateArrowPosition));
        }
    }

    private void UpdateArrowPositionWrapper(CallbackContext callbackContext)
    {
        StartCoroutine(nameof(UpdateArrowPosition));
        isPressing = callbackContext.started;
    }

    private IEnumerator UpdateArrowPosition()
    {
        yield return new WaitForEndOfFrame();
        var selectedBtn = EventSystem.current.currentSelectedGameObject;
        UpdateArrowPosition(selectedBtn);
    }

    public void UpdateArrowPosition(GameObject buttonToNavigate)
    {
        var btnTransform = buttonToNavigate.transform;
        var arrowPosition = btnTransform.Find(PlayerSelectionUIController.ArrowPositionName).GetComponent<RectTransform>().localPosition;
        gameObject.transform.SetParent(btnTransform, false);
        gameObject.transform.localPosition = arrowPosition;
    }
}
