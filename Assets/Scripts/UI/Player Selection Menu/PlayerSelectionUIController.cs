using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSelectionUIController : MonoBehaviour
{
    public static PlayerSelectionUIController instance;

    [SerializeField] private TMP_Text[] _penosasTexts;
    [SerializeField] private GameObject _startTxt;

    public TMP_Text[] PenosasTexts => _penosasTexts;
    public GameObject StartText => _startTxt;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
}
