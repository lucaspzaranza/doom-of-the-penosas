using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;
using System;

[Serializable]
public class PlayerSelectionData
{
    [SerializeField] private Penosas _selectedPenosa;

    public PlayerSelectionData()
    {
        _selectedPenosa = (Penosas)(-1);
    }

    public PlayerSelectionData(Penosas selectedCharacter)
    {
        _selectedPenosa = selectedCharacter;
    }

    public PlayerSelectionData(PlayerSelectionData newData)
    {
        _selectedPenosa = newData.SelectedPenosa;
    }

    public Penosas SelectedPenosa
    {
        get => _selectedPenosa;
        set => _selectedPenosa = value;
    }
}