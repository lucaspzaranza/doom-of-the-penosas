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

    [DrawIfBoolEqualsTo("_canStayIdle", false)]
    [SerializeField] private EnemyState _stateToForceChangeIfIdle;

    [Tooltip("Check this if you want this enemy to start attacking the player right after it " +
    "detects the player presence. Else, it'll chase the player.")]
    [SerializeField] protected bool _instantAttack;
    public bool InstantAttack => _instantAttack;

    [Tooltip("The distance the enemy will have to stay from the player to start its attack.")]
    [DrawIfBoolEqualsTo("_instantAttack", false)]
    [Range(0f, 5f)]
    [SerializeField] private float _attackDistance;
    public float AttackDistance => _attackDistance;

    [DrawIfBoolEqualsTo("_instantAttack", false)]
    [Range(0.5f, 1f)]
    [SerializeField] private float _attackMinDistance;

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

    [SerializeField] protected PlayerDetector _playerDetector;
    public PlayerDetector PlayerDetector => _playerDetector;

    [SerializeField] protected bool _isLeft;

    protected DamageableObject _detectedPlayer;
    protected EnemyState _previousState;
    protected Collider2D _enemyCollider;
    protected bool _collidedWithPlayer = false;
    protected bool _attackCanceled = false;

    private void OnEnable()
    {
        EnemyStateGeneralData.EventHandlerSetup(this);
        EnemyStateGeneralData.DoInitialState();
        _enemyCollider = GetComponent<Collider2D>();

        EnemyStateGeneralData.OnStateChangedSuccess += HandleOnStateChangedSuccess;
    }

    private void OnDisable()
    {
        EnemyStateGeneralData.EventHandlerDispose();
        EnemyStateGeneralData.OnStateChangedSuccess -= HandleOnStateChangedSuccess;
    }

    protected virtual void Update()
    {
        // Temporary for test usage only
        if (Input.GetKeyDown(KeyCode.O))
        {
            Shoot(0);
        }

        if (PlayerDetector.DetectedPlayerNearObject(out _detectedPlayer))
        {
            if(State == EnemyState.Idle || State == EnemyState.Patrol)
            {
                ChangeState(EnemyState.ChasingPlayer);
                return;
            }
        }
        else if (CollidedWithPlayer(out _detectedPlayer))
            ChangeState(EnemyState.ChasingPlayer);

        if (EnemyStateGeneralData.CurrentState != null)
        {
            PerformActionBasedOnState(State);

            CheckForNewStateTimeCounter += Time.deltaTime;
            if (CheckForNewStateTimeCounter > TimeToCheckNewState)
            {
                CheckForNewStateTimeCounter = 0f;
                CheckForNewRandomState(State);
            }
        }
        else
            CheckForNewRandomState(State);        
    }

    public virtual void PerformActionBasedOnState(EnemyState state) 
    {
        if (!CanStayIdle && state == EnemyState.Idle)
        {
            ChangeState(_stateToForceChangeIfIdle);
            return;
        }

        if(InstantAttack && state == EnemyState.ChasingPlayer)
        {
            ChangeState(EnemyState.Attacking);
            return;
        }

        EnemyStateGeneralData.DoAction(state);
    }

    /// <summary>
    /// Fire the event to cancel the current state and change it to the <i>newState</i> parameter. <br/>
    /// If the current state can't be canceled, the function will schedule the change only when the current state
    /// be performed.
    /// </summary>
    /// <param name="newState"></param>
    public virtual void ChangeState(EnemyState newState)
    {
        print($"ChangeState function. _state: {_state}, newState: {newState}");
        if (_state == newState)
            return;

        OnEnemyChangedState?.Invoke(newState);
    }

    /// <summary>
    /// Function to handle when the EnemyStateGeneralData successfully changed the current state to another.
    /// It'll handle the event fired to update the enemy states.
    /// </summary>
    /// <param name="newState">The new state to update the enemy current state.</param>
    public virtual void HandleOnStateChangedSuccess(EnemyState newState)
    {
        _previousState = _state;
        _state = newState;

        if (_previousState == EnemyState.Attacking && _attackCanceled)
            _attackCanceled = false;
    }

    public void ReturnToPreviousState()
    {
        _state = _previousState;
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

    public virtual void ChasePlayer() { }

    public virtual bool ReachedAttackDistance()
    {
        if (_detectedPlayer == null)
            return false;

        float distance = Vector2.Distance(transform.position, _detectedPlayer.transform.position);
        bool reachedAtkdistance = distance <= AttackDistance;

        return reachedAtkdistance;
    }

    public virtual void Attack() { }

    public virtual void Shoot(int weaponIndex)
    {
        Vector2 direction = WeaponController.WeaponDataList[weaponIndex]
            .EnemyWeaponSpawnTransform.SpawnTransform.position;
        WeaponController.WeaponDataList[weaponIndex].WeaponUnit.Shoot(direction, GetDirection());
    }

    protected virtual int GetDirection()
    {
        if(_detectedPlayer == null)
            return _isLeft ? -1 : 1;
        else
        {
            float distance = transform.position.x - _detectedPlayer.transform.position.x;
            return distance > 0? -1 : 1;
        }
    }

    protected virtual void CheckForNewRandomState(EnemyState enemyState) 
    {

        if (EnemyStateGeneralData.HasStateChangeScheduled)
            return;

        bool changeState = UnityEngine.Random.Range(1, 101) <= EnemyStateGeneralData.CurrentState.ChangeRate;
        if (changeState)
        {
            EnemyState newState = EnemyStateGeneralData.GetNewRandomState();
            print("newState: " + newState);

            if (!CanStayIdle && newState == EnemyState.Idle)
            {
                print("Ops, enemy can't stay idle and the selected state is Idle! " +
                    "So we'll pass it and maintain the current state...");
                return;
            }

            ChangeState(newState);
        }
    }

    /// <summary>
    /// Checks if there is some player colliding with the enemy. Useful to detect when some player is at the same enemy position.
    /// </summary>
    /// <param name="_detectedPlayer"></param>
    /// <returns></returns>
    public virtual bool CollidedWithPlayer(out DamageableObject _detectedPlayer)
    {
        DamageableObject damageableObject = null;

        if (SharedFunctions.HitSomething(_enemyCollider, PlayerDetector.RaycastLayer, out Collider2D hit)
        && hit.TryGetComponent(out damageableObject) && SharedFunctions.DamageableObjectIsPlayer(damageableObject))
        {
            _detectedPlayer = damageableObject;
            _collidedWithPlayer = true;
        }
        else
        {
            _detectedPlayer = null;
            _collidedWithPlayer = false;
        }

        return _collidedWithPlayer;
    }

    protected virtual void Flip(int direction)
    {
        if(direction < 0 && transform.localScale.x > 0)
        {
            _isLeft = true;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            PlayerDetector?.Flip();
        }
        else if(direction > 0 && transform.localScale.x < 0)
        {
            _isLeft = false;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            PlayerDetector?.Flip();
        }
    }

    protected virtual void Death() 
    {
        if (FireEventWhenDead)
            OnEnemyDeath?.Invoke(this);

        Destroy(gameObject);
    }
}
