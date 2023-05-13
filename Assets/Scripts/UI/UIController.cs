using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IController
{
    [Header("UI Controllers")]
    [SerializeField] private PlayerSelectionUIController _playerSelectionUIController;
    [SerializeField] private PlayerInGameUIController _playerInGameUIController;

    // Props
    public PlayerSelectionUIController PlayerSelectionUIController => _playerSelectionUIController;
    public PlayerInGameUIController PlayerInGameUIController => _playerInGameUIController;

    public void Setup()
    {
        
    }

    public void Dispose()
    {
        
    }

    public Text GetCountdownTextFromInGameController()
    {
        return PlayerInGameUIController.GetCountdownText();
    }

    public void GameOverActivation(bool val)
    {
        PlayerInGameUIController.GameOverContainerActivation(val);
    }

    public void InGameSetup()
    {
        PlayerInGameUIController.Setup();
    }
}
