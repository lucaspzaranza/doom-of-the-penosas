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
    private InputAction jumpAction;

    [SerializeField] private PlayerHUD _HUD = null;
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

    public PlayerHUD HUD => _HUD;
    public bool HasArmor => PlayerData.ArmorLife > 0;

    public GameObject JetCopterObject => _jetCopter;

    private bool Vertical => Mathf.Abs(Input.GetAxisRaw(InputStrings.Vertical)) == 1;

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
        jumpAction = playerInputActions.Player.Jump;
        jumpAction.performed += Jump;
        jumpAction.canceled += Fall;
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = defaultGravity;
        _inventory = GetComponentInChildren<Inventory>();
    }

    void Update()
    {
        Move();
        //Jump();
        Parachute();
        Shoot();
        //ChangeSpecialItem();

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

    private void ChangeSpecialItem()
    {
        if (Input.GetButtonDown(InputStrings.ChangeSpecialItem) && !Inventory.IsEmpty)
        {
            float value = Input.GetAxisRaw(InputStrings.ChangeSpecialItem);
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
        float horizontal = Input.GetAxis(InputStrings.Horizontal);
        float vertical = Input.GetAxis(InputStrings.Vertical);
        if (horizontal > 0 && isLeft) Flip();
        else if (horizontal < 0 && !isLeft) Flip();

        if (!HitWall())
            rb.velocity = new Vector2(speed * horizontal, rb.velocity.y);

        SetMovementAnimators(vertical);
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
        anim.SetBool(animHashes.up, vertical == 1);
        anim.SetBool(animHashes.down, vertical == -1);
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
            rb.velocity = new Vector2(rb.velocity.x, speed * 2);
    }

    private void Fall(InputAction.CallbackContext context)
    {
        float yVelocity = rb.velocity.y;
        float newY = yVelocity < 0 ? yVelocity : 0;

        rb.velocity = new Vector2(rb.velocity.x, newY);
    }

    private void Parachute()
    {
        if (JetCopterActivated) return;

        bool buttonPressed = Gamepad.current.buttonSouth.isPressed || Keyboard.current.spaceKey.isPressed;
        bool buttonRelease = Gamepad.current.buttonSouth.wasReleasedThisFrame || Keyboard.current.spaceKey.wasReleasedThisFrame;

        if (buttonPressed && !parachute.activeSelf && !isGrounded && rb.velocity.y < 0)
        {
            rb.gravityScale = parachuteGravity;
            parachute.SetActive(true);
        }
        else if (buttonRelease) ResetGravity();
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
        if (PlayerData._1stWeaponLevel == 1 && Input.GetButtonDown(InputStrings.Fire1))
            Instantiate_1stShot();
        else if (PlayerData._1stWeaponLevel == 2 && Input.GetButton(InputStrings.Fire1))
            Instantiate_1stShotLv2();
        else if (PlayerData._1stWeaponLevel == 3 && Input.GetButtonDown(InputStrings.Fire1) && !isShooting)
            Instantiate_1stShot();
        else if (Input.GetButtonDown(InputStrings.Fire2) && PlayerData._2ndWeaponAmmo > 0)
            InstantiateSecondaryShot();
        else if (Input.GetButtonDown(InputStrings.Fire3)) UseSpecialItem();

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