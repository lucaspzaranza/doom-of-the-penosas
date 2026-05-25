using UnityEngine;
using System.Collections;
using System;
using SharedData.Enumerations;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IPlayerLobbyUIController
{
    public event Action<GameMode> OnGameModeButtonPressed;
    public event Action<GameObject> OnGameReadyToStart;
    public event Action<IReadOnlyList<Penosas>> OnLobbySelectedCharacters;
    public event Action<IReadOnlyList<InputDevice>> OnLobbySelectedDevices;
    public event Action<Language> OnLobbySelectedLanguage;
    public event Action<bool> OnLobbySetNewGame;
    public event Action OnCancelSelection;

    GameObject GameObject { get; }

    void Setup();
    void Dispose();
}