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

    [Tooltip("The percentage (%) to change this state to another.")]
    [Range(0, 100)]
    [SerializeField] private int _changeRate;
    public int ChangeRate => _changeRate;

    [SerializeField] private EnemyAction _action;
    public EnemyAction Action => _action;

    [SerializeField] private List<EnemyState> _possiblesStatesToRandomlyChange;
    public List <EnemyState> PossiblesStatesToRandomlyChange => _possiblesStatesToRandomlyChange;
}