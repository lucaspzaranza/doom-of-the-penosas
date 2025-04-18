﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using SharedData.Enumerations;

public class AnimatorHashes
{
    public readonly int shootTrigger = Animator.StringToHash(ConstantStrings.Shoot);
    public readonly int shotLevel = Animator.StringToHash(ConstantStrings.ShotLevel);
    public readonly int XVelocity = Animator.StringToHash(ConstantStrings.XVelocity);
    public readonly int YVelocity = Animator.StringToHash(ConstantStrings.YVelocity);
    public readonly int isGrounded = Animator.StringToHash(ConstantStrings.IsGrounded);
    public readonly int up = Animator.StringToHash(ConstantStrings.Up);
    public readonly int down = Animator.StringToHash(ConstantStrings.Down);
    public readonly int jetCopter = Animator.StringToHash(ConstantStrings.JetCopter);
}

public class Penosa : DamageableObject
{
    public Action<byte> OnPlayerLostAllLives;
    public Action<byte> OnPlayerLostAllContinues;
    public Action<byte> OnPlayerRespawn;
    public Action<byte> OnPlayerDeath;
    public Action<int> OnArmorLifeChanged;

    /// <summary>
    /// The boolean is to send to the event if you are equipping the rideArmor or ejecting it. <br/>
    /// True for equipping, False for ejecting.
    /// </summary>
    public Action<byte, RideArmor, bool> OnPlayerRideArmor;

    #region Vars

    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _pauseAction;
    private InputAction _jumpAction;
    private InputAction _parachuteAction;
    private InputAction _changespecialItemAction;
    private InputAction _fire1Action;
    private InputAction _fire2Action;
    private InputAction _fire3Action;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private LandCharacterProps _landCharacterProps;
    [SerializeField] private PlayerData _playerData;
    private Inventory _inventory = null;

    [Header("Movement")]
    public float _speed;
    public float _jumpForce;

    [Header("Parachute")]
    public GameObject _parachute;
    public float _parachuteGravity;

    [SerializeField] private float _shotspeed;
    private GameObject _currentGrenade;
    [SerializeField] private GameObject _spawnCoordinatesContainer;
    [SerializeField] private Transform[] _horizontalShotSpawnCoordinates = null;
    [SerializeField] private Transform[] _verticalShotSpawnCoordinates = null;
    [SerializeField] private Transform _secondaryShotSpawnCoordinates = null;

    [Header("Items")]
    [SerializeField] private GameObject _jetCopter = null;

    private float _shotAnimTimeCounter;
    private float _continuousTimeCounter;
    private Animator _anim;
    private Rigidbody2D _rb;
    private bool _isShooting;
    private bool _isWalking;
    private bool _isLeft;
    private bool _isInCountdown;
    private bool _canRideArmor;
    private AnimatorHashes _animHashes = new AnimatorHashes();
    private Kawarimi _kawarimi;
    private FallFromPlatform _lastPlatformLanded;
    private FallFromPlatform _platform;

    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer _legs;

    [Header("Ride Armor")]
    [SerializeField] private bool _rideArmorEquipped;
    [SerializeField] private RideArmorActivator _rideArmorActivator;
    [SerializeField] private RideArmor _rideArmor;

    #endregion

    #region Props

    public bool HasArmor => ArmorLife > PlayerConsts.DeathLife;

    [SerializeField][Range(0, PlayerConsts.Max_Life)] private int _armorLife;

    public int ArmorLife
    {
        get => _armorLife;
        set
        {
            _armorLife = Mathf.Clamp(value, 0, PlayerConsts.Max_Life);
            OnArmorLifeChanged?.Invoke(_armorLife);
            if (value < 0) Life -= (Mathf.Abs(value));
        }
    }

    public GameObject JetCopterObject => _jetCopter;

    private bool Vertical => Mathf.Abs(_moveAction.ReadValue<Vector2>().y) > PlayerConsts.InputZeroValue;

    public Inventory Inventory => _inventory;

    public bool JetCopterActivated { get; set; }

    public Animator Animator => _anim;

    public bool Adrenaline => _speed > PlayerConsts.DefaultSpeed;

    public PlayerData PlayerData
    {
        get => _playerData;
        set => _playerData = value;
    }

