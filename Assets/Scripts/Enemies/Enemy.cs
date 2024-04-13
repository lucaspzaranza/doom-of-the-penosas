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

    [SerializeField] protected EnemyState _state;
    public EnemyState State => _state;

    [SerializeField] private EnemyWeaponController _weaponController;
    public EnemyWeaponController WeaponController => _weaponController;

    [SerializeField] protected Rigidbody2D _rigidBody;
    public Rigidbody2D RigidBody => _rigidBody;

    [SerializeField] protected Transform _shotSpawn;
    public Transform ShotSpawn => _shotSpawn;

    public virtual void Patrol() { }

    public virtual void TakeDamage(int damage) { }

    public virtual void ChangeState(EnemyState newState)
    {
        _state = newState;
    }

    protected virtual void Move() { }

    protected virtual void Shoot(Vector2 coordinates) { }

    protected virtual void CheckForNewState() { }

    protected virtual void Death() { }
}
