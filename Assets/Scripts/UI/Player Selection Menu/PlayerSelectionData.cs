using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;
using Mirror;
using System;

[System.Serializable]
public class PlayerSelectionData
{
    [SerializeField] private string name;
    [SerializeField] private Penosas _selectedPenosa;
    [SerializeField] private NetworkArrowPosition _networkArrow;

    public NetworkArrowPosition NetworkArrow
    {
        get => _networkArrow;
        set => _networkArrow = value;
    }

    public Penosas SelectedPenosa
    {
        get => _selectedPenosa;
        set => _selectedPenosa = value;
    }
}