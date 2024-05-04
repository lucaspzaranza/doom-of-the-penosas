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
    public Action OnActionPerformed;

    private EnemyActionStatus _status;
    public EnemyActionStatus Status => _status;

    [Tooltip("If checked, the action may be interrupted by another when another Enemy State be chosen. " +
    "If not, the next state will start only when this action ends.")]
    [SerializeField] private bool _canBeCanceled;
    public bool CanBeCanceled => _canBeCanceled;

    public EnemyState EnemyState { get; set; }

    [SerializeField] private UnityEvent _actionToPerform;

    private bool CanPerformAction => _actionToPerform.GetPersistentEventCount() > 0;

    public void SetActionStatusStarted()
    {
        _status = EnemyActionStatus.Started;
        OnActionStarted?.Invoke(this);
    }

    public void SetActionStatusPerformed()
    {
        _status = EnemyActionStatus.Performed;
        OnActionPerformed?.Invoke();
    }

    public void SetActionStatusCanceled()
    {
        if (_canBeCanceled)
            _status = EnemyActionStatus.Canceled;
    }

    public void DoAction()
    {
        if (_status == EnemyActionStatus.Started && CanPerformAction)
            _actionToPerform?.Invoke();
    }
}