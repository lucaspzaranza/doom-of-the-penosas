using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : DamageableObject
{
    #region Vars

    public static Action<Enemy> OnEnemyDeath;
    public Action<EnemyState> OnEnemyChangedState;
    public Action OnEnemyFlip;

    [Header("Enemy General Variables")]
    [SerializeField] private bool _isBoss;
    public bool IsBoss => _isBoss;

    [SerializeField] protected bool _isCritical;
    public bool IsCritical => _isCritical;

    [SerializeField] private bool _canMove;
    public bool CanMove => _canMove;

    [DrawItDisabled]
    [SerializeField] protected bool _isMoving;
    public bool IsMoving => _isMoving;

    [SerializeField] protected bool _hasAttackWaves;
    public bool HasAttackWaves => _hasAttackWaves;

    [SerializeField] protected float _speed;
    public float Speed => _speed;
    
    [SerializeField] private EnemyType _enemyType;
    public EnemyType EnemyType => _enemyType;

    [DrawIfEnumEqualsTo("_enemyType", new EnemyType(), EnemyType.Flying)]
    [SerializeField] protected FlyingChaseMode _flyingChaseMode;
    public FlyingChaseMode FlyingChaseMode => _flyingChaseMode;

    [SerializeField] protected EnemyFireType _fireType;
    public EnemyFireType FireType => _fireType;

    [DrawIfEnumEqualsTo("_enemyType", new EnemyType(), EnemyType.Land)]
    [SerializeField] protected LandCharacterProps _landCharacterProps;

    [DrawIfEnumEqualsTo("_enemyType", new EnemyType(), EnemyType.Flying)]
    [SerializeField] protected FlyingCharaterProps _flyingCharacterProps;

    [SerializeField] private EnemyState _state;
    public EnemyState State => _state;

    [SerializeField] protected float _timeToCheckNewState;
    public float TimeToCheckNewState => _timeToCheckNewState;

    [SerializeField] protected bool _fireEventWhenDead;
    public bool FireEventWhenDead => _fireEventWhenDead;

    [SerializeField] protected bool _canStayIdle;
    public bool CanStayIdle => _canStayIdle;

    [SerializeField] protected bool _canFlip = true;
    public bool CanFlip => _canFlip;

    // Conditional Editor Fields

    [DrawIfBoolEqualsTo("_canStayIdle", false)]
    [SerializeField] private EnemyState _stateToForceChangeIfIdle;

    [Tooltip("Check this if you want this enemy to start attacking the player right after it " +
    "detects the player presence. Else, it'll chase the player.")]
    [SerializeField] protected bool _instantAttack;
    public bool InstantAttack => _instantAttack;

    [Tooltip("The distance the enemy will have to stay from the player to start its attack.")]
    //[DrawIfBoolEqualsTo("_instantAttack", false)]
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

    [DrawIfBoolEqualsTo("_hasAttackWaves", true)]
    [SerializeField]
    protected AttackWavesController _attackWaveController;
    public AttackWavesController AttackWavesController => _attackWaveController;

    // Basic fields

    [SerializeField] private EnemyWeaponController _weaponController;
    public EnemyWeaponController WeaponController => _weaponController;

    [SerializeField] protected Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    protected float _checkNewStateTimeCounter;
    protected float CheckForNewStateTimeCounter
    {
        get => _checkNewStateTimeCounter;
        set => _checkNewStateTimeCounter = value;
    }

    [SerializeField] protected EnemyStateGeneralData _enemyStateGeneralData;
    public EnemyStateGeneralData EnemyStateGeneralData => _enemyStateGeneralData;

    protected bool IsUsingWeaponWhichRotates => WeaponController.HasWeaponWhichRotates();

    [SerializeField] protected bool _isLeft;
    public bool IsLeft => _isLeft;

    [DrawIfEnumEqualsTo("_enemyType", new EnemyType(), EnemyType.Flying)]
    [SerializeField] protected bool _isDown;
    public bool IsDown => _isDown;

    protected int _selectedWeaponIndex;
    public int SelectedWeaponIndex
    {
        get => _selectedWeaponIndex;
        protected set => _selectedWeaponIndex = value;
    }

    [SerializeField] protected float _movementDuration;
    [SerializeField] protected float _timeToInvertMovement;
    [SerializeField] protected float _intervalBetweenAttacks;

    [DrawIfEnumEqualsTo("_flyingChaseMode", new FlyingChaseMode(), FlyingChaseMode.Mixed)]
    [Tooltip("The distance limit from X and Y Axis used by an enemy with Mixed Flying Chase Mode. " +
    "If the distance is greater than this value, the enemy will choose the axis with the longer distance.")]
    [SerializeField] protected float _distanceFromAxes;

    protected float _intervalBetweenAttacksTimeCounter;
    protected float _attackDurationTimeCounter;
    protected float _fireRateCounter;
    protected DamageableObject _detectedPlayer;
    protected EnemyState _previousState;
    protected Collider2D _enemyCollider;
    protected bool _collidedWithPlayer = false;
    protected bool _attackCanceled = false;
    protected bool _moveFlyingOnHorizontal = false;
    protected bool _moveFlyingOnVertical = false;
    protected List<EnemyWeaponDataListUnit> _weaponsWhichRotateTowardsPlayer;
    protected float _xDirection;
    protected float _yDirection;
    protected float _movementTimeCounter;
    protected float _timeToInvertMovementTimeCounter;

    [Tooltip("This is the weapon which will be used for detect the player's presence in common enemies.")]
    [SerializeField] protected int _frontalWeaponIndex;
    public int FrontalWeaponIndex => _frontalWeaponIndex;

    #endregion

    protected virtual void OnEnable()
    {
        EnemyStateGeneralData.EventHandlerSetup(this);
        UpdateWeaponSprites();
        EnemyStateGeneralData.DoInitialState();
        _enemyCollider = GetComponent<Collider2D>();

        _weaponsWhichRotateTowardsPlayer = WeaponController.WeaponDataList.
            Where(weapon => weapon.WeaponGameObjectData.RotateTowardsPlayer).
            ToList();

        if (_hasAttackWaves)
        {
            AttackWavesController.SetWeaponController(WeaponController);
            AttackWavesController.ChooseAttackWave(0);
        }
    }

    protected void Start()
    {
        SetRandomFlyingMixedMovement();
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
            //Shoot(weaponIndex);
            SelectWeaponOrWaveAndAttack();
        }

        if (_weaponsWhichRotateTowardsPlayer.Count > 0)
            RotateWeaponsTowardsPlayer();

        // Detec��o do jogador aqui � diferente. Se rotacionar atr�s do jogador,
        // tem que usar a Overlap Area e ver se t� na dist�ncia permitida
        if (DetectedPlayer())
        {
            if (State == EnemyState.Idle || State == EnemyState.Patrol)
            {
                print("Detected Player");
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

    protected virtual bool DetectedPlayer()
    {
        return WeaponController.WeaponDataList[FrontalWeaponIndex].WeaponGameObjectData
            .PlayerDetector.DetectedPlayerNearObject(out _detectedPlayer);
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
        _weaponsWhichRotateTowardsPlayer.
            ForEach(weapon => weapon.WeaponGameObjectData.RotateWeaponTowardsPlayer());
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

    public virtual void Patrol()
    {
        if (CanMove)
            Move();
    }

    protected virtual void Move()
    {
        if (_movementTimeCounter < _movementDuration)
        {
            _movementTimeCounter += Time.deltaTime;

            if (EnemyType == EnemyType.Land)
                MoveLandEnemy();
            else if (EnemyType == EnemyType.Flying)
                MoveFlyingEnemy();
        }
        else
        {
            _timeToInvertMovementTimeCounter += Time.deltaTime;
            _isMoving = false;

            if (_timeToInvertMovementTimeCounter > _timeToInvertMovement)
            {
                _movementTimeCounter = 0;
                _timeToInvertMovementTimeCounter = 0;

                if (EnemyType == EnemyType.Flying)
                {
                    if (FlyingChaseMode == FlyingChaseMode.Diagonal)
                    {
                        FlipVerticalDirection();
                        Flip(transform.localScale.x < 0 ? 1 : -1);
                    }
                    else if (FlyingChaseMode == FlyingChaseMode.Vertical)
                        FlipVerticalDirection();
                    else if (FlyingChaseMode == FlyingChaseMode.Mixed)
                    {
                        SetRandomFlyingMixedMovement();

                        if (_moveFlyingOnHorizontal)
                            Flip(transform.localScale.x < 0 ? 1 : -1);

                        if (_moveFlyingOnVertical)
                            FlipVerticalDirection();
                    }
                    else // Horizontal chasing mode
                        Flip(transform.localScale.x < 0 ? 1 : -1);
                }
                else // Land Character
                    Flip(transform.localScale.x < 0 ? 1 : -1);

                EnemyStateGeneralData.SetCurrentActionAsPerformed();
            }
        }
    }
    protected virtual void MoveLandEnemy()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _landCharacterProps.WallCheckCollider,
            _landCharacterProps.TerrainWithoutPlatformLayerMask,
            out Collider2D hitWall)
        )
        {
            _isMoving = true;
            _xDirection = GetDirection() * _speed;

            Vector2 direction = new Vector2(_xDirection, Rigidbody.velocity.y);
            Rigidbody.velocity = direction;
            Flip((int)_xDirection);
        }
        else
            _isMoving = false;
    }

    protected virtual void MoveFlyingEnemy()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _flyingCharacterProps.FlyingCheckCollider,
            _flyingCharacterProps.FlyingLayerMask,
            out Collider2D hitWall))
        {
            _isMoving = true;
            _xDirection = 0f;
            _yDirection = 0f;

            if (FlyingChaseMode == FlyingChaseMode.Horizontal)
                _xDirection = GetDirection() * _speed;
            else if (FlyingChaseMode == FlyingChaseMode.Vertical)
                _yDirection = GetDirection(calculateInVerticalAxis: true) * _speed;
            else if (FlyingChaseMode == FlyingChaseMode.Diagonal)
            {
                _xDirection = GetDirection() * _speed;
                _yDirection = GetDirection(calculateInVerticalAxis: true) * _speed;
            }
            else if (FlyingChaseMode == FlyingChaseMode.Mixed)
            {
                _xDirection = _moveFlyingOnHorizontal ? GetDirection() * _speed : 0f;
                _yDirection = _moveFlyingOnVertical ? GetDirection(calculateInVerticalAxis: true) * _speed : 0f;
            }

            Vector2 direction = new Vector2(_xDirection, _yDirection);
            transform.Translate(direction * Time.deltaTime);

            Flip((int)_xDirection);
            if (_yDirection != 0f)
                _isDown = _yDirection < 0f;
        }
        else
            _isMoving = false;
    }


    public virtual void ChasePlayer()
    {
        print("ChasePlayer");
        if (_detectedPlayer == null)
        {
            print("Lost player from sight. Returning to initial state...");
            ChangeState(EnemyStateGeneralData.InitialState);
            _isMoving = false;
            return;
        }

        if (ReachedAttackDistance())
        {
            print("Reached attack distance, so let's attack.");
            if (_collidedWithPlayer)
            {
                float distance = transform.position.x - _detectedPlayer.transform.position.x;
                Flip(distance < 0 ? 1 : -1);
            }
            _isMoving = false;
            ChangeState(EnemyState.Attacking);
        }
        else if (CanMove)
        {
            if (EnemyType == EnemyType.Land)
                MoveLandEnemy();
            else if (EnemyType == EnemyType.Flying)
            {
                if (FlyingChaseMode != FlyingChaseMode.Mixed)
                    MoveFlyingEnemy();
                else
                    ChasePlayerFlyingOnMixedMode();
            }
        }
    }

    public override void TakeDamage(int damage, bool force = false)
    {
        base.TakeDamage(damage, force);

        if (_tookDamage && ChangeStateAfterDamage && State == EnemyState.Idle)
        {
            //print($"Took damage, changing state to {StateAfterDamage}...");
            ChangeState(StateAfterDamage);
        }
    }

    protected virtual void ChasePlayerFlyingOnMixedMode()
    {
        if (Rigidbody != null && !SharedFunctions.HitSomething(
            _flyingCharacterProps.FlyingCheckCollider,
            _flyingCharacterProps.FlyingLayerMask,
            out Collider2D hitWall))
        {
            _isMoving = true;
            _xDirection = 0f;
            _yDirection = 0f;

            float xDistance = Mathf.Abs(transform.position.x - _detectedPlayer.transform.position.x);
            float yDistance = Mathf.Abs(transform.position.y - _detectedPlayer.transform.position.y);
            float axisDistance = xDistance - yDistance;

            if (Mathf.Abs(axisDistance) > _distanceFromAxes)
            {
                if (axisDistance >= 0f)
                {
                    _moveFlyingOnHorizontal = true;
                    _moveFlyingOnVertical = false;
                    _xDirection = GetDirection() * _speed;
                }
                else if (axisDistance < 0f)
                {
                    _moveFlyingOnHorizontal = false;
                    _moveFlyingOnVertical = true;
                    _yDirection = GetDirection(calculateInVerticalAxis: true) * _speed;
                }
            }
            else if (!ReachedAttackDistance())
            {
                _moveFlyingOnHorizontal = true;
                _moveFlyingOnVertical = true;
                _xDirection = GetDirection() * _speed;
                _yDirection = GetDirection(calculateInVerticalAxis: true) * _speed;
            }

            Vector2 direction = new Vector2(_xDirection, _yDirection);
            transform.Translate(direction * Time.deltaTime);

            Flip((int)_xDirection);
            if (_yDirection != 0f)
                _isDown = _yDirection < 0f;
        }
        else
            _isMoving = false;
    }

    protected float GetDistanceFromPlayer()
    {
        if (_detectedPlayer == null)
            return float.PositiveInfinity;

        return Vector2.Distance(_detectedPlayer.transform.position, transform.position);
    }

    public virtual bool ReachedAttackDistance()
    {
        if (_detectedPlayer == null)
            return false;

        float distance = GetDistanceFromPlayer();
        //print($"atk distance: {distance}. Ideal Distance is less or equal than: {AttackDistance}");
        bool reachedAtkdistance = distance <= AttackDistance;

        return reachedAtkdistance;
    }

    public virtual void Attack()
    {
        EnemyActionStatus status = EnemyStateGeneralData.CurrentState.Action.Status;
        //print("Attack(); status: " + status + " Current state: " + EnemyStateGeneralData.CurrentState.EnemyState);
        //print("SelectedWeaponIndex: " + SelectedWeaponIndex);

        if (status == EnemyActionStatus.Started) // Single weapon logic. Expand to suport multiple weapons
        {
            if (_attackDurationTimeCounter <=
                WeaponController.WeaponDataList[SelectedWeaponIndex].WeaponScriptableObject.AttackDuration)
            {
                if (_fireRateCounter >
                    WeaponController.WeaponDataList[SelectedWeaponIndex].WeaponScriptableObject.FireRate)
                {
                    //print("SHOOT");
                    SelectWeaponOrWaveAndAttack();
                    _fireRateCounter = 0;
                }
                else
                    _fireRateCounter += Time.deltaTime;

                _attackDurationTimeCounter += Time.deltaTime;
            }
            else
            {
                //print("End of attack wave.");
                EnemyStateGeneralData.SetCurrentActionAsPerformed();
                return;
            }
        }
        else if (status == EnemyActionStatus.Performed)
        {
            _intervalBetweenAttacksTimeCounter += Time.deltaTime;

            if (_intervalBetweenAttacksTimeCounter >= _intervalBetweenAttacks)
            {
                //print("Starting another attack wave...");
                _intervalBetweenAttacksTimeCounter = 0;
                _attackDurationTimeCounter = 0;
                _fireRateCounter = 0;
                EnemyStateGeneralData.CurrentState.Action.SetActionStatusStarted();
                return;
            }
        }

        if (!_attackCanceled && LostPlayerFromSight())
        {
            _attackCanceled = true;
            _fireRateCounter = 0;
            _attackDurationTimeCounter = 0;

            if (InstantAttack)
            {
                print("Couldn't attack player anymore, so let's return to Initial State.");
                ChangeState(EnemyStateGeneralData.InitialState);
            }
            else
            {
                print("Couldn't attack player anymore, so let's chase him.");
                ChangeState(EnemyState.ChasingPlayer);
            }
        }
    }

    protected virtual bool LostPlayerFromSight()
    {
        if (!IsUsingWeaponWhichRotates)
        {
            // If it's NOT an InstantAttack, it means the enemy must approach the player to start the attack.
            // Therefore it must detect if the player is out of a strikeable distance to set him lost from sight.
            if (!InstantAttack)
                return !ReachedAttackDistance();
            // Else, it means the enemy mustn't calculate any distance from player to start its attack.
            // As soon the enemy detects players presence, it'll start attacking.
            // So the detection will be on player's presence without any need of attack calculation distance.
            else
                return !DetectedPlayer();
        }
        else
        {
            // If the weapon rotates towards player and if the enemy doesn't detect any player's presence, 
            // it means the enemy lost him from sight;
            if (!DetectedPlayer())
                return true;

            // Else and there's some player detected, the enemy must check if the player is out of some
            // strikeable distance to lose him from sight.
            return !ReachedAttackDistance();
        }
    }

    public virtual void SelectWeaponOrWaveAndAttack()
    {
        if (!HasAttackWaves)
        {
            if (FireType == EnemyFireType.Individual)
            {
                SelectedWeaponIndex = _detectedPlayer is not null ?
                    WeaponController.GetClosestWeaponIndexFrom(_detectedPlayer.transform.position)
                    : 0;

                Shoot(SelectedWeaponIndex);
            }
            else
            {
                for (int i = 0; i < WeaponController.WeaponDataList.Count; i++)
                {
                    Shoot(i);
                }
            }
        }
        else if(!AttackWavesController.CurrentWave.WaveStarted)
        {
            AttackWavesController.ChooseRandomAttackWave(IsCritical);
            AttackWavesController.StartAttackWave();
        }
    }

    public virtual void Shoot(int weaponIndex)
    {
        Transform spawnTransform = WeaponController.WeaponDataList[weaponIndex]
            .WeaponGameObjectData.SpawnTransform;

        EnemyWeaponDataListUnit weapon = WeaponController.WeaponDataList[weaponIndex];

        if(weapon.WeaponGameObjectData.gameObject.activeInHierarchy)
        {
            int direction = GetShotDirection(weapon.WeaponGameObjectData);
            weapon.WeaponScriptableObject.Shoot(spawnTransform, direction);
        }
        else
            WarningMessages.EnemyWeaponIsInactive(name, weapon.WeaponGameObjectData.name);
    }    

    /// <summary>
    /// Get direction for moving purposes.
    /// </summary>
    /// <param name="calculateInVerticalAxis"></param>
    /// <returns></returns>
    protected virtual int GetDirection(bool calculateInVerticalAxis = false)
    {
        if(!calculateInVerticalAxis)
        {
            if (_detectedPlayer == null)
                return IsLeft ? -1 : 1;
            else
            {
                float distance = transform.position.x - _detectedPlayer.transform.position.x;
                return distance > 0 ? -1 : 1;
            }
        }
        else
        {
            if (_detectedPlayer == null)
                return IsDown ? -1 : 1;
            else
            {
                float distance = transform.position.y - _detectedPlayer.transform.position.y;
                return distance > 0 ? -1 : 1;
            }
        }
    }

    /// <summary>
    /// Get direction fot shooting purposes.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual int GetShotDirection(EnemyWeaponGameObjectData data)
    {        
        int direction = 1;

        Vector2 weaponWorldPos = data.WeaponSpriteRenderer.transform.position;
        Vector2 weaponPivotPos = data.RotationPivot.transform.position;
        Vector2 referenceWorldPos = data.ReferencePoint.transform.position;

        if (!data.FireInVerticalAxis)
        {
            if(!data.RotateTowardsPlayer || (data.RotateTowardsPlayer && !data.IsBackWeapon))
                direction = referenceWorldPos.x <= weaponWorldPos.x ? -1 : 1;
            else // A back weapon which rotates towards player has inverted direction logic due is rotation values
                direction = referenceWorldPos.x <= weaponWorldPos.x ? 1 : -1;
        }
        else
        {
            direction = referenceWorldPos.y <= weaponPivotPos.y ? 1 : -1;
            //direction *= IsLeft ? -1 : 1; // invert the previous result due the inverted weapon X scale when IsLeft is true
            //print("direction: " + direction);
        }

        //print($"reference position: {referenceWorldPos}, " +
        //    $"weapon position: {weaponWorldPos} + " +
        //    $"Red Axis is: {data.transform.right}, " +
        //    $"direction is {direction}");

        return direction;
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
        LayerMask layerMask = WeaponController.WeaponDataList[FrontalWeaponIndex].WeaponGameObjectData.PlayerDetector.RaycastLayer;

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
        if (!CanFlip)
            return;

        if(direction < 0 && transform.localScale.x > 0 && !IsLeft)
        {
            _isLeft = true;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            WeaponController.FlipWeaponsPlayerDetectors();
            OnEnemyFlip?.Invoke();
        }
        else if(direction > 0 && transform.localScale.x < 0 && IsLeft)
        {
            _isLeft = false;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            WeaponController.FlipWeaponsPlayerDetectors();
            OnEnemyFlip?.Invoke();
        }
    }

    protected virtual void FlipVerticalDirection()
    {
        _isDown = !_isDown;
    }

    /// <summary>
    /// Sets randomically if the enemy flies at horizontal and vertical axis.
    /// </summary>
    protected void SetRandomFlyingMixedMovement()
    {
        if (EnemyType == EnemyType.Flying && FlyingChaseMode == FlyingChaseMode.Mixed)
        {
            _moveFlyingOnHorizontal = SharedFunctions.GetRandomBoolean();

            if (_moveFlyingOnHorizontal)
                _moveFlyingOnVertical = SharedFunctions.GetRandomBoolean();
            else
                _moveFlyingOnVertical = true;

            //print($"Move Horizontal? {_moveFlyingOnHorizontal}. Move on Vertical? {_moveFlyingOnVertical}");
        }
    }

    protected virtual void Death() 
    {
        if (FireEventWhenDead)
            OnEnemyDeath?.Invoke(this);

        Destroy(gameObject);
    }
}
