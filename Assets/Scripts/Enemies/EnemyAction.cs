using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemyAction
{
    public Action<EnemyAction> OnActionStarted;
    public Action<bool> OnActionPerformed;

    [SerializeField]
    private EnemyActionStatus _status;
    public EnemyActionStatus Status
    {
        get => _status;
        set => _status = value;
    }

    [Tooltip("If checked, the action may be interrupted by another when another Enemy State be chosen. " +
    "If not, the next state will start only when this action ends.")]
    [SerializeField] private bool _canBeCanceled;
    public bool CanBeCanceled => _canBeCanceled;

    public EnemyState EnemyState { get; set; }

    [SerializeField] private UnityEvent _actionToPerform;

    private bool CanPerformAction => _actionToPerform.GetPersistentEventCount() > 0;

    public void SetActionStatusStarted()
    {
        Status = EnemyActionStatus.Started;
        OnActionStarted?.Invoke(this);
    }

    /// <param name="instantLoop">Set this param as true if you wish 
    /// this action stars right after it reached its end.</param>
    public void SetActionStatusPerformed(bool instantLoop = false)
    {
        Status = EnemyActionStatus.Performed;
        OnActionPerformed?.Invoke(instantLoop);
    }

    public void SetActionStatusCanceled()
    {
        if (_canBeCanceled)
            Status = EnemyActionStatus.Canceled;
    }

    public void DoAction()
    {
        if (CanPerformAction)
            _actionToPerform?.Invoke();
    }
}