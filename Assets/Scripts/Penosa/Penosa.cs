using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AnimatorHashes
{
    public readonly int shootTrigger = Animator.StringToHash("Shoot");
    public readonly int shotLevel = Animator.StringToHash("Shot Level");
    public readonly int XVelocity = Animator.StringToHash("XVelocity");
    public readonly int YVelocity = Animator.StringToHash("YVelocity");
    public readonly int isGrounded = Animator.StringToHash("IsGrounded");
    public readonly int up = Animator.StringToHash("Up");
    public readonly int down = Animator.StringToHash("Down");
    public readonly int jetCopter = Animator.StringToHash("JetCopter");
}

public class Penosa : MonoBehaviour
{
    #region Vars

    private PlayerInput playerInputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction parachuteAction;
    private InputAction changespecialItemAction;
    private InputAction fire1Action;
    private InputAction fire2Action;
    private InputAction fire3Action;

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

    public bool HasArmor => PlayerData.ArmorLife > 0;

    public GameObject JetCopterObject => _jetCopter;

    private bool Vertical => Mathf.Abs(moveAction.ReadValue<Vector2>().y) > 0;

    public Inventory Inventory => _inventory;

    public bool JetCopterActivated { get; set; }

    public Animator Animator => anim;

    public bool Adrenaline => speed > PlayerConsts.defaultSpeed;

    public PlayerData PlayerData
    {
        get => _playerData;
        set => _playerData = value;
    }

    public bool IsBlinking => isBlinking;

    #endregion

    void Awake()
    {
        //ResetPlayerData();
        playerInputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        InputSystemSetup();

        _inventory = GetComponentInChildren<Inventory>();
    }

    private void OnDisable() 
    {
        DisableInputSystem();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = defaultGravity;
    }

    void Update()
    {
        Move();
        Parachute();
        Shoot();

        if (Input.GetKeyDown(KeyCode.Return)) // Remove this!   
            TakeDamage(40, true);

        if (PlayerData.Life <= 0 && !Adrenaline && !IsBlinking)
        {
            PlayerData.Lives--;
            Death();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, terrainLayerMask);
        anim.SetBool(animHashes.isGrounded, isGrounded);

        if (isGrounded && rb.gravityScale != defaultGravity && !JetCopterActivated) ResetGravity();

        if (isBlinking)
        {
            blinkIntervalTimeCounter += Time.fixedDeltaTime;
            blinkTimeCounter += Time.fixedDeltaTime;
            if (blinkIntervalTimeCounter >= blinkFrameInterval)
            {
                Blink();
                blinkIntervalTimeCounter = 0f;
            }
        }
    }

    private void InputSystemSetup()
    {
        moveAction = playerInputActions.Player.MoveAim;
        moveAction.Enable();

        jumpAction = playerInputActions.Player.Jump;
        jumpAction.performed += Jump;
        jumpAction.canceled += Fall;
        jumpAction.Enable();

        parachuteAction = playerInputActions.Player.Parachute;
        parachuteAction.Enable();

        changespecialItemAction = playerInputActions.Player.ChangeSpecialItem;
        changespecialItemAction.performed += ChangeSpecialItem;
        changespecialItemAction.Enable();

        fire1Action = playerInputActions.Player.Fire1Shot;
        fire1Action.Enable();

        fire2Action = playerInputActions.Player.Fire2Shot;
        fire2Action.Enable();

        fire3Action = playerInputActions.Player.Fire3SpecialItem;
        fire3Action.Enable();
    }

    private void DisableInputSystem()
    {
        moveAction.Disable();

        jumpAction.performed -= Jump;
        jumpAction.canceled -= Fall;
        jumpAction.Disable();

        parachuteAction.Disable();

        changespecialItemAction.performed -= ChangeSpecialItem;
        changespecialItemAction.Disable();

        playerInputActions.Player.TimeBombMovement.Disable();

        fire1Action.Disable();
        fire2Action.Disable();
        fire3Action.Disable();
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
        if (PlayerData.Lives == 0)
            GameController.instance.RemovePlayerFromScene(PlayerData.ID);
        else
        {
            // Play some death animation...
            ResetPlayerData();
            InitiateBlink();
        }
    }

    public void ResetPlayerData()
    {
        PlayerData.Life = PlayerConsts.max_life;
        PlayerData._1stWeaponLevel = 1;
        PlayerData._2ndWeaponLevel = 1;
        PlayerData._1stWeaponAmmo = 0;
        PlayerData._2ndWeaponAmmo = PlayerConsts._2ndWeaponInitialAmmo;
    }

