using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TextSOBase : ScriptableObject
{    
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private bool _skippable;
    public bool Skippable => _skippable;

    public abstract IReadOnlyList<Step> Steps { get; }
}

public abstract class TextSOBase<TStep> : TextSOBase where TStep : Step
{
    [SerializeField] protected List<TStep> _steps;
    public override IReadOnlyList<Step> Steps => _steps.Cast<Step>().ToList();
    public IReadOnlyList<TStep> TypedSteps => _steps;
}
