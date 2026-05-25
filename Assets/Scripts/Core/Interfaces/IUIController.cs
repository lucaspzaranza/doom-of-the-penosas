using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IUIController
{
    event Action<IReadOnlyList<Penosas>> OnUISelectedCharacters;
    event Action<GameMode> OnUIGameModeSelected;
    event Action<bool> OnUISetNewGame;
    event Action<int> OnUIGameSelectedSceneIndex;
    event Action OnUIBackToMainMenuFromMapaMundi;
    event Action<IReadOnlyList<InputDevice>> OnUISelectedDevices;
    event Action<Language> OnUISelectedLanguage;
    Action<byte> OnCountdownIsOver { get; set; }
    // event Action<byte> OnCountdownIsOver;

    IPlayerLobbyUIController PlayerLobbyUIController { get; }
    IPlayerInGameUIController PlayerInGameUIController { get; }
    IMapaMundiController MapaMundiController { get; }

    void Setup();
    void Dispose();
    void DisposeLobbyController();
    void InstantiateMapaMundiController();
    void CountdownActivation(byte playerID, bool val);
    void SelectGameSceneCanvas();
    void InstantiatePlayerInGameUIController();
    void PauseMenuActivation(bool val);
    void GameOverActivation();
}
