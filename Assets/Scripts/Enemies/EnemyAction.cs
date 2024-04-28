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
    public Action<EnemyAction> OnActionPerformed;

    [SerializeField] private EnemyActionStatus _status;
    public EnemyActionStatus Status => _status;

    [SerializeField] private bool _canBeCanceled;
    public bool CanBeCanceled => _canBeCanceled;

    [SerializeField] private UnityEvent _actionToPerform;

    private bool CanPerformAction => _actionToPerform.GetPersistentEventCount() > 0;

    public void SetActionStatusStarted(bool fireEvent = true)
    {
        _status = EnemyActionStatus.Started;
        if(fireEvent)
            OnActionStarted?.Invoke(this);
    }

    public void SetActionStatusPerformed(bool fireEvent = true)
    {
        _status = EnemyActionStatus.Performed;
        if(fireEvent)
            OnActionPerformed?.Invoke(this);
    }

    public void SetActionStatusCanceled(bool fireEvent = true)
    {
        if (_canBeCanceled)
            _status = EnemyActionStatus.Canceled;
    }

    public void DoAction()
    {
        if (_status == EnemyActionStatus.Started && CanPerformAction)
            _actionToPerform.Invoke();
    }
}