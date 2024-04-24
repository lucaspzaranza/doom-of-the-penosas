using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public abstract class Enemy : MonoBehaviour
{
    public static Action<Enemy> OnEnemyDeath;
    
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

    [SerializeField] protected float _glowDuration;
    protected Color _colorBeforeGlow;
    protected bool _isGlowing;
    private float _glowIntervalTimeCounter;
    private const float _glowFrameInterval = 0.05f;
    protected float _glowTimeCounter;
    protected bool _isLeft;

    public virtual void Patrol() { }

    public virtual void TakeDamage(int damage) 
    {
        _life -= damage;
        _isGlowing = true;

        if (_life <= 0)
        {
            _life = 0;
            Death();
        }
    }

    protected virtual void Glow()
    {
        if (_glowTimeCounter < _glowDuration)
        {
            EnemySprites.ForEach(sprite =>
            {
                Color blinkColor = sprite.color == Color.red ? Color.black : Color.red;
                sprite.color = blinkColor;
            });
        }
        else
        {
            _isGlowing = false;
            EnemySprites.ForEach(sprite =>
            {
                sprite.color = Color.white;
            });

            _glowTimeCounter = 0f;
        }
    }

    public virtual void ChangeState(EnemyState newState)
    {
        _state = newState;
    }

    protected virtual void Move() { }

    public virtual void Shoot(int weaponId)
    {
        Vector2 direction = WeaponController.WeaponDataList.First
            (weaponData => weaponData.WeaponUnit.ID == weaponId).
            EnemyWeaponSpawnTransform.SpawnTransform.position;
        WeaponController.WeaponDataList[weaponId].WeaponUnit.Shoot(direction, GetShotDirection());
    }

    public virtual int GetShotDirection()
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

    protected virtual void FixedUpdate()
    {
        if (_isGlowing)
        {
            _glowIntervalTimeCounter += Time.fixedDeltaTime;
            _glowTimeCounter += Time.fixedDeltaTime;
            if (_glowIntervalTimeCounter >= _glowFrameInterval)
            {
                Glow();
                _glowIntervalTimeCounter = PlayerConsts.BlinkInitialValue;
            }
        }
    }
}
