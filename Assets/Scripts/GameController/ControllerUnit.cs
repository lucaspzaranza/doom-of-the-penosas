using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerUnit: Controller
{
    [Header("Parent Controller")]
    [SerializeField] protected Controller _parentController;

    public virtual void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup) { }

    public override void Setup()
    {
        SetControllerFromParent<GameController>();
    }

    protected void SetControllerFromParent<T>() where T: Controller
    {
        //print($"Setting on {name} the parent Controller looking at the {transform.parent}.");
        _parentController = GetComponentInParent<T>();
    }

    protected GameMode GetGameMode()
    {
        GameController gameCtrl = null;
        if (_parentController.TryGetComponent(out gameCtrl))
            return gameCtrl.GameMode;
        else
            return ((ControllerUnit)_parentController).GetGameMode();
    }

    protected bool GetIsNewGame()
    {
        GameController gameCtrl = null;
        if (_parentController.TryGetComponent(out gameCtrl))
            return gameCtrl.IsNewGame;
        else
            return ((ControllerUnit)_parentController).GetIsNewGame();
    }

    protected IReadOnlyList<Penosas> GetCharacterSelectionList()
    {
        GameController gameCtrl = null;
        if (_parentController.TryGetComponent(out gameCtrl))
            return gameCtrl.CharacterSelectionList;
        else
            return ((ControllerUnit)_parentController).GetCharacterSelectionList();
    }

    /// <summary>
    /// If we pass the index 0, it'll return the index 1 and vice versa.
    /// </summary>
    public int GetComplementaryPlayerIndex(int currentIndex)
    {
        return (currentIndex + 1) % 2;
    }
}
