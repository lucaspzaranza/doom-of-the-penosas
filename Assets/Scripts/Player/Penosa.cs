using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

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
    public static Action<byte> OnPlayerDeath;

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
    public float speed;
    public float jumpForce;

    [Header(InputStrings.Jump)]
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private Collider2D wallCheckCollider = null;
    public LayerMask terrainLayerMask;
    public float defaultGravity;

    [Header("Parachute")]
    public GameObject parachute;
    public float parachuteGravity;

    public float shotspeed;
    private GameObject currentGrenade;
    [SerializeField] private Transform[] horizontalShotSpawnCoordinates = null;
    [SerializeField] private Transform[] verticalShotSpawnCoordinates = null;
    [SerializeField] private Transform secondaryShotSpawnCoordinates = null;

    [Header("Items")]
    [SerializeField] private GameObject _jetCopter = null;

    private float shotAnimTimeCounter;
    private float continuousTimeCounter;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isShooting;
    private bool isWalking;
    private bool isLeft;
    private bool isGrounded;
    private AnimatorHashes animHashes = new AnimatorHashes();

    [Header("Blink")]
    public float blinkDuration;
    private float blinkTimeCounter;
    private float blinkIntervalTimeCounter;
    private const float blinkFrameInterval = 0.05f;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer legs;
    [SerializeField] private bool isBlinking = false;
    #endregion

    #region Props

    public bool HasArmor => PlayerData.ArmorLife > PlayerConsts.DeathLife;

    public GameObject JetCopterObject => _jetCopter;

    private bool Vertical => Mathf.Abs(_moveAction.ReadValue<Vector2>().y) > PlayerConsts.InputZeroValue;

    public Inventory Inventory => _inventory;

    public bool JetCopterActivated { get; set; }

    public Animator Animator => anim;

    public bool Adrenaline => speed > PlayerConsts.DefaultSpeed;

    public PlayerData PlayerData
    {
        get => _playerData;
        set => _playerData = value;
    }

    public bool IsBlinking => isBlinking;

    #endregion

    private void OnEnable()
    {
        InputSystemSetup();
        _inventory = GetComponentInChildren<Inventory>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = defaultGravity;
    }

    void Update()
    {
        if (_playerController.GameIsPaused())
            return;

        Move();
        Parachute();
        Shoot();

        if (Input.GetKeyDown(KeyCode.X)) // Remove this!   
            TakeDamage(40, true);

        if (PlayerData.Life <= PlayerConsts.DeathLife && !Adrenaline && !IsBlinking)
        {
            PlayerData.Lives--;
            Death();
        }
    }

    void FixedUpdate()
    {
        if (_playerController.GameIsPaused())
            return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, PlayerConsts.OverlapCircleDiameter, terrainLayerMask);
        anim.SetBool(animHashes.isGrounded, isGrounded);

        if (isGrounded && rb.gravityScale != defaultGravity && !JetCopterActivated) 
            ResetGravity();

        if (isBlinking)
        {
            blinkIntervalTimeCounter += Time.fixedDeltaTime;
            blinkTimeCounter += Time.fixedDeltaTime;
            if (blinkIntervalTimeCounter >= blinkFrameInterval)
            {
                Blink();
                blinkIntervalTimeCounter = PlayerConsts.BlinkInitialValue;
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

    public void PauseMenu(InputAction.CallbackContext context)
    {
        if(!_playerController.GameIsPaused())
            _playerController.OnPlayerPause?.Invoke(true);
    }

    public void Blink()
    {
        if (blinkTimeCounter < blinkDuration)
        {
            float alpha = body.color.a;
            float transparency = alpha == 1 ? 0f : 1f;
            body.color = new Color(255f, 255f, 255f, transparency);
            legs.color = new Color(255f, 255f, 255f, transparency);
        }
        else
        {
            isBlinking = false;
            body.color = new Color(255f, 255f, 255f, 1f);
            legs.color = new Color(255f, 255f, 255f, 1f);
            blinkTimeCounter = 0f;
        }
    }

    public void InitiateBlink()
    {
        isBlinking = true;
    }

    public void Death()
    {
        if (PlayerData.Lives == PlayerConsts.GameOverLives)
            OnPlayerDeath(PlayerData.LocalID);
            //GameController.instance.RemovePlayerFromScene(PlayerData.LocalID);
        else
        {
            // Play some death animation...
            ResetPlayerData();
            InitiateBlink();
        }
    }

    public void ResetPlayerData()
    {
        PlayerData.Life = PlayerConsts.Max_Life;
        PlayerData._1stWeaponLevel = PlayerConsts.WeaponInitialLevel;
        PlayerData._2ndWeaponLevel = PlayerConsts.WeaponInitialLevel;
        PlayerData._1stWeaponAmmoProp = PlayerConsts._1stWeaponInitialAmmo;
        PlayerData._2ndWeaponAmmoProp = PlayerConsts._2ndWeaponInitialAmmo;
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

        if (horizontal > 0 && isLeft) Flip();
        else if (horizontal < 0 && !isLeft) Flip();

        if (!HitWall() && rb != null)
            rb.velocity = new Vector2(speed * horizontal, rb.velocity.y);

        SetMovementAnimators(vertical);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_playerController.GameIsPaused())
            return;

        if (!JetCopterActivated)
        {
            if (isGrounded)
                rb.AddForce(Vector2.up * jumpForce);
        }
        else
            rb.velocity = new Vector2(rb.velocity.x, speed);
    }

    private void Fall(InputAction.CallbackContext context)
    {
        float yVelocity = rb.velocity.y;
        float newY = yVelocity < 0 ? yVelocity : 0;

        rb.velocity = new Vector2(rb.velocity.x, newY);
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
        contactFilter.SetLayerMask(terrainLayerMask);
        wallCheckCollider.OverlapCollider(contactFilter, results);
        return results[0] != null;
    }

    private void SetMovementAnimators(float vertical)
    {
        if (rb == null)
            return;

        anim.SetInteger(animHashes.XVelocity, (int)rb.velocity.x);
        anim.SetFloat(animHashes.YVelocity, rb.velocity.y);

        anim.SetBool(animHashes.up, vertical > 0);
        anim.SetBool(animHashes.down, vertical < 0);
    }

    private void Flip()
    {
        isLeft = !isLeft;
        transform.localScale = new Vector2
            (transform.localScale.x * -1, transform.localScale.y);
        Inventory.transform.localScale = new Vector2(isLeft ? -1f : 1f, 1f);
    }

    private void Parachute()
    {
        bool buttonPressed = _parachuteAction.ReadValue<float>() > 0f;

        if (JetCopterActivated) return;

        if (buttonPressed && !parachute.activeSelf && !isGrounded && rb.velocity.y < 0)
        {
            rb.gravityScale = parachuteGravity;
            parachute.SetActive(true);
        }
        else if (!buttonPressed) ResetGravity();
    }

    private void ResetGravity()
    {
        rb.gravityScale = defaultGravity;
        if (parachute.activeSelf)
            parachute.GetComponent<Animator>().SetTrigger(ConstantStrings.TurnOff);
    }

    private Transform GetShotSpawnCoordinates()
    {
        if (Vertical)
        {
            // Se estiver olhando pra baixo, pega os 3 últimos valores do array,
            // onde foram armazenados os Transforms dos frames da galinha mirando pra baixo
            int downShotSpawnPosIndex = Input.GetAxisRaw(InputStrings.Vertical) < 0 ? 3 : 0;
            return verticalShotSpawnCoordinates[PlayerData._1stWeaponLevel + downShotSpawnPosIndex - 1];
        }
        else return horizontalShotSpawnCoordinates[PlayerData._1stWeaponLevel - 1];
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
        else direction = isLeft ? -1 : 1;

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
            isShooting = true;
            anim.SetInteger(animHashes.shotLevel, PlayerData._1stWeaponLevel);
            anim.SetTrigger(animHashes.shootTrigger);
            shotAnimTimeCounter = 0;

            var currentTransform = GetShotSpawnCoordinates();
            var currentRotation = GetShotRotation();
            int currentDirection = GetShotDirection();

            GameObject newBullet = ObjectPool.instance.GetObject(PlayerData.Current1stShot);
            newBullet.transform.position = currentTransform.position;
            newBullet.transform.rotation = currentRotation;
            newBullet.transform.localScale = 
                new Vector2(newBullet.transform.localScale.x * currentDirection, newBullet.transform.localScale.y);

            if (PlayerData._1stWeaponLevel == 2) SetShotLevel2VariationRate(ref newBullet);

            newBullet.GetComponent<Projectile>().Speed = shotspeed * currentDirection;

            SetAmmo(WeaponType.Primary, PlayerData._1stWeaponAmmoProp - 1);
        }
    }

    private void InitializeShot2Animators()
    {
        isShooting = true;
        anim.SetTrigger(animHashes.shootTrigger);
        anim.SetInteger(animHashes.shotLevel, 4);
        shotAnimTimeCounter = 0;
    }

    private GameObject GetProjectileFromPool(GameObject instanceToSearch)
    {
        GameObject result = ObjectPool.instance.GetObject(instanceToSearch);
        result.transform.position = secondaryShotSpawnCoordinates.position;
        result.transform.rotation = Quaternion.identity;

        return result;
    }

    private void InstantiateSecondaryShot()
    {
        bool canSpawn = PlayerData._2ndWeaponLevel == 1;
        // currentGrenade sendo nula significa que não tem nenhuma granada em tela no momento.
        // Essa verificação é feita pelo fato de que só pode atirar um 2nd shot por vez.
        bool fire2Level2Logic = PlayerData._2ndWeaponLevel == 2 && currentGrenade == null;
        canSpawn |= fire2Level2Logic;
        if (canSpawn)
        {
            InitializeShot2Animators();
            // Somente o nível 1 do Tiro 2 terá Pooling, já que o nível 2 só tem uma instância por vez na tela.
            if (PlayerData._2ndWeaponLevel == 1)
                currentGrenade = GetProjectileFromPool(PlayerData.Current2ndShot);
            else if(fire2Level2Logic)
                currentGrenade = Instantiate(PlayerData.Current2ndShot, secondaryShotSpawnCoordinates.position, Quaternion.identity);

            var currentGrenadeScript = currentGrenade.GetComponent<Grenade>();
            currentGrenadeScript.Speed *= isLeft ? -1 : 1;
            currentGrenade.transform.localScale = new Vector2(currentGrenade.transform.localScale.x * (isLeft ? -1 : 1),
                                                                                currentGrenade.transform.localScale.y);
            currentGrenadeScript.CallThrowGrenade();
            var kawarimi = currentGrenade.GetComponent<Kawarimi>();
            if (kawarimi != null) kawarimi.penosa = gameObject;

            // Se a bomba for nivel 2, precisamos guardar a referência dela para futuras verificações
            if(PlayerData._2ndWeaponLevel == 1 && currentGrenade != null) 
                currentGrenade = null;

            SetAmmo(WeaponType.Secondary, PlayerData._2ndWeaponAmmoProp - 1);
        }
    }

    private void Instantiate_1stShotLv2()
    {
        if (continuousTimeCounter >= PlayerConsts.MachineGunInterval)
        {
            Instantiate_1stShot();
            continuousTimeCounter = 0f;
        }
        else continuousTimeCounter += Time.deltaTime;
    }

    private void ResetShootAnim()
    {
        shotAnimTimeCounter += Time.deltaTime;
        float timeToResetAnimation = PlayerData._1stWeaponLevel <= 2 ? PlayerConsts.ShotAnimDuration :
                                    PlayerConsts.ShotLvl3Duration;
        if (shotAnimTimeCounter >= timeToResetAnimation)
        {
            anim.SetInteger(animHashes.shotLevel, 0);
            shotAnimTimeCounter = 0;
            isShooting = false;
        }
    }

    private void Shoot()
    {
        bool fire1ButtonPressed = _fire1Action.ReadValue<float>() > 0;

        // Lvl Diferente de 2, porque com o nível 2, o comportamento do tiro é diferente,
        // precisa verificar se está pressionado a cada frame, enquanto que nos lvls 1 e 3 
        // a função de tiro só deve ser chamada no frame em que o botão foi pressionado.
        if (PlayerData._1stWeaponLevel != 2) fire1ButtonPressed &= _fire1Action.triggered;

        bool fire2ButtonPressed = _fire2Action.ReadValue<float>() > 0 && _fire2Action.triggered;
        bool fire3ButtonPressed = _fire3Action.ReadValue<float>() > 0 && _fire3Action.triggered;

        if (fire1ButtonPressed && PlayerData._1stWeaponLevel == 1)
            Instantiate_1stShot();
        else if (fire1ButtonPressed && PlayerData._1stWeaponLevel == 2 && fire1ButtonPressed)
            Instantiate_1stShotLv2();
        else if (fire1ButtonPressed && !isShooting && PlayerData._1stWeaponLevel == 3)
            Instantiate_1stShot();
        else if (fire2ButtonPressed && PlayerData._2ndWeaponAmmoProp > 0)
            InstantiateSecondaryShot();
        else if (fire3ButtonPressed) UseSpecialItem();

        if (isShooting) ResetShootAnim();
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
}

/*
    DEFAULT VALUES:
    speed: 2
    jumpForce: 26
    defaultGravity: 1.5f
    parachuteGravity: 0.2f
    shotSpeed: 8
*/