using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerUnit: ControllerBase
{
    [Header("Parent Controller")]
    [SerializeField] protected ControllerBase _parentController;

    public override void Setup()
    {
        base.Setup();
        _parentController = GetComponentInParent<GameController>();
    }
}
