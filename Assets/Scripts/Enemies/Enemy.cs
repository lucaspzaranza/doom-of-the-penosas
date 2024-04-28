using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : DamageableObject
{
    public static Action<Enemy> OnEnemyDeath;
    public Action<EnemyState> OnEnemyChangedState;

    [Header("Enemy General Variables")]
    [SerializeField] protected float _speed;
    public float Speed => _speed;

    [SerializeField] private EnemyType _enemyType;
    public EnemyType EnemyType => _enemyType;

    [DrawIfEnumEqualsTo("_enemyType", new EnemyType(), EnemyType.Land)]
    [SerializeField] protected LandCharacterProps _landCharacterProps;

    [DrawIfEnumEqualsTo("_enemyType", new EnemyType(), EnemyType.Flying)]
    [SerializeField] protected FlyingCharaterProps _flyingCharacterProps;

    [SerializeField] private EnemyState _state;
    public EnemyState State => _state;

    [SerializeField] protected float _timeToCheckNewState;
    public float TimeToCheckNewState => _timeToCheckNewState;

    [SerializeField] protected bool _isCritical;
    public bool IsCritical => _isCritical;

    [SerializeField] protected bool _fireEventWhenDead;
    public bool FireEventWhenDead => _fireEventWhenDead;

    [SerializeField] protected bool _canStayIdle;
    public bool CanStayIdle => _canStayIdle;

    [SerializeField] private EnemyWeaponController _weaponController;
    public EnemyWeaponController WeaponController => _weaponController;

    [SerializeField] protected Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [SerializeField] protected List<SpriteRenderer> _enemySprites;
    public List<SpriteRenderer> EnemySprites => _enemySprites;

    protected float _checkNewStateTimeCounter;
    protected float CheckForNewStateTimeCounter
    {
        get => _checkNewStateTimeCounter;
        set => _checkNewStateTimeCounter = value;
    }

    [SerializeField] protected EnemyStateGeneralData _enemyStateGeneralData;
    public EnemyStateGeneralData EnemyStateGeneralData => _enemyStateGeneralData;

    protected bool _isLeft;

    private void OnEnable()
    {
        EnemyStateGeneralData.EventHandlerSetup(this);
        EnemyStateGeneralData.DoInitialState();
    }

    private void OnDisable()
    {
        EnemyStateGeneralData.EventHandlerDispose();
    }

    protected virtual void Update()
    {
        // Temporary for test usage only
        if (Input.GetKeyDown(KeyCode.O))
        {
            Shoot(0);
        }

        //if (FoundPlayer() && (State == EnemyState.Idle || State == EnemyState.Patrol))
        //    ChangeState(EnemyState.ChasingPlayer);

        if (EnemyStateGeneralData.CurrentState != null)
        {
            PerformActionBasedOnState(State);

            CheckForNewStateTimeCounter += Time.deltaTime;
            if (CheckForNewStateTimeCounter > TimeToCheckNewState)
            {
                CheckForNewStateTimeCounter = 0f;
                CheckForNewState(State);
            }
        }
        else
            CheckForNewState(State);
    }

    public virtual bool FoundPlayer()
    {
        return false;
    }

    public virtual void PerformActionBasedOnState(EnemyState state) 
    {
        EnemyStateGeneralData.DoAction(state);
    }

    public virtual void ChangeState(EnemyState newState)
    {
        _state = newState;
        OnEnemyChangedState?.Invoke(_state);
    }

    protected override void SetLife(int value)
    {
        _life = value;

        if (_life <= 0)
        {
            _life = 0;
            Death();
        }
    }

    public virtual void Patrol() { }

    protected virtual void Move() { }

    public virtual void Shoot(int weaponId)
    {
        Vector2 direction = WeaponController.WeaponDataList.First
            (weaponData => weaponData.WeaponUnit.ID == weaponId).
            EnemyWeaponSpawnTransform.SpawnTransform.position;
        WeaponController.WeaponDataList[weaponId].WeaponUnit.Shoot(direction, GetDirection());
    }

    protected virtual int GetDirection()
    {
        return _isLeft ? -1 : 1;
    }

    protected virtual void CheckForNewState(EnemyState enemyState) 
    {
        bool changeState = UnityEngine.Random.Range(1, 101) <= EnemyStateGeneralData.CurrentState.ChangeRate;
        if (changeState)
        {
            EnemyState newState = EnemyStateGeneralData.GetNewRandomState();
            print($"Changing State to {newState}.");
            ChangeState(newState);
        }
    }

    protected virtual void Flip()
    {
        _isLeft = !_isLeft;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    protected virtual void Death() 
    {
        if (FireEventWhenDead)
            OnEnemyDeath?.Invoke(this);

        Destroy(gameObject);
    }
}
