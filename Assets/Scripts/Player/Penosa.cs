using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using Newtonsoft.Json.Linq;
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

public class Penosa : MonoBehaviour
{
    public Action<byte> OnPlayerLostAllLives;
    public Action<byte> OnPlayerLostAllContinues;
    public Action<byte> OnPlayerRespawn;

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
    [SerializeField] private PlayerData _playerData;
    private Inventory _inventory = null;

    [Header("Movement")]
    public float _speed;
    public float _jumpForce;

    [Header(InputStrings.Jump)]
    [SerializeField] private Transform _groundCheck = null;
    [SerializeField] private Collider2D _wallCheckCollider = null;
    public LayerMask _terrainLayerMask;

    [Header("Parachute")]
    public GameObject _parachute;
    public float _parachuteGravity;

    public float _shotspeed;
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
    private bool _isGrounded;
    private bool _isInCountdown;
    private AnimatorHashes _animHashes = new AnimatorHashes();

    [Header("Blink")]
    public float _blinkDuration;
    private float _blinkTimeCounter;
    private float _blinkIntervalTimeCounter;
    private const float _blinkFrameInterval = 0.05f;
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer _legs;
    [SerializeField] private bool _isBlinking = false;
    #endregion

    #region Props

    public bool HasArmor => PlayerData.ArmorLife > PlayerConsts.DeathLife;

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

    public bool IsBlinking => _isBlinking;

