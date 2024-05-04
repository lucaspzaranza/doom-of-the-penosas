using JetBrains.Annotations;
using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemyStateGeneralData
{
    [SerializeField] private EnemyState _initialState;
    public EnemyState InitialState => _initialState;

    [SerializeField] private List<EnemyStateDataUnit> _statesList;
    public List<EnemyStateDataUnit> StatesList => _statesList;

    private EnemyStateDataUnit _currentState;
    public EnemyStateDataUnit CurrentState => _currentState;

    public bool HasStateChangeScheduled => _scheduledStateData != null;

    private Enemy _enemy;
    private EnemyStateDataUnit _cachedStateData;
    private EnemyStateDataUnit _scheduledStateData;
    private bool _changedState;

    public void EventHandlerSetup(Enemy enemyToAssign)
    {
        foreach (var enemyActionData in StatesList)
        {
            enemyActionData.Action.EnemyState = enemyActionData.EnemyState;
            enemyActionData.Action.OnActionStarted += HandleOnActionStarted;
            enemyActionData.Action.OnActionPerformed += HandleOnActionPerformed;
        }

        _enemy = enemyToAssign;
        _enemy.OnEnemyChangedState += HandleOnEnemyChangedState;
    }

    public void EventHandlerDispose()
    {
        foreach (var enemyActionData in _statesList)
        {
            enemyActionData.Action.OnActionStarted -= HandleOnActionStarted;
            enemyActionData.Action.OnActionPerformed -= HandleOnActionPerformed;
        }

        _enemy.OnEnemyChangedState -= HandleOnEnemyChangedState;
        _enemy = null;
    }

    public void DoInitialState()
    {
        if (InitialState != _enemy.State)
            _enemy.ChangeState(InitialState);
            
        EnemyStateDataUnit initialStateData = StatesList.Find(stateData => stateData.EnemyState == _initialState);
        initialStateData?.Action.SetActionStatusStarted();
        _changedState = true;
    }

    public EnemyState GetNewRandomState()
    {
        int length = CurrentState.PossiblesStatesToRandomlyChange.Count;
        int randomIndex = Random.Range(0, length);

        return length == 0? CurrentState.EnemyState : 
            CurrentState.PossiblesStatesToRandomlyChange[randomIndex];
    }

    public void DoAction(EnemyState enemyState)
    {
        if(_changedState)
        {
            _cachedStateData = StatesList.Find(stateData => stateData.EnemyState == enemyState);
            _changedState = false;
        }

        if (_cachedStateData != null)
            _cachedStateData.Action.DoAction();
        else
            WarningMessages.EnemyActionNotFound(enemyState);
    }

    public void SetCurrentActionAsPerformed()
    {
        CurrentState.Action.SetActionStatusPerformed();
    }

    public void HandleOnActionStarted(EnemyAction enemyAction)
    {
        _currentState = StatesList.Find(state => state.EnemyState == enemyAction.EnemyState);
    }

    public void HandleOnActionPerformed()
    {
        if (HasStateChangeScheduled)
        {
            _currentState = _scheduledStateData;
            _scheduledStateData = null;
        }

        CurrentState.Action.SetActionStatusStarted();
    }

    public void HandleOnEnemyChangedState(EnemyState newState)
    {
        EnemyStateDataUnit newStateToChange = StatesList.Find(state => state.EnemyState == newState);

        if(newStateToChange == null)
        {
            _enemy.ReturnToPreviousState();
            WarningMessages.EnemyActionNotFound(newState);
            return;
        }

        if (_currentState == null)
            return;

        if (_currentState.Action.CanBeCanceled)
        {
            if (!_currentState.Equals(newStateToChange))
                _changedState = true;

            _currentState.Action.SetActionStatusCanceled();
            newStateToChange.Action.SetActionStatusStarted();
        }
        else
            _scheduledStateData = newStateToChange;
    }
}