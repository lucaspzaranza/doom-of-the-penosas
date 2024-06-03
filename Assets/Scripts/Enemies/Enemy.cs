using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : DamageableObject
{
    #region Vars

    public static Action<Enemy> OnEnemyDeath;
    public Action<EnemyState> OnEnemyChangedState;

    [Header("Enemy General Variables")]
    [SerializeField] private bool _isBoss;
    public bool IsBoss => _isBoss;

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

    // Conditional Editor Fields

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

    [SerializeField] protected bool _changeStateAfterDamage;
    public bool ChangeStateAfterDamage => _changeStateAfterDamage;

    [DrawIfBoolEqualsTo("_changeStateAfterDamage", true)]
    [SerializeField] protected EnemyState _stateAfterDamage;
    public EnemyState StateAfterDamage => _stateAfterDamage;

    // Basic fields

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

    //[SerializeField] protected PlayerDetector _playerDetector;
    //public PlayerDetector PlayerDetector => _playerDetector;

    [SerializeField] protected bool _isLeft;

    protected DamageableObject _detectedPlayer;
    protected EnemyState _previousState;
    protected Collider2D _enemyCollider;
    protected bool _collidedWithPlayer = false;
    protected bool _attackCanceled = false;
    protected List<EnemyWeaponDataListUnit> _weaponsWhichRotateTowardsPlayer;

    #endregion

    [SerializeField] private int weaponIndex;

    private void OnEnable()
    {
        EnemyStateGeneralData.EventHandlerSetup(this);
        UpdateWeaponSprites();
        EnemyStateGeneralData.DoInitialState();
        _enemyCollider = GetComponent<Collider2D>();

        _weaponsWhichRotateTowardsPlayer = WeaponController.WeaponDataList.
            Where(weapon => weapon.WeaponGameObjectData.RotateTowardsPlayer).
            ToList();
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
            Shoot(weaponIndex);
        }

        if (_weaponsWhichRotateTowardsPlayer.Count > 0)
            RotateWeaponsTowardsPlayer();

        // Detecção do jogador aqui é diferente. Se rotacionar atrás do jogador,
        // tem que usar a Overlap Area e ver se tá na distância permitida
        //if (PlayerDetector.DetectedPlayerNearObject(transform.position, out _detectedPlayer))
        if (WeaponController.WeaponDataList[0]
        .WeaponGameObjectData
        .PlayerDetector
        .DetectedPlayerNearObject(transform.position, out _detectedPlayer))
        {
            //print("Detected Player");
            if(State == EnemyState.Idle || State == EnemyState.Patrol)
            {
                ChangeState(EnemyState.ChasingPlayer);
                return;
            }
        }
        else if (CollidedWithPlayer(out _detectedPlayer))
        {
            //print("CollidedWithPlayer");
            if(InstantAttack)
            {
                int direction = GetDirection();
                //print($"Let's flip the enemy direction. Current direction: {direction}");
                Flip(direction);
                ChangeState(EnemyState.Attacking);
            }
            else if(!InstantAttack)
                ChangeState(EnemyState.ChasingPlayer);
        }

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

    protected void UpdateWeaponSprites()
    {
        foreach (var weaponData in WeaponController.WeaponDataList)
        {
            weaponData.WeaponGameObjectData.SetWeaponSprite(weaponData.WeaponScriptableObject.Sprite);
        }
    }

    protected virtual void RotateWeaponsTowardsPlayer()
    {
        _weaponsWhichRotateTowardsPlayer.ForEach(weapon => weapon.WeaponGameObjectData.RotateWeaponTowardsPlayer(_isLeft));
    }

    public virtual void PerformActionBasedOnState(EnemyState state) 
    {
        if (!CanStayIdle && state == EnemyState.Idle)
        {
            ChangeState(_stateToForceChangeIfIdle);
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
        //print($"ChangeState function | State: {State}, newState: {newState}");
        if (_state == newState)
        {
            //print("There is no need to change to the same state. Canceling ChangeState function");
            return;
        }

        if (InstantAttack && newState == EnemyState.ChasingPlayer)
        {
            //print("Can't chase, let's attack instantly!");
            newState = EnemyState.Attacking;
        }

        OnEnemyChangedState?.Invoke(newState);
    }

    /// <summary>
    /// Function to handle when the EnemyStateGeneralData successfully changed the current state to another.
    /// It'll handle the event fired to update the enemy states.
    /// </summary>
    /// <param name="newState">The new state to update the enemy current state.</param>
    public virtual void HandleOnStateChangedSuccess(EnemyState newState)
    {
        //print("HandleOnStateChangedSuccess: " + newState);
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

        float distance = Vector2.Distance(_detectedPlayer.transform.position, transform.position);
        print($"atk distance: {distance}. Ideal Distance is less or equal than: {AttackDistance}");
        bool reachedAtkdistance = distance <= AttackDistance;

        return reachedAtkdistance;
    }

    public virtual void Attack() { }

    public virtual void Shoot(int weaponIndex)
    {
        Transform spawnTransform = WeaponController.WeaponDataList[weaponIndex]
            .WeaponGameObjectData.SpawnTransform;
        WeaponController.WeaponDataList[weaponIndex].WeaponScriptableObject.Shoot(spawnTransform, GetDirection());
    }    

    protected int GetIndexFromClosestWeaponFromPlayer()
    {
        return 0;
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
            //print("newState: " + newState);

            if (!CanStayIdle && newState == EnemyState.Idle)
            {
                //print("Ops, enemy can't stay idle and the selected state is Idle! " +
                //    "So we'll pass it and maintain the current state...");
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
        LayerMask layerMask = WeaponController.WeaponDataList[0].WeaponGameObjectData.PlayerDetector.RaycastLayer;

        if (SharedFunctions.HitSomething(_enemyCollider, layerMask, out Collider2D hit)
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
            //PlayerDetector?.Flip();
            WeaponController.FlipWeaponsPlayerDetectors();
        }
        else if(direction > 0 && transform.localScale.x < 0)
        {
            _isLeft = false;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            //PlayerDetector?.Flip();
            WeaponController.FlipWeaponsPlayerDetectors();
        }
    }

    protected virtual void Death() 
    {
        if (FireEventWhenDead)
            OnEnemyDeath?.Invoke(this);

        Destroy(gameObject);
    }
}
