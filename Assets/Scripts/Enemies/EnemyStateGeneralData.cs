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

    private Enemy _enemy;
    private EnemyStateDataUnit _cachedStateData;
    private bool _changedState;

    public void EventHandlerSetup(Enemy enemyToAssign)
    {
        foreach (var enemyActionData in _statesList)
        {
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
        EnemyStateDataUnit initialStateData = StatesList.Find(stateData => stateData.EnemyState == _initialState);
        initialStateData?.Action.SetActionStatusStarted();
        _changedState = true;
    }

    public EnemyState GetNewRandomState()
    {
        int length = CurrentState.PossiblesStatesToRandomlyChange.Count;
        int randomIndex = Random.Range(0, length);

        return CurrentState.PossiblesStatesToRandomlyChange[randomIndex];
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

    public void HandleOnActionStarted(EnemyAction enemyAction)
    {
        _currentState = StatesList.Find(stateData => stateData.Action.Equals(enemyAction));
    }

    public void HandleOnActionPerformed(EnemyAction enemyAction)
    {
        _currentState = null;
    }

    public void HandleOnEnemyChangedState(EnemyState newState)
    {
        EnemyStateDataUnit newStateToChange = StatesList.Find(state => state.EnemyState == newState);

        if(newStateToChange != null)
        {
            if (_currentState.Action.CanBeCanceled)
            {
                if (!_currentState.Equals(newStateToChange))
                    _changedState = true;

                _currentState.Action.SetActionStatusCanceled();
                newStateToChange.Action.SetActionStatusStarted();
            }
            else
            {
                // Schedule new state change only when curent State action be performed.
                // Use Movement Action as a first prototype to test this feature.
            }
        }
        else
            WarningMessages.EnemyActionNotFound(newState);
    }
}