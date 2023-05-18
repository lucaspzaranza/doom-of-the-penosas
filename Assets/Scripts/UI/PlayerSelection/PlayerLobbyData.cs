using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;
using System;

[Serializable]
public class PlayerLobbyData
{
    [SerializeField] private Penosas _selectedPenosa;

    public PlayerLobbyData()
    {
        _selectedPenosa = (Penosas)(-1);
    }

    public PlayerLobbyData(Penosas selectedCharacter)
    {
        _selectedPenosa = selectedCharacter;
    }

    public PlayerLobbyData(PlayerLobbyData newData)
    {
        _selectedPenosa = newData.SelectedPenosa;
    }

    public Penosas SelectedPenosa
    {
        get => _selectedPenosa;
        set => _selectedPenosa = value;
    }
}