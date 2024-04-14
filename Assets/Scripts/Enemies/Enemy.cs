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

    [SerializeField] protected float _speed;
    public float Speed => _speed;

    [SerializeField] private EnemyState _state;
    public EnemyState State => _state;

    [SerializeField] protected bool _isCritical;
    public bool IsCritical => _isCritical;

    [SerializeField] private EnemyWeaponController _weaponController;
    public EnemyWeaponController WeaponController => _weaponController;

    [SerializeField] protected Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [SerializeField] private bool _isLandCharacter;
    [DrawIfBoolEqualsTo("_isLandCharacter", true)]
    [SerializeField] protected LandCharacterProps _landCharacterProps;

    public virtual void Patrol() { }

    public virtual void TakeDamage(int damage) 
    {
        _life = Mathf.Clamp(damage, PlayerConsts.DeathLife, PlayerConsts.Max_Life);
    }

    public virtual void ChangeState(EnemyState newState)
    {
        _state = newState;
    }

    protected virtual void Move() { }

    protected virtual void Shoot(Vector2 coordinates) { }

    protected virtual void CheckForNewState() { }

    protected virtual void Death() { }
}
