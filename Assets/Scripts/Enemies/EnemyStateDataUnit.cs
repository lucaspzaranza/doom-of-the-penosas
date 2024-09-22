using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateDataUnit
{
    [SerializeField] private string _name;

    [SerializeField] private EnemyState _enemyState;
    public EnemyState EnemyState
    {
        get => _enemyState;
        set => _enemyState = value;
    }

    [SerializeField] private EnemyAction _action;
    public EnemyAction Action => _action;

    [Space]
    [Tooltip("The percentage (%) to change this state to another. Set this to 0 if you don't " +
    "want this state to be randomly changed.")]
    [Range(0, 100)]
    [SerializeField] private int _changeRate;
    public int ChangeRate => _changeRate;

    [Tooltip("The states you can randomly change this state to.")]
    [SerializeField] private List<EnemyState> _possiblesStatesToRandomlyChange;
    public List <EnemyState> PossiblesStatesToRandomlyChange => _possiblesStatesToRandomlyChange;
}