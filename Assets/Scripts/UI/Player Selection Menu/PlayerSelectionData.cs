using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;
using Mirror;
using System;

[Serializable]
public class PlayerSelectionData
{
    [SerializeField] private NetworkArrow _networkArrow;
    [SerializeField] private Penosas _selectedPenosa;

    public PlayerSelectionData()
    {
        _selectedPenosa = (Penosas)(-1);
    }

    public PlayerSelectionData(NetworkArrow networkArrow, Penosas selectedCharacter)
    {
        _networkArrow = networkArrow;
        _selectedPenosa = selectedCharacter;
    }

    public PlayerSelectionData(PlayerSelectionData newData)
    {
        _networkArrow = newData.NetworkArrow;
        _selectedPenosa = newData.SelectedPenosa;
    }

    public NetworkArrow NetworkArrow
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