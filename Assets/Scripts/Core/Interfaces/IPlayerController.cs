using SharedData.Enumerations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{
    event Action<byte, bool> OnCountdownActivation;
    event Action<bool> OnPlayerPause;
    event Action<byte> OnPlayerGameOver;
    event Action<IReadOnlyList<Penosas>> OnPlayersExchanged;

    Vector2 PlayerStartPosition { get; }
    IList<IPlayerData> PlayersData { get; }
    Transform Transform { get; }
    GameMode GameMode { get; }
    GameStatus GameStatus { get; }
    GameObject GameObject { get; }

    void Setup(IReadOnlyList<Penosas> characters, IReadOnlyList<InputDevice> selectedDevices = null);
    void Dispose();
    void EventDispose();
    void ResetPlayerEquipmentData();
    bool GameIsPaused();
    GameObject RequestProjectileFromGameController(GameObject projectile);
    void InvokeOnPlayerPause(bool value);
    void AddPlayers();
    void AddOnCountdownActivationListener(Action<byte, bool> listener);
    void AddOnPlayerPauseListener(Action<bool> listener);
    void AddOnPlayerGameOverListener(Action<byte> listener);
    void AddOnPlayersExchangedListener(Action<IReadOnlyList<Penosas>> listener);
    void RemoveInputController();
    void ResetSinglePlayerData(byte playerID);
    void ExchangePlayers();
    void ChangePlayerCharacter(byte playerID, Penosas newCharacter);
}