    public Rigidbody2D Rigidbody2D
    {
        get
        {
            if(_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
    }

    public bool IsLeft => _isLeft;

    #endregion

    private void OnEnable()
    {
        InputSystemSetup();
        _inventory = GetComponentInChildren<Inventory>();
        _inventory.OnInventorySpecialItemAdded += HandleOnInventorySpecialItemAdded;
        _inventory.OnInventoryCleared += HandleOnInventoryCleared;

        AutoRotate.OnSpinningProjectileEnabled += HandleOnSpinningProjectileEnabled;
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
        if (_playerController.GameIsPaused() || _isInCountdown)
            return;

        Move();
        Parachute();
        Shoot();

        // TEMPORARY!! Remove this!
        if (Input.GetKeyDown(KeyCode.X)) 
        {
            //TakeDamage(100, true);
            PlayerData.Lives = 0;
            Death();
        }

        if (PlayerData.Life <= PlayerConsts.DeathLife && !Adrenaline && !IsBlinking)
        {
            PlayerData.Lives--;
            Death();
        }
    }

    void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, PlayerConsts.OverlapCircleDiameter, _terrainLayerMask);
        _anim.SetBool(_animHashes.isGrounded, _isGrounded);

        if (_isGrounded && Rigidbody2D.gravityScale != PlayerConsts.DefaultGravity && !JetCopterActivated) 
            ResetGravity();

        if (_isBlinking)
        {
            _blinkIntervalTimeCounter += Time.fixedDeltaTime;
            _blinkTimeCounter += Time.fixedDeltaTime;
            if (_blinkIntervalTimeCounter >= _blinkFrameInterval)
            {
                Blink();
                _blinkIntervalTimeCounter = PlayerConsts.BlinkInitialValue;
            }
        }
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

    public void Blink()
    {
        if (_blinkTimeCounter < _blinkDuration)
        {
            float alpha = _body.color.a;
            float transparency = alpha == 1 ? 0f : 1f;
            _body.color = new Color(255f, 255f, 255f, transparency);
            _legs.color = new Color(255f, 255f, 255f, transparency);
        }
        else
        {
            _isBlinking = false;
            _body.color = new Color(255f, 255f, 255f, 1f);
            _legs.color = new Color(255f, 255f, 255f, 1f);
            _blinkTimeCounter = 0f;
        }
    }

    public void InitiateBlink()
    {
        _isBlinking = true;
    }

    public void Death()
    {
        ResetPlayerData();

        if (PlayerData.Lives == PlayerConsts.GameOverLives)
            OnPlayerLostAllLives?.Invoke(PlayerData.LocalID);
        else
            // Play some death animation...
            InitiateBlink();
    }

    public void ResetPlayerData()
    {
        PlayerData.Life = PlayerConsts.Max_Life;
        PlayerData._1stWeaponLevel = PlayerConsts.WeaponInitialLevel;
        PlayerData._2ndWeaponLevel = PlayerConsts.WeaponInitialLevel;
        PlayerData._1stWeaponAmmoProp = PlayerConsts._1stWeaponInitialAmmo;
        PlayerData._2ndWeaponAmmoProp = PlayerConsts._2ndWeaponInitialAmmo;
        PlayerData.ArmorLife = PlayerConsts.ArmorInitialLife;
    }

    private void ChangeSpecialItem(InputAction.CallbackContext context)
    {
        if (!Inventory.IsEmpty)
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

    private void UseSpecialItem()
    {
        if (Inventory.SelectedSlot != null && Inventory.SelectedSlot.Item != null &&
            !Inventory.SelectedSlot.Item.ItemInUse)
            Inventory.SelectedSlot.Item.Use();
    }

    private void Move()
    {
        Vector2 moveVector = _moveAction.ReadValue<Vector2>();

        float horizontal = GetNormalizedMovementValue(moveVector.x);
        float vertical = GetNormalizedMovementValue(moveVector.y);

        if (horizontal > 0 && _isLeft) Flip();
        else if (horizontal < 0 && !_isLeft) Flip();

        if (!HitWall() && Rigidbody2D != null)
            Rigidbody2D.velocity = new Vector2(_speed * horizontal, Rigidbody2D.velocity.y);

        SetMovementAnimators(vertical);
    }

    private void Jump(InputAction.CallbackContext context)
    {        
        if (_playerController.GameIsPaused())
            return;

        if (!JetCopterActivated)
        {
            if (_isGrounded)
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
    }

    private float GetNormalizedMovementValue(float raw)
    {
        if (raw > 0) return 1f;
        else if (raw < 0) return -1f;

        return 0f;
    }

    private bool HitWall()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.SetLayerMask(_terrainLayerMask);
        _wallCheckCollider.OverlapCollider(contactFilter, results);
        return results[0] != null;
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
        transform.localScale = new Vector2
            (transform.localScale.x * -1, transform.localScale.y);
        Inventory.transform.localScale = new Vector2(_isLeft ? -1f : 1f, 1f);
    }

    private void Parachute()
    {
        bool buttonPressed = _parachuteAction.ReadValue<float>() > 0f;

        if (JetCopterActivated) return;

        if (buttonPressed && !_parachute.activeSelf && !_isGrounded && Rigidbody2D.velocity.y < 0)
        {
            Rigidbody2D.gravityScale = _parachuteGravity;
            _parachute.SetActive(true);
        }
        else if (!buttonPressed) ResetGravity();
    }

    private void ResetGravity()
    {
        Rigidbody2D.gravityScale = PlayerConsts.DefaultGravity;
        if (_parachute.activeSelf)
            _parachute.GetComponent<Animator>().SetTrigger(ConstantStrings.TurnOff);
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

    private int GetShotDirection()
    {
        int direction = 0;
        if (Vertical)
            direction = Input.GetAxisRaw(InputStrings.Vertical) > 0 ? 1 : -1;
        else direction = _isLeft ? -1 : 1;

        return direction;
    }

    private void SetShotLevel2VariationRate(ref GameObject projectile)
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
            var kawarimi = _currentGrenade.GetComponent<Kawarimi>();
            if (kawarimi != null) kawarimi.penosa = gameObject;

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

        // TEMPORARY!!
        //if (fire1ButtonPressed && Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    TakeDamage(30, true);
        //    //PlayerData.Lives = 0;
        //    //Death();
        //}

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
        else if (fire2ButtonPressed && PlayerData._2ndWeaponAmmoProp > 0)
            InstantiateSecondaryShot();
        else if (fire3ButtonPressed) UseSpecialItem();

        if (_isShooting) ResetShootAnim();
    }

    public void SetAmmo(WeaponType weaponType, int ammo)
    {
        if (weaponType == WeaponType.Primary && PlayerData._1stWeaponLevel > 1)
            PlayerData._1stWeaponAmmoProp = ammo;
        else if (weaponType == WeaponType.Secondary)
            PlayerData._2ndWeaponAmmoProp = ammo;
    }

    public void TakeDamage(int dmg, bool force = false) // Remove this default parameter. It's for test usage only.
    {
        if (!IsBlinking || force)
        {
            if (HasArmor) PlayerData.ArmorLife -= dmg;
            else PlayerData.Life -= dmg;
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

    public void SetPlayerOnSceneAfterGameOver(bool val)
    {
        _isInCountdown = !val;

        if(_isInCountdown)
        {
            if (_parachute.activeSelf)
                _parachute.SetActive(false);

            if (JetCopterObject.activeSelf)
            {
                JetCopterObject.SetActive(false);
                JetCopterActivated = false;
                Animator.SetBool(ConstantStrings.JetCopter, false);
            }

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
        _groundCheck.gameObject.SetActive(val);
        _wallCheckCollider.gameObject.SetActive(val);
        Inventory.gameObject.SetActive(val);
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