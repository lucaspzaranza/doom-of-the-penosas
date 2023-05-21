using SharedData.Enumerations;
using System;
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

    /// <summary>
    /// Returns the Controller prefab from the list of prefabs this Controller has access.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>The prefab GameObject.</returns>
    public GameObject GetControllerFromPrefabList<T>() where T: Controller
    {
        var prefab = ChildControllersPrefabs.SingleOrDefault(c => c.GetComponent<T>());
        return prefab;
    }
}