    private void ChangeSpecialItem(InputAction.CallbackContext context)
    {
        if (!Inventory.IsEmpty)
        {
            float value = changespecialItemAction.ReadValue<float>();
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
        Vector2 moveVector = moveAction.ReadValue<Vector2>();

        float horizontal = GetNormalizedMovementValue(moveVector.x);
        float vertical = GetNormalizedMovementValue(moveVector.y);

        if (horizontal > 0 && isLeft) Flip();
        else if (horizontal < 0 && !isLeft) Flip();

        if (!HitWall())
            rb.velocity = new Vector2(speed * horizontal, rb.velocity.y);

        SetMovementAnimators(vertical);
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

    private void Jump(InputAction.CallbackContext context) 
    {
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

    private void Parachute()
    {
        bool buttonPressed = parachuteAction.ReadValue<float>() > 0f;

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
            parachute.GetComponent<Animator>().SetTrigger("TurnOff");
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
        float posVariationRate = Random.Range(PlayerConsts.shotLvl2VariationRate,
                                            -PlayerConsts.shotLvl2VariationRate);
        if (Vertical)
            projectile.transform.position = new Vector3
            (projectile.transform.position.x + posVariationRate, projectile.transform.position.y);
        else
            projectile.transform.position = new Vector3
            (projectile.transform.position.x, projectile.transform.position.y + posVariationRate);
    }

    private void Instantiate_1stShot()
    {
        if (PlayerData._1stWeaponLevel == 1 || (PlayerData._1stWeaponLevel > 1 && PlayerData._1stWeaponAmmo > 0))
        {
            isShooting = true;
            anim.SetInteger(animHashes.shotLevel, PlayerData._1stWeaponLevel);
            anim.SetTrigger(animHashes.shootTrigger);
            shotAnimTimeCounter = 0;

            var currentTransform = GetShotSpawnCoordinates();
            var currentRotation = GetShotRotation();
            var newBullet = Instantiate(PlayerData.Current1stShot, currentTransform.position, currentRotation);
            if (PlayerData._1stWeaponLevel == 2) SetShotLevel2VariationRate(ref newBullet);

            newBullet.GetComponent<Projectile>().speed = shotspeed * GetShotDirection();

            SetAmmo(WeaponType._1st, PlayerData._1stWeaponAmmo - 1);
        }
    }

    private void InstantiateSecondaryShot()
    {
        bool canSpawn = PlayerData._2ndWeaponLevel == 1;
        // currentGrenade sendo nula significa que não tem nenhuma granada em tela no momento.
        // Essa verificação é feita pelo fato de que só pode atirar um 2nd shot por vez.
        canSpawn |= PlayerData._2ndWeaponLevel == 2 && currentGrenade == null;
        if (canSpawn)
        {
            isShooting = true;
            anim.SetTrigger(animHashes.shootTrigger);
            anim.SetInteger(animHashes.shotLevel, 4);
            shotAnimTimeCounter = 0;

            currentGrenade = Instantiate(PlayerData.Current2ndShot,
                secondaryShotSpawnCoordinates.position, Quaternion.identity);
            currentGrenade.GetComponent<Projectile>().speed *= isLeft ? -1 : 1;
            var kawarimi = currentGrenade.GetComponent<Kawarimi>();
            if (kawarimi != null) kawarimi.penosa = gameObject;

            SetAmmo(WeaponType.Secondary, PlayerData._2ndWeaponAmmo - 1);
        }
    }

    private void Instantiate_1stShotLv2()
    {
        if (continuousTimeCounter >= PlayerConsts.machineGunInterval)
        {
            Instantiate_1stShot();
            continuousTimeCounter = 0f;
        }
        else continuousTimeCounter += Time.deltaTime;
    }

    private void ResetShootAnim()
    {
        shotAnimTimeCounter += Time.deltaTime;
        float timeToResetAnimation = PlayerData._1stWeaponLevel <= 2 ? PlayerConsts.shotAnimDuration :
                                    PlayerConsts.shotLvl3Duration;
        if (shotAnimTimeCounter >= timeToResetAnimation)
        {
            anim.SetInteger(animHashes.shotLevel, 0);
            shotAnimTimeCounter = 0;
            isShooting = false;
        }
    }

    private void Shoot()
    {
        bool fire1ButtonPressed = fire1Action.ReadValue<float>() > 0;
        // Lvl Diferente de 2, porque com o nível 2, o comportamento do tiro é diferente,
        // precisa verificar se está pressionado a cada frame, enquanto que nos lvls 1 e 3 
        // a função de tiro só deve ser chamada no frame em que o botão foi pressionado.
        if (PlayerData._1stWeaponLevel != 2) fire1ButtonPressed &= fire1Action.triggered;

        bool fire2ButtonPressed = fire2Action.ReadValue<float>() > 0 && fire2Action.triggered;
        bool fire3ButtonPressed = fire3Action.ReadValue<float>() > 0 && fire3Action.triggered;

        if (fire1ButtonPressed && PlayerData._1stWeaponLevel == 1)
            Instantiate_1stShot();
        else if (fire1ButtonPressed && PlayerData._1stWeaponLevel == 2 && fire1ButtonPressed)
            Instantiate_1stShotLv2();
        else if (fire1ButtonPressed && !isShooting && PlayerData._1stWeaponLevel == 3)
            Instantiate_1stShot();
        else if (fire2ButtonPressed && PlayerData._2ndWeaponAmmo > 0)
            InstantiateSecondaryShot();
        else if (fire3ButtonPressed) UseSpecialItem();

        if (isShooting) ResetShootAnim();
    }

    public void SetAmmo(WeaponType weaponType, int ammo)
    {
        if (weaponType == WeaponType._1st && PlayerData._1stWeaponLevel > 1)
            PlayerData._1stWeaponAmmo = ammo;
        else if (weaponType == WeaponType.Secondary)
            PlayerData._2ndWeaponAmmo = ammo;
    }

    public void TakeDamage(int dmg, bool force = false) // Remove this default parameter. It's for test usage only.
    {
        if (!IsBlinking || force)
        {
            if (HasArmor) PlayerData.ArmorLife -= dmg;
            else PlayerData.Life -= dmg;
        }
    }
}

/*
    DEFAULT VALUES:
    speed: 2
    jumpForce: 26
    defaultGravity: 150
    parachuteGravity: 0.25f
    shotSpeed: 8
*/