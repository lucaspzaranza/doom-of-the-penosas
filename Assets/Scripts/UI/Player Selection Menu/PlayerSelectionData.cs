using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;

[System.Serializable]
public class PlayerSelectionData
{
    [SerializeField] private string name;
    [SerializeField] private Penosas _selectedPenosa;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private GameObject _prefab;

    public Vector2 PreviousCoordinate { get; set; }

    public Penosas SelectedPenosa
    {
        get => _selectedPenosa;
        set => _selectedPenosa = value;
    }

    public GameObject Arrow
    {
        get => _arrow;
        set => _arrow = value;
    }

    public GameObject Prefab => _prefab;
}