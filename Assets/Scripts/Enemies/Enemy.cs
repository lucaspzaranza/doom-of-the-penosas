using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private int _life;
    public int Life
    {
        get => _life;
    }

    [SerializeField] private EnemyState _state;
    public EnemyState State => _state;

    public virtual void Patrol() { }

    public virtual void TakeDamage(int damage) { }

    public virtual void ChangeState(EnemyState newState)
    {
        _state = newState;
    }

    protected virtual void Move() { }

    protected virtual void Shoot() { }

    protected virtual void CheckForNewState() { }

    protected virtual void Death() { }
}
