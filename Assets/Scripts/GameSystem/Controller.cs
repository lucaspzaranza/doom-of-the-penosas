using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Controller : MonoBehaviour, IController
{
    [Header("Controller Prefabs")]
    [SerializeField] private List<Controller> _childControllers;
    public IReadOnlyList<Controller> ChildControllers => _childControllers;

    private static GameMode _gameMode;
    public GameMode GetGameMode() => _gameMode;

    public virtual void Setup() { }
    public virtual void Dispose() { }

    private void OnEnable()
    {
        UIController.OnGameModeSelected += SetGameMode;
    }

    private void OnDisable()
    {
        UIController.OnGameModeSelected -= SetGameMode;
    }

    private void SetGameMode(GameMode newGameMode)
    {
        _gameMode = newGameMode;
    }
}
