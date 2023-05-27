using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.EventSystems;
using System;

public class CursorPosition : MonoBehaviour
{
    public static Action<CursorPosition, Vector2> OnCursorMoved;

    private PlayerInput playerInputActions;
    private InputAction navigation;
    private InputAction selection;
    private bool isPressing;
    private GameObject _currentSelected;
    private Vector2 _pressed;

    // Props
    private GameObject _lastSelected;
    public GameObject LastSelected => _lastSelected;

    private GameObject _currentPressed;
    public GameObject CurrentPressed => _currentPressed;

    private GameObject _lastPressed;
    public GameObject LastPressed => _lastPressed;

    [SerializeField] private GameObject _defaultCursorParent;
    public GameObject DefaultCursorParent => _defaultCursorParent;

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.CursorNavigation;
        navigation.started += UpdateCursorPositionWrapper;
        navigation.canceled += UpdateCursorPositionWrapper;
        navigation.Enable();

        selection = playerInputActions.PlayerSelectionMenu.SelectPlayer;
        selection.started += SetLastPressedButton;
        selection.Enable();

        EventSystem.current.SetSelectedGameObject(transform.parent.gameObject);
    }

    private void OnDisable()
    {
        navigation.started -= UpdateCursorPositionWrapper;
        navigation.canceled -= UpdateCursorPositionWrapper;
        navigation.Disable();

        selection.started -= SetLastPressedButton;
        selection.Disable();
    }

    void Update()
    {
        if(isPressing)
        {
            _pressed = navigation.ReadValue<Vector2>();
            if (_pressed != Vector2.zero)
                StartCoroutine(nameof(UpdateCursorPosition));
        }
    }

    private void UpdateCursorPositionWrapper(CallbackContext callbackContext)
    {
        if(callbackContext.started)
            _lastSelected = EventSystem.current.currentSelectedGameObject;
        StartCoroutine(nameof(UpdateCursorPosition));
        isPressing = callbackContext.started;
    }

    private void SetLastPressedButton(CallbackContext callbackContext)
    {
        _lastPressed = _currentPressed;
        _currentPressed = EventSystem.current.currentSelectedGameObject;
    }

    private IEnumerator UpdateCursorPosition()
    {
        yield return new WaitForEndOfFrame();
        var selectedBtn = EventSystem.current.currentSelectedGameObject;
        UpdateCursorPosition(selectedBtn);
    }

    public void UpdateCursorPosition(GameObject buttonToNavigate)
    {
        if (buttonToNavigate == null)
            return;

        var cursorPosition = buttonToNavigate.GetCursorPosition();

        gameObject.transform.SetParent(buttonToNavigate.transform, false);
        gameObject.transform.localPosition = cursorPosition;
        _currentSelected = buttonToNavigate;

        // Only sends one per time and if it is a horizontal move
        if(isPressing)
            OnCursorMoved?.Invoke(this, _pressed);
    }
}
