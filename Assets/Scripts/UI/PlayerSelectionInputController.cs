using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectionInputController : MonoBehaviour
{
    [SerializeField] private GameObject _1PArrow;
    [SerializeField] private GameObject _2PArrow;

    [SerializeField] private float _1stPlayerArrowXCoord;
    [SerializeField] private float _2ndPlayerArrowXCoord;

    private PlayerInput playerInputActions;
    private InputAction navigation;
    private int playerCount = 1;

    void Awake()
    {
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        navigation = playerInputActions.PlayerSelectionMenu.ArrowNavigation;
        navigation.performed += ChangePlayerSelection;
        navigation.Enable();
    }

    private void ChangePlayerSelection(InputAction.CallbackContext context)
    {
        float direction = navigation.ReadValue<float>();
        if (direction < 0) // Left
        {
            if(playerCount == 1)
            {
                _1PArrow.transform.localPosition = new Vector2(_1stPlayerArrowXCoord, transform.localPosition.y);
            }
        }
        else if (direction > 0) // Right
        {
            if (playerCount == 1)
            {
                _1PArrow.transform.localPosition = new Vector2(_2ndPlayerArrowXCoord, transform.localPosition.y);
            }
        }
    }
}
