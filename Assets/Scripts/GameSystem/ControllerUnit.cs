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
        //print($"Setting on {name} the parent Controller looking at the {transform.parent}.");
        GetControllerFromParent<GameController>();
    }

    protected void GetControllerFromParent<T>() where T: Controller
    {
        _parentController = (T)GetComponentInParent<T>();
    }

    /// <summary>
    /// If we pass the index 0, it'll return the index 1 and vice versa.
    /// </summary>
    public int GetComplementaryPlayerIndex(int currentIndex)
    {
        return (currentIndex + 1) % 2;
    }
}
