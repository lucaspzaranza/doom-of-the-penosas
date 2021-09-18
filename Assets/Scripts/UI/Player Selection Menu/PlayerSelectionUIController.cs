using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSelectionUIController : MonoBehaviour
{
    public static PlayerSelectionUIController instance;

    [SerializeField] private TMP_Text[] _penosasTexts;
    [SerializeField] private GameObject _startTxt;
    [SerializeField] private PlayerSelectionMenuState _menuState;

    public TMP_Text[] PenosasTexts => _penosasTexts;
    public GameObject StartText => _startTxt;
    public PlayerSelectionMenuState MenuState => _menuState;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        _menuState = PlayerSelectionMenuState.PlayerSelection;
    }

    public void SetState(PlayerSelectionMenuState newState)
    {
        _menuState = newState;
    }
}
