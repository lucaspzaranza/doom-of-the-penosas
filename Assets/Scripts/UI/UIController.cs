using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : ControllerUnit
{
    public Action<GameMode> OnOnGameModeSelected;

    [Space]
    [Header("UI Controllers")]
    [SerializeField] private PlayerSelectionUIController _playerSelectionUIController;
    [SerializeField] private PlayerInGameUIController _playerInGameUIController;

    // Props
    public PlayerSelectionUIController PlayerSelectionUIController => _playerSelectionUIController;
    public PlayerInGameUIController PlayerInGameUIController => _playerInGameUIController;

    public override void Setup()
    {
        base.Setup();

        if(PlayerSelectionUIController != null)
        {
            PlayerSelectionUIController.Setup();
            PlayerSelectionUIController.OnGameModeButtonPressed += HandleOnGameModeButtonPressed;
        }
    }

    public override void Dispose()
    {
        PlayerSelectionUIController.OnGameModeButtonPressed -= HandleOnGameModeButtonPressed;
    }

    private void HandleOnGameModeButtonPressed(GameMode gameMode)
    {
        OnOnGameModeSelected?.Invoke(gameMode);
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

    public GameMode GetGameMode()
    {
        return ((GameController)_parentController).GameMode;
    }
}
