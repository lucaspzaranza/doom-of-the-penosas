using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public abstract class Controller : MonoBehaviour, IController
{
    [Header("Controller Prefabs")]
    [SerializeField] private List<GameObject> _childControllersPrefabs;
    public List<GameObject> ChildControllersPrefabs => _childControllersPrefabs;

    private static GameMode _gameMode;
    public GameMode GetGameMode() => _gameMode;

    public abstract void Setup();
    public abstract void Dispose();

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
