using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerUnit: Controller
{
    public Action OnReferencesLoaded;

    [Header("Parent Controller")]
    [SerializeField] protected Controller _parentController;

    public virtual void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup) { }

    public override void Setup()
    {
        SetControllerFromParent<GameController>();
    }

    protected void SetControllerFromParent<T>() where T: Controller
    {
        _parentController = GetComponentInParent<T>();
    }

    public GameController TryToGetGameControllerFromParent()
    {
        GameController gameController = null;
        _parentController.TryGetComponent(out gameController);
        return gameController;
    }

    protected GameMode GetGameMode()
    {
        GameController gameCtrl = TryToGetGameControllerFromParent();
        if (gameCtrl != null)
            return gameCtrl.GameMode;
        else
            return ((ControllerUnit)_parentController).GetGameMode();
    }

    protected bool GetIsNewGame()
    {
        GameController gameCtrl = TryToGetGameControllerFromParent();
        if (gameCtrl != null)
            return gameCtrl.IsNewGame;
        else
            return ((ControllerUnit)_parentController).GetIsNewGame();
    }

    public bool GameIsPaused()
    {
        GameController gameCtrl = TryToGetGameControllerFromParent();
        if (gameCtrl != null)
            return gameCtrl.GameIsPaused;
        else
            return ((ControllerUnit)_parentController).GameIsPaused();
    }

    protected IReadOnlyList<Penosas> GetCharacterSelectionList()
    {
        GameController gameCtrl = TryToGetGameControllerFromParent();
        if (gameCtrl != null)
            return gameCtrl.CharacterSelectionList;
        else
            return ((ControllerUnit)_parentController).GetCharacterSelectionList();
    }

    public LanguageSO GetSelectedLanguage()
    {
        GameController gameCtrl = TryToGetGameControllerFromParent();
        if (gameCtrl != null)
            return gameCtrl.CurrentLanguage;
        else
            return ((ControllerUnit)_parentController).GetSelectedLanguage();
    }
}
