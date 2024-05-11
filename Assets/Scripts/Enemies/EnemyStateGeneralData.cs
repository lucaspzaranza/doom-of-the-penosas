using JetBrains.Annotations;
using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemyStateGeneralData
{
    public Action<EnemyState> OnStateChangedSuccess;

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
    private bool _changedState = false;

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
        OnStateChangedSuccess += _enemy.HandleOnStateChangedSuccess;
    }

    public void EventHandlerDispose()
    {
        foreach (var enemyActionData in _statesList)
        {
            enemyActionData.Action.OnActionStarted -= HandleOnActionStarted;
            enemyActionData.Action.OnActionPerformed -= HandleOnActionPerformed;
        }

        _enemy.OnEnemyChangedState -= HandleOnEnemyChangedState;
        OnStateChangedSuccess -= _enemy.HandleOnStateChangedSuccess;
        _enemy = null;
    }

    public void DoInitialState()
    {
        if (InitialState != _enemy.State)
            _enemy.ChangeState(InitialState);
            
        EnemyStateDataUnit initialStateData = StatesList.Find(stateData => stateData.EnemyState == _initialState);
        initialStateData?.Action.SetActionStatusStarted();
        Debug.Log("_changedState = true");
        _changedState = true;
    }

    public EnemyState GetNewRandomState()
    {
        int length = CurrentState.PossiblesStatesToRandomlyChange.Count;
        int randomIndex = UnityEngine.Random.Range(0, length);
        Debug.Log("randomIndex: " + randomIndex.ToString() + " CurrentState.EnemyState: " + 
            CurrentState.EnemyState.ToString());

        return length == 0? CurrentState.EnemyState : 
            CurrentState.PossiblesStatesToRandomlyChange[randomIndex];
    }

    public void DoAction(EnemyState enemyState)
    {
        if(_changedState)
        {
            Debug.Log("Changed state to " + enemyState);
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

    public void HandleOnActionPerformed(bool instantLoop = false)
    {
        Debug.Log("HandleOnActionPerformed");
        if (HasStateChangeScheduled)
        {
            Debug.Log("Starting Scheduled State: " + _scheduledStateData.EnemyState + 
                " Action Status: " + _scheduledStateData.Action.Status);

            if (!_currentState.Equals(_scheduledStateData))
                _changedState = true;

            _currentState = _scheduledStateData;
            _scheduledStateData = null;
            CurrentState.Action.SetActionStatusStarted();
            OnStateChangedSuccess?.Invoke(CurrentState.EnemyState);
        }

        if(instantLoop)
            CurrentState.Action.SetActionStatusStarted();
    }

    public void HandleOnEnemyChangedState(EnemyState newState)
    {
        Debug.Log("newState: " + newState);
        EnemyStateDataUnit newStateToChange = StatesList.Find(state => state.EnemyState == newState);

        Debug.Log("newStateToChange.EnemyState: " + newStateToChange.EnemyState);
        Debug.Log("_currentState == null: " + (_currentState == null).ToString());

        if(newStateToChange == null)
        {
            _enemy.ReturnToPreviousState();
            WarningMessages.EnemyActionNotFound(newState);
            return;
        }

        if (_currentState == null)
        {
            OnStateChangedSuccess?.Invoke(newStateToChange.EnemyState);
            return;
        }

        if (_currentState.Action.CanBeCanceled)
        {
            if (!_currentState.Equals(newStateToChange))
                _changedState = true;

            _currentState.Action.SetActionStatusCanceled();
            newStateToChange.Action.SetActionStatusStarted();
            OnStateChangedSuccess?.Invoke(newStateToChange.EnemyState);
        }
        else
        {
            Debug.Log($"Action can't be immediately canceled, so let's schedule the {newState} action");
            _scheduledStateData = newStateToChange;
        }
    }
}