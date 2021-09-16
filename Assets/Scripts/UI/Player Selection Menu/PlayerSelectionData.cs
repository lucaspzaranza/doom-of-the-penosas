using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSelectionData
{
    [SerializeField] private string name;
    public bool Active;

    [SerializeField] private Penosas _selectedPenosa;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private float _arrowXInitCoord;
    [SerializeField] private GameObject _prefab;

    public Penosas SelectedPenosa
    {
        get => _selectedPenosa;
        set => _selectedPenosa = value;
    }

    public float ArrowXInitCoord => _arrowXInitCoord;
    public GameObject Arrow => _arrow;
    public GameObject Prefab => _prefab;

    public void SetArrowXInitCoordValue(float newValue)
    {
        _arrowXInitCoord = newValue;
    }
}