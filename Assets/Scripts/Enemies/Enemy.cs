using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : DamageableObject
{
    public static Action<Enemy> OnEnemyDeath;
    
    [SerializeField] protected float _speed;
    public float Speed => _speed;

    [SerializeField] private EnemyState _state;
    public EnemyState State => _state;

    [SerializeField] protected bool _isCritical;
    public bool IsCritical => _isCritical;

    [SerializeField] protected bool _fireEventWhenDead;

    [SerializeField] private EnemyWeaponController _weaponController;
    public EnemyWeaponController WeaponController => _weaponController;

    [SerializeField] protected Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [SerializeField] private bool _isLandCharacter;
    public bool IsLandCharacter => _isLandCharacter;

    [DrawIfBoolEqualsTo("_isLandCharacter", true)]
    [SerializeField] protected LandCharacterProps _landCharacterProps;

    [SerializeField] protected List<SpriteRenderer> _enemySprites;
    public List<SpriteRenderer> EnemySprites => _enemySprites;

    protected bool _isLeft;

    public virtual void Patrol() { }

    public virtual void ChangeState(EnemyState newState)
    {
        _state = newState;
    }

    protected override void SetLife(int value)
    {
        _life -= value;

        if (_life <= 0)
        {
            _life = 0;
            Death();
        }
    }

    protected virtual void Move() { }

    public virtual void Shoot(int weaponId)
    {
        Vector2 direction = WeaponController.WeaponDataList.First
            (weaponData => weaponData.WeaponUnit.ID == weaponId).
            EnemyWeaponSpawnTransform.SpawnTransform.position;
        WeaponController.WeaponDataList[weaponId].WeaponUnit.Shoot(direction, GetShotDirection());
    }

    protected virtual int GetShotDirection()
    {
        return _isLeft ? -1 : 1;
    }

    protected virtual void CheckForNewState() { }

    protected virtual void Flip()
    {
        _isLeft = !_isLeft;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    protected virtual void Death() 
    {
        if (_fireEventWhenDead)
            OnEnemyDeath?.Invoke(this);

        Destroy(gameObject);
    }
}
