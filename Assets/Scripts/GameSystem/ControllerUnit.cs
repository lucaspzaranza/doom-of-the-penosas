using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerUnit: Controller
{
    [Header("Parent Controller")]
    [SerializeField] protected Controller _parentController;

    public override void Setup()
    {
        base.Setup();
        _parentController = GetComponentInParent<GameController>();
    }

    /// <summary>
    /// Returns the Game Mode from the main Game Controller.
    /// </summary>
    /// <returns>The Game Mode.</returns>
    public virtual GameMode GetGameMode()
    {
        return ((GameController)_parentController).GameMode;
    }
}