    public Rigidbody2D Rigidbody2D
    {
        get
        {
            if(_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
    }

    public GameObject CurrentGrenade => _currentGrenade;

    public bool IsLeft => _isLeft;

    public bool RideArmorEquipped => _rideArmorEquipped;

    public RideArmor RideArmorProp => _rideArmor;

    public float ShotSpeed => _shotspeed;

    private UnityEngine.InputSystem.PlayerInput PlayerInput => _playerInput;
    public InputAction MoveAction => _moveAction;
    public InputAction JumpAction => _jumpAction;
    public InputAction PauseAction => _pauseAction;
    public InputAction Fire1Action => _fire1Action;
    public InputAction Fire2Action => _fire2Action;
    public InputAction Fire3Action => _fire3Action;

    #endregion

    private void OnEnable()
    {
        InputSystemSetup();
        _inventory = GetComponentInChildren<Inventory>();
        _inventory.OnInventorySpecialItemAdded += HandleOnInventorySpecialItemAdded;
        _inventory.OnInventoryCleared += HandleOnInventoryCleared;

        AutoRotate.OnSpinningProjectileEnabled += HandleOnSpinningProjectileEnabled;

        _platform = null;
    }

    private void OnDisable()
    {
        _inventory.OnInventorySpecialItemAdded -= HandleOnInventorySpecialItemAdded;
        _inventory.OnInventoryCleared -= HandleOnInventoryCleared;

        AutoRotate.OnSpinningProjectileEnabled -= HandleOnSpinningProjectileEnabled;

        InputSystemReset();
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
        Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
    }

    void Update()
    {
        if (_playerController != null && 
            _playerController.GameIsPaused() || _isInCountdown)
            return;

        Move();
        Parachute();
        Shoot();

        // TEMPORARY!! Remove this!
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(30, true);
        }

        if (Life <= PlayerConsts.DeathLife && !Adrenaline && !IsBlinking)
            PlayerLostALife();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _landCharacterProps.IsGrounded = Physics2D.OverlapCircle(_landCharacterProps.GroundCheck.position, 
            PlayerConsts.OverlapCircleDiameter, _landCharacterProps.TerrainLayerMask);
        _landCharacterProps.IsGrounded &= Rigidbody2D.velocity.y == 0;
        _anim.SetBool(_animHashes.isGrounded, _landCharacterProps.IsGrounded);

        bool insideWater = Physics2D.OverlapCircle(_landCharacterProps.GroundCheck.position, 
            PlayerConsts.OverlapCircleDiameter, _landCharacterProps.WaterLayerMask);
        if(insideWater && !IsBlinking && !RideArmorEquipped)
            PlayerLostALife();

        if (_landCharacterProps.IsGrounded && Rigidbody2D.gravityScale != PlayerConsts.DefaultGravity 
            && !JetCopterActivated && !RideArmorEquipped) 
            ResetGravity();

        _landCharacterProps.IsOnPlatform = SharedFunctions.HitSomething(
            _landCharacterProps.GroundCheckCollider, _landCharacterProps.PlatformOnlyLayerMask, out Collider2D platformCollider);

        if (_landCharacterProps.IsGrounded && RideArmorEquipped && Rigidbody2D.gravityScale > 0)
        {
            _landCharacterProps.GroundCheck.gameObject.SetActive(false);
            Rigidbody2D.gravityScale = 0;
            Rigidbody2D.velocity = Vector2.zero;
        }

        if(_landCharacterProps.IsGrounded && !_landCharacterProps.IsOnPlatform && 
            _platform != null && HasPlatformIgnored(_platform.Collider))
        {
            Physics2D.IgnoreCollision(_landCharacterProps.CharacterCollider, _platform.Collider, false);
            SetPlatform(null);
        }
    }

    private bool HasPlatformIgnored(Collider2D collider)
    {
        return Physics2D.GetIgnoreCollision(_landCharacterProps.CharacterCollider, collider);
    }

    private bool CanFallFromPlatform()
    {
        if (_platform == null)
            return false;

        return !_platform.HasTerrainAbovePlatform(transform.position);
    }

    private void FallFromPlatform()
    {
        Physics2D.IgnoreCollision(_landCharacterProps.CharacterCollider, _platform.Collider);
    }

    public void SetPlatform(FallFromPlatform platformToAssign)
    {
        _lastPlatformLanded = _platform;
        if(_lastPlatformLanded != null)
            Physics2D.IgnoreCollision(_landCharacterProps.CharacterCollider, _lastPlatformLanded.Collider, false);

        _platform = platformToAssign;
    }

    public void PlayerLostALife()
    {
        PlayerData.Lives--;
        ResetGravity();
        Death();        
    }

    private void InputSystemSetup()
    {
        _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _fire1Action = _playerInput.actions.FindAction(PlayerConsts.Fire1Action);
        _fire2Action = _playerInput.actions.FindAction(PlayerConsts.Fire2Action);
        _fire3Action = _playerInput.actions.FindAction(PlayerConsts.Fire3SpecialAction);
        _moveAction = _playerInput.actions.FindAction(PlayerConsts.MoveAction);
        _parachuteAction = _playerInput.actions.FindAction(PlayerConsts.ParachuteAction);
        _changespecialItemAction = _playerInput.actions.FindAction(PlayerConsts.ChangeSpecialItemAction);
        _changespecialItemAction.performed += ChangeSpecialItem;

        _pauseAction = _playerInput.actions.FindAction(PlayerConsts.PauseMenu);
        _pauseAction.performed += PauseMenu;
        
        _jumpAction = _playerInput.actions.FindAction(PlayerConsts.JumpAction);
        _jumpAction.performed += Jump;
        _jumpAction.canceled += Fall;
    }

    private void InputSystemReset()
    {
        _changespecialItemAction.performed -= ChangeSpecialItem;

        _pauseAction.performed -= PauseMenu;

        _jumpAction.performed -= Jump;
        _jumpAction.canceled -= Fall;
    }

    private void HandleOnSpinningProjectileEnabled(AutoRotate projectile)
    {
        if(_playerData.Character == Penosas.Dolores && _isLeft)
            projectile.SetRotationDirection(-1);
    }

    private void HandleOnInventorySpecialItemAdded(InventoryListItem inventoryListItem)
    {
        _playerData.InventoryData.UpdateData(inventoryListItem);
    }

    private void HandleOnInventoryCleared()
    {
        _playerData.InventoryData.ClearInventoryData();
    }

    public void PauseMenu(InputAction.CallbackContext context)
    {
        if(_isInCountdown)
            OnPlayerRespawn?.Invoke(PlayerData.LocalID);
        else if (!_playerController.GameIsPaused())
            _playerController.OnPlayerPause?.Invoke(true);
    }
    
    public void Death()
    {
        if (JetCopterObject.activeSelf)
        {
            var jetCopterScript = Inventory.SelectedSlot.Item.GetComponent<JetCopter>();
            jetCopterScript.SetJetCopterActivation(false);
            jetCopterScript.RemoveItemIfAmountEqualsZero();
        }

        ResetPlayerData();

        if (PlayerData.Lives == PlayerConsts.GameOverLives)
            OnPlayerLostAllLives?.Invoke(PlayerData.LocalID);
        else
            // Play some death animation...
            InitiateBlink();

        OnPlayerDeath?.Invoke(PlayerData.LocalID);
    }

    public void ResetPlayerData()
    {
        Life = PlayerConsts.Max_Life;
        PlayerData._1stWeaponLevel = PlayerConsts.WeaponInitialLevel;
        PlayerData._2ndWeaponLevel = PlayerConsts.WeaponInitialLevel;
        PlayerData._1stWeaponAmmoProp = PlayerConsts._1stWeaponInitialAmmo;
        PlayerData._2ndWeaponAmmoProp = PlayerConsts._2ndWeaponInitialAmmo;
        PlayerData.Player.ArmorLife = PlayerConsts.ArmorInitialLife;
    }

    private void ChangeSpecialItem(InputAction.CallbackContext context)
    {
        if (!Inventory.IsEmpty && !RideArmorEquipped)
        {
            float value = _changespecialItemAction.ReadValue<float>();
            int length = Inventory.Slots.Count;
            int index = Inventory.Slots.IndexOf(Inventory.SelectedSlot);
            if (value > 0 && index < length - 1) index++;
            else if (value < 0 && index > 0) index--;
            Inventory.SelectItem(index);
            Inventory.ShowSlot();
        }
    }

    private void UseSpecialItemOrRideArmor()
    {
        if (!_canRideArmor && !RideArmorEquipped && Inventory.SelectedSlot != null && 
            Inventory.SelectedSlot.Item != null && !Inventory.SelectedSlot.Item.ItemInUse)
            Inventory.SelectedSlot.Item.Use();
        else if (_canRideArmor)
            RideArmor(_rideArmorActivator.RideArmor);
        else if (RideArmorEquipped)
            EjectRideArmor();
    }

    private void DeactivateParachuteIfActive()
    {
        if (_parachute.activeSelf)
            _parachute.GetComponent<Animator>().SetTrigger(ConstantStrings.TurnOff);
    }

    public void RideArmor(RideArmor rideArmorToEquip)
    {
        if (rideArmorToEquip.Type == RideArmorType.Chickencopter && 
            ((Chickencopter)rideArmorToEquip).ChickencopterAbandoned)
            return;

        DeactivateParachuteIfActive();

        Rigidbody2D.gravityScale = 0f;
        if (!_landCharacterProps.IsGrounded && _parachute.activeSelf && 
            rideArmorToEquip.Type != RideArmorType.Chickencopter)
            Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;

        _rideArmor = rideArmorToEquip;
        _rideArmor.Equip(this, _playerController);

        if ((IsLeft && _rideArmor.transform.localScale.x > 0) || 
        (!IsLeft && _rideArmor.transform.localScale.x < 0))
        {
            _rideArmor.transform.localScale = new Vector2(_rideArmor.transform.localScale.x * -1,
                _rideArmor.transform.localScale.y);
            _rideArmor.SetDirection(_isLeft ? -1 : 1);
        }
        Rigidbody2D.velocity = Vector2.zero;
        transform.SetParent(_rideArmor.transform, true);
        transform.localPosition = _rideArmor.PlayerPosition.localPosition;

        _rideArmorEquipped = true;
        _body.enabled = false;
        _legs.enabled = false;
        _canRideArmor = false;
        SetCollidersActivation(false, false);

        if (CurrentGrenade != null)
            Destroy(CurrentGrenade);

        OnPlayerRideArmor?.Invoke(PlayerData.LocalID, rideArmorToEquip, true);
    }

    private void SetCollidersActivation(bool value, bool changeGroundCheck = true)
    {
        _landCharacterProps.WallCheckCollider.enabled = value;
        gameObject.GetComponent<BoxCollider2D>().enabled = value;

        if(changeGroundCheck)
            _landCharacterProps.GroundCheck.gameObject.SetActive(value);
    }

    public void EjectRideArmor()
    {
        // Can eject only ride armors with optional use and with some life
        if (_rideArmor.Required && _rideArmor.Life > 0) 
            return;

        SetCollidersActivation(true);
        Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
        _rideArmor.Eject();
        transform.parent = null;
        _body.enabled = true;
        _legs.enabled = true;
        OnPlayerRideArmor?.Invoke(PlayerData.LocalID, _rideArmor, false);
        _rideArmor = null;
        _rideArmorActivator = null;
        _rideArmorEquipped = false;
    }

    private void Move()
    {
        Vector2 moveVector = _moveAction.ReadValue<Vector2>();

        float horizontal = GetNormalizedMovementValue(moveVector.x);
        float vertical = GetNormalizedMovementValue(moveVector.y);

        if (horizontal > 0 && _isLeft) Flip();
        else if (horizontal < 0 && !_isLeft) Flip();

        Vector2 direction = new Vector2(horizontal * _speed, Rigidbody2D.velocity.y);

        if (!RideArmorEquipped && Rigidbody2D != null &&
        !SharedFunctions.HitSomething(_landCharacterProps.WallCheckCollider, 
            _landCharacterProps.TerrainWithoutPlatformLayerMask, out Collider2D hitWall))
            Rigidbody2D.velocity = direction;
        else if(RideArmorEquipped)
        {
            if (_rideArmor.Type == RideArmorType.Chickencopter)
                direction = new Vector2(direction.x, vertical * _speed);

            _rideArmor.Move(direction);
            _rideArmor.Aim(vertical);
        }

        if(_landCharacterProps.IsOnPlatform)
        {
            bool canFall = CanFallFromPlatform();

            if (_landCharacterProps.IsOnPlatform && !RideArmorEquipped && vertical < 0 &&
                _landCharacterProps.CharacterCollider.enabled && Rigidbody2D.velocity.y == 0f &&
                canFall)
                FallFromPlatform();
        }

        SetMovementAnimators(vertical);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_playerController.GameIsPaused())
            return;

        if (RideArmorEquipped)
        {
            _rideArmor.Jump();
            return;
        }

        if (!JetCopterActivated)
        {
            if (_landCharacterProps.IsGrounded)
                Rigidbody2D.AddForce(Vector2.up * _jumpForce);
        }
        else
            Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, _speed);
    }

    private void Fall(InputAction.CallbackContext context)
    {
        float yVelocity = Rigidbody2D.velocity.y;
        float newY = yVelocity < 0 ? yVelocity : 0;

        Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, newY);

        if (RideArmorEquipped && _rideArmor.Type == RideArmorType.TireMonoWheel)
            _rideArmor.GetComponent<TireMonoWheel>().DeactivateBounciness();
    }

    public float GetNormalizedMovementValue(float raw)
    {
        if (raw > 0) return 1f;
        else if (raw < 0) return -1f;

        return 0f;
    }

    private void SetMovementAnimators(float vertical)
    {
        if (_rb == null)
            return;

        _anim.SetInteger(_animHashes.XVelocity, (int)Rigidbody2D.velocity.x);
        _anim.SetFloat(_animHashes.YVelocity, Rigidbody2D.velocity.y);

        _anim.SetBool(_animHashes.up, vertical > 0);
        _anim.SetBool(_animHashes.down, vertical < 0);
    }

    public void Flip()
    {
        _isLeft = !_isLeft;
        if(!RideArmorEquipped)
        {
            transform.localScale = new Vector2
                (transform.localScale.x * -1, transform.localScale.y);
        }
        else
        {
            _rideArmor.SetDirection(_isLeft ? -1 : 1);
            _rideArmor.transform.localScale = new Vector2
                (_rideArmor.transform.localScale.x * -1, _rideArmor.transform.localScale.y);
        }

        Inventory.transform.localScale = new Vector2(_isLeft ? -1f : 1f, 1f);
    }

    private void Parachute()
    {
        bool buttonPressed = _parachuteAction.ReadValue<float>() > 0f;

        if (JetCopterActivated || RideArmorEquipped) return;

        if (buttonPressed && !_parachute.activeSelf && !_landCharacterProps.IsGrounded && Rigidbody2D.velocity.y < 0)
        {
            Rigidbody2D.gravityScale = _parachuteGravity;
            _parachute.SetActive(true);
        }
        else if (!buttonPressed) ResetGravity();
    }

    private void ResetGravity()
    {
        Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
        DeactivateParachuteIfActive();
    }

    private Transform GetShotSpawnCoordinates()
    {
        if (Vertical)
        {
            // Se estiver olhando pra baixo, pega os 3 últimos valores do array,
            // onde foram armazenados os Transforms dos frames da galinha mirando pra baixo
            int downShotSpawnPosIndex = Input.GetAxisRaw(InputStrings.Vertical) < 0 ? 3 : 0;
            return _verticalShotSpawnCoordinates[PlayerData._1stWeaponLevel + downShotSpawnPosIndex - 1];
        }
        else return _horizontalShotSpawnCoordinates[PlayerData._1stWeaponLevel - 1];
    }

    private Quaternion GetShotRotation()
    {
        return Vertical ? Quaternion.AngleAxis(90f, Vector3.forward) : Quaternion.identity;
    }

    public int GetShotDirection()
    {
        int direction = 0;
        if (Vertical)
            direction = Input.GetAxisRaw(InputStrings.Vertical) > 0 ? 1 : -1;
        else direction = _isLeft ? -1 : 1;

        return direction;
    }

    public void SetShotLevel2VariationRate(ref GameObject projectile)
    {
        float posVariationRate = UnityEngine.Random.Range(PlayerConsts.ShotLvl2VariationRate,
                                            -PlayerConsts.ShotLvl2VariationRate);
        if (Vertical)
            projectile.transform.position = new Vector3
            (projectile.transform.position.x + posVariationRate, projectile.transform.position.y);
        else
            projectile.transform.position = new Vector3
            (projectile.transform.position.x, projectile.transform.position.y + posVariationRate);
    }

    private void Instantiate_1stShot()
    {
        if (PlayerData._1stWeaponLevel == 1 || (PlayerData._1stWeaponLevel > 1 && PlayerData._1stWeaponAmmoProp > 0))
        {
            _isShooting = true;
            _anim.SetInteger(_animHashes.shotLevel, PlayerData._1stWeaponLevel);
            _anim.SetTrigger(_animHashes.shootTrigger);
            _shotAnimTimeCounter = 0;

            var currentTransform = GetShotSpawnCoordinates();
            var currentRotation = GetShotRotation();
            int currentDirection = GetShotDirection();

            GameObject newBullet = _playerController.RequestProjectileFromGameController(PlayerData.Current1stShot);
            newBullet.transform.position = currentTransform.position;
            newBullet.transform.rotation = currentRotation;
            newBullet.transform.localScale = 
                new Vector2(newBullet.transform.localScale.x * currentDirection, newBullet.transform.localScale.y);

            if (PlayerData._1stWeaponLevel == 2) SetShotLevel2VariationRate(ref newBullet);

            newBullet.GetComponent<Projectile>().Speed = _shotspeed * currentDirection;

            SetAmmo(WeaponType.Primary, PlayerData._1stWeaponAmmoProp - 1);
        }
    }

    private void InitializeShot2Animators()
    {
        _isShooting = true;
        _anim.SetTrigger(_animHashes.shootTrigger);
        _anim.SetInteger(_animHashes.shotLevel, 4);
        _shotAnimTimeCounter = 0;
    }

    private GameObject GetProjectileFromPool(GameObject instanceToSearch)
    {
        GameObject result = _playerController.RequestProjectileFromGameController(instanceToSearch);
        result.transform.position = _secondaryShotSpawnCoordinates.position;
        result.transform.rotation = Quaternion.identity;

        return result;
    }

    private void InstantiateSecondaryShot()
    {
        bool canSpawn = PlayerData._2ndWeaponLevel == 1;
        // currentGrenade sendo nula significa que não tem nenhuma granada em tela no momento.
        // Essa verificação é feita pelo fato de que só pode atirar um 2nd shot por vez.
        bool fire2Level2Logic = PlayerData._2ndWeaponLevel == 2 && _currentGrenade == null;
        canSpawn |= fire2Level2Logic;
        if (canSpawn)
        {
            InitializeShot2Animators();
            // Somente o nível 1 do Tiro 2 terá Pooling, já que o nível 2 só tem uma instância por vez na tela.
            if (PlayerData._2ndWeaponLevel == 1)
                _currentGrenade = GetProjectileFromPool(PlayerData.Current2ndShot);
            else if(fire2Level2Logic)
                _currentGrenade = Instantiate(PlayerData.Current2ndShot, _secondaryShotSpawnCoordinates.position, Quaternion.identity);

            var currentGrenadeScript = _currentGrenade.GetComponent<Grenade>();
            currentGrenadeScript.Speed *= _isLeft ? -1 : 1;
            _currentGrenade.transform.localScale = new Vector2(_currentGrenade.transform.localScale.x * (_isLeft ? -1 : 1),
                                                                                _currentGrenade.transform.localScale.y);
            currentGrenadeScript.CallThrowGrenade();
            if (_currentGrenade.TryGetComponent(out _kawarimi))
                _kawarimi.Penosa = gameObject;

            // Se a bomba for nivel 2, precisamos guardar a referência dela para futuras verificações
            if(PlayerData._2ndWeaponLevel == 1 && _currentGrenade != null) 
                _currentGrenade = null;

            SetAmmo(WeaponType.Secondary, PlayerData._2ndWeaponAmmoProp - 1);
        }
    }

    private void Instantiate_1stShotLv2()
    {
        if (_continuousTimeCounter >= PlayerConsts.MachineGunInterval)
        {
            Instantiate_1stShot();
            _continuousTimeCounter = 0f;
        }
        else _continuousTimeCounter += Time.deltaTime;
    }

    private void ResetShootAnim()
    {
        _shotAnimTimeCounter += Time.deltaTime;
        float timeToResetAnimation = PlayerData._1stWeaponLevel <= 2 ? PlayerConsts.ShotAnimDuration :
                                    PlayerConsts.ShotLvl3Duration;
        if (_shotAnimTimeCounter >= timeToResetAnimation)
        {
            _anim.SetInteger(_animHashes.shotLevel, 0);
            _shotAnimTimeCounter = 0;
            _isShooting = false;
        }
    }

    private void Shoot()
    {
        bool fire1ButtonPressed = _fire1Action.ReadValue<float>() > 0;

        if (RideArmorEquipped && fire1ButtonPressed)
        {
            _rideArmor.Shoot();
            return;
        }

        // Lvl Diferente de 2, porque com o nível 2 o comportamento do tiro é diferente,
        // precisa verificar se está pressionado a cada frame, enquanto que nos lvls 1 e 3 
        // a função de tiro só deve ser chamada no frame em que o botão foi pressionado.
        if (PlayerData._1stWeaponLevel != 2) fire1ButtonPressed &= _fire1Action.triggered;

        bool fire2ButtonPressed = _fire2Action.ReadValue<float>() > 0 && _fire2Action.triggered;
        bool fire3ButtonPressed = _fire3Action.ReadValue<float>() > 0 && _fire3Action.triggered;

        if (fire1ButtonPressed && PlayerData._1stWeaponLevel == 1)
            Instantiate_1stShot();
        else if (fire1ButtonPressed && PlayerData._1stWeaponLevel == 2 && fire1ButtonPressed)
            Instantiate_1stShotLv2();
        else if (fire1ButtonPressed && !_isShooting && PlayerData._1stWeaponLevel == 3)
            Instantiate_1stShot();
        else if (fire2ButtonPressed && PlayerData._2ndWeaponAmmoProp > 0 && !RideArmorEquipped)
            InstantiateSecondaryShot();
        else if (fire3ButtonPressed) UseSpecialItemOrRideArmor();

        if (_isShooting) ResetShootAnim();
    }

    public void SetAmmo(WeaponType weaponType, int ammo)
    {
        if (weaponType == WeaponType.Primary && PlayerData._1stWeaponLevel > 1)
            PlayerData._1stWeaponAmmoProp = ammo;
        else if (weaponType == WeaponType.Secondary)
            PlayerData._2ndWeaponAmmoProp = ammo;
    }

    protected override void SetLife(int value)
    {
        _life = Mathf.Clamp(value, 0, PlayerConsts.Max_Life);
        _playerData.OnLifeChanged?.Invoke(_life);

        if (_life == 0 && !Adrenaline && !IsBlinking)
        {
            _playerData.Lives--;
            Death();
        }
    }

    public override void TakeDamage(int dmg, bool force = false) // Remove this default parameter. It's for test usage only.
    {
        if (!IsBlinking || force)
        {
            if(_kawarimi != null)
            {
                _kawarimi.KawarimiNoJutsu();
                _kawarimi = null;
                return;
            }

            if (HasArmor) ArmorLife -= dmg;
            else if(RideArmorEquipped) RideArmorProp.Life -= dmg;
            else Life -= dmg;

            StartGlow();
        }
    }

    public void SetPlayerData(PlayerData newPlayerData)
    {
        _playerData = newPlayerData;
    }

    public void SetPlayerController(PlayerController controller)
    {
        _playerController = controller;
    }

    private void DeactivateJetCopter()
    {
        JetCopterObject.SetActive(false);
        JetCopterActivated = false;
        Animator.SetBool(ConstantStrings.JetCopter, false);
    }

    public void SetPlayerOnSceneAfterGameOver(bool val)
    {
        _isInCountdown = !val;

        if(_isInCountdown)
        {
            if (_parachute.activeSelf)
                _parachute.SetActive(false);

            if (JetCopterObject.activeSelf)
                DeactivateJetCopter();

            if (Adrenaline)
                _speed = PlayerConsts.DefaultSpeed;

            if(Inventory.SlotGameObject.activeSelf)
            {
                Inventory.ResetTemporizerCounter();
                Inventory.SlotGameObject.SetActive(false);
            }
        }

        _body.gameObject.SetActive(val);
        _legs.gameObject.SetActive(val);
        _landCharacterProps.GroundCheck.gameObject.SetActive(val);
        _landCharacterProps.WallCheckCollider.gameObject.SetActive(val);
        Inventory.gameObject.SetActive(val);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == ConstantStrings.RideArmorCoreTag && !JetCopterActivated &&
            !_canRideArmor && !RideArmorEquipped)
        {
            _rideArmorActivator = other.GetComponent<RideArmorActivator>();

            if(!_parachute.activeSelf)
                _canRideArmor = true;

            if(_rideArmorActivator.RideArmor.Type == RideArmorType.JetSkinha)
                RideArmor(_rideArmorActivator.RideArmor);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == ConstantStrings.RideArmorCoreTag)
        {
            _canRideArmor = false;
            _rideArmorActivator = null;
        }
    }
}

/*
    DEFAULT VALUES:
    speed: 2
    jumpForce: 26
    defaultGravity: 1.5f
    parachuteGravity: 0.2f
    shotSpeed: 8
*/