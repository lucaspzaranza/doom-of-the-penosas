using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class AnimatorHashes 
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
    #region Consts

    public const float machineGunInterval = 0.1f; 
    public const float shotLvl3Duration = 0.5f;
    public const float shotLvl2VariationRate = 0.1f;
    public const float shotAnimDuration = 0.25f;
    public const byte max_life = 100;
    public const byte max_lives = 9;
    public const byte primaryWeaponMaxLevel = 3;
    public const byte secondaryWeaponMaxLevel = 2;
    public const byte fstWeaponMaxLvl = 3;
    public const byte scndWeaponMaxLvl = 2;
    public const int maxAmmo = 999;

    #endregion

    #region Vars
    [Header("Player")]
    public string penosaName;   
    [SerializeField] [Range(0, max_life)] private int _life;
    [SerializeField] [Range(0, max_life)] private int _armorLife;
    [SerializeField] [Range(0, max_lives)] private int _lives; 
    [SerializeField] [Range(1, fstWeaponMaxLvl)] private byte primaryWeaponLvl;
    [SerializeField] [Range(1, scndWeaponMaxLvl)] private byte secondaryWeaponLvl;
    private Inventory _inventory = null;

    [Header("Movement")]
    public float speed;
    public float jumpForce;

    [Header("Jump")]
    [SerializeField] private Transform groundCheck = null; 
    [SerializeField] private Collider2D wallCheckCollider = null;
    public LayerMask terrainLayerMask;
    public float defaultGravity;

    [Header("Parachute")]
    public GameObject parachute;
    public float parachuteGravity;

    [Header("Shot")]
    public GameObject[] primaryShot;
    public GameObject[] secondaryShot;
    public float shotspeed;
    private GameObject currentGrenade;
    [SerializeField] private Transform[] horizontalShotSpawnCoordinates = null;
    [SerializeField] private Transform[] verticalShotSpawnCoordinates = null;
    [SerializeField] private Transform secondaryShotSpawnCoordinates = null;

    [Header("Ammo")]
    [SerializeField] private int primaryWeaponAmmo;
    [SerializeField] private int secondaryWeaponAmmo;

    [Header("Items")]
    [SerializeField] private GameObject jetCopter = null;
    
    private float shotAnimTimeCounter;
    private float continuousTimeCounter;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isShooting;
    private bool isWalking;
    private bool isLeft;
    private bool isGrounded;
    private AnimatorHashes animHashes = new AnimatorHashes();

    #endregion

    #region Props
    
    public byte PrimaryWeaponLevel 
    {
        get { return primaryWeaponLvl; }
        set { if(value <= primaryWeaponMaxLevel) primaryWeaponLvl = value; }
    }

    public byte SecondaryWeaponLevel 
    {
        get { return secondaryWeaponLvl; }
        set { if(value <= secondaryWeaponMaxLevel) secondaryWeaponLvl = value; }
    }

    public int PrimaryWeaponAmmo
    {
        get { return primaryWeaponAmmo; }
        private set 
        { 
            if(value <= maxAmmo && value >= 0)
            {
                primaryWeaponAmmo = value; 
                if(primaryWeaponAmmo == 0) PrimaryWeaponLevel = 1;
            }
        }
    }

    public int SecondaryWeaponAmmo
    {
        get { return secondaryWeaponAmmo; }
        private set 
        {
            if(value <= maxAmmo && value >= 0) 
            {
                secondaryWeaponAmmo = value;
                if(secondaryWeaponAmmo == 0) SecondaryWeaponLevel = 1;
            }
        }
    }

    public int Life
    {
        get { return _life; }
        set { if(value >= 0) _life = (value <= max_life)? value : max_life; }
    }

    public bool HasArmor {get; set;}

    public int ArmorLife
    {
        get => _armorLife;
        set  
        {   
            if(value >= 0) _armorLife = (value <= max_life)? value : max_life; 
            if(_armorLife == 0) HasArmor = false;
        }
    }

    public int Lives
    {
        get { return _lives; }
        set { _lives = (value <= max_lives)? value : max_lives; }
    }

    private GameObject Current1stShot
    {
        get 
        {                     
            byte index = primaryWeaponLvl < 3 && primaryWeaponLvl > 0? (byte) 0 : (byte) 1;
            return primaryShot[index];
        }
    }

    private GameObject Current2ndShot => secondaryShot[secondaryWeaponLvl - 1];

    public GameObject JetCopterObject => jetCopter;

    private bool Vertical => Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1;

    public Inventory Inventory => _inventory;

    public bool JetCopterActivated {get; set;}

    public Animator Animator => anim;

    #endregion

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
        Jump();
        Parachute();
        Shoot();
        ChangeSpecialItem();
    
        if(Input.GetKeyDown(KeyCode.Return))        
            TakeDamage(10);        
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, terrainLayerMask);        
        anim.SetBool(animHashes.isGrounded, isGrounded);
        
        if(isGrounded && rb.gravityScale != defaultGravity) ResetGravity();      
    }

    private void ChangeSpecialItem()
    {
        if(Input.GetButtonDown("ChangeSpecialItem") && !Inventory.IsEmpty)
        {   
            float value = Input.GetAxisRaw("ChangeSpecialItem");
            int length = Inventory.Slots.Count;
            int index = Inventory.Slots.IndexOf(Inventory.SelectedSlot);
            if(value > 0 && index < length - 1) index++;
            else if(value < 0 && index > 0) index--;            
            Inventory.SelectItem(index);
            Inventory.ShowSlot();
        }
    }

    private void UseSpecialItem()
    {
        if(Inventory.SelectedSlot != null && Inventory.SelectedSlot.Item != null)    
            Inventory.SelectedSlot.Item.Use();            
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");                 
        if(horizontal > 0 && isLeft) Flip();
        else if (horizontal < 0 && !isLeft) Flip();

        if(!HitWall())        
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
        Inventory.transform.localScale = new Vector2(isLeft? -1f : 1f, 1f);
    }

    private void Jump()
    {
        if(!JetCopterActivated)
        {
            if(Input.GetButtonDown("Jump") && isGrounded)
                rb.AddForce(Vector2.up * jumpForce);        
            else if(Input.GetButtonUp("Jump") && !isGrounded)                    
                rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
        else       
        {    
            if(Input.GetButton("Jump"))     
                rb.velocity = new Vector2(rb.velocity.x, speed);                                      
            else 
            {
                var newY = rb.velocity.y;
                rb.velocity = new Vector2(rb.velocity.x, newY > 0? newY : 0);          
            }
        }               
    }

    private void Parachute()
    {
        if(Input.GetButton("Jump") && !isGrounded && rb.velocity.y < 0)
        {
            rb.gravityScale = parachuteGravity;
            parachute.SetActive(true);
        }
        else if (Input.GetButtonUp("Jump")) ResetGravity();
    }

    private void ResetGravity()
    {
        rb.gravityScale = defaultGravity;
        if(parachute.activeSelf)        
            parachute.GetComponent<Animator>().SetTrigger("TurnOff");
    }

    private Transform GetShotSpawnCoordinates()
    {
        if(Vertical) 
        {
            // Se estiver olhando pra baixo, pega os 3 últimos valores do array,
            // onde foram armazenados os Transforms dos frames da galinha mirando pra baixo
            int downShotSpawnPosIndex = Input.GetAxisRaw("Vertical") < 0 ? 3 : 0;                 
            return verticalShotSpawnCoordinates[primaryWeaponLvl + downShotSpawnPosIndex - 1];
        }
        else return horizontalShotSpawnCoordinates[primaryWeaponLvl - 1];
    }

    private Quaternion GetShotRotation()
    {
        return Vertical? Quaternion.AngleAxis(90f, Vector3.forward) : Quaternion.identity;
    }

    private int GetShotDirection()
    {
        int direction = 0;
        if(Vertical)
            direction = Input.GetAxisRaw("Vertical") > 0 ? 1 : -1;
        else direction = isLeft? -1 : 1;

        return direction;
    }

    private void SetShotLevel2VariationRate(ref GameObject projectile)
    {
        float posVariationRate = Random.Range(shotLvl2VariationRate, -shotLvl2VariationRate);
        if(Vertical)        
            projectile.transform.position = new Vector3 
            (projectile.transform.position.x + posVariationRate, projectile.transform.position.y);        
        else
            projectile.transform.position = new Vector3 
            (projectile.transform.position.x, projectile.transform.position.y + posVariationRate);
    }

    private void InstantiatePrimaryShot()
    {
        if(PrimaryWeaponLevel == 1 || (PrimaryWeaponLevel > 1 && PrimaryWeaponAmmo > 0))
        {
            isShooting = true;    
            anim.SetInteger(animHashes.shotLevel, primaryWeaponLvl); 
            anim.SetTrigger(animHashes.shootTrigger);
            shotAnimTimeCounter = 0;

            var currentTransform = GetShotSpawnCoordinates();
            var currentRotation = GetShotRotation();
            var newBullet = Instantiate(Current1stShot, currentTransform.position, currentRotation);
            if(primaryWeaponLvl == 2) SetShotLevel2VariationRate(ref newBullet);

            newBullet.GetComponent<Projectile>().speed = shotspeed * GetShotDirection();

            SetAmmo(WeaponType.Primary, PrimaryWeaponAmmo - 1);
        }
    }

    private void InstantiateSecondaryShot()
    {        
        bool canSpawn = secondaryWeaponLvl == 1;
        canSpawn |= secondaryWeaponLvl == 2 && currentGrenade == null;
        if(canSpawn)
        {
            isShooting = true;
            anim.SetTrigger(animHashes.shootTrigger);
            anim.SetInteger(animHashes.shotLevel, 4);
            shotAnimTimeCounter = 0;

            currentGrenade = Instantiate(Current2ndShot, 
                secondaryShotSpawnCoordinates.position, Quaternion.identity);
            currentGrenade.GetComponent<Projectile>().speed *= isLeft? -1 : 1;
            var kawarimi = currentGrenade.GetComponent<Kawarimi>();
            if(kawarimi != null) kawarimi.penosa = gameObject;

            SetAmmo(WeaponType.Secondary, SecondaryWeaponAmmo - 1);
        }
    }

    private void InstantiatePrimaryShotLv2()
    {
        if(continuousTimeCounter >= machineGunInterval)
        {
            InstantiatePrimaryShot();
            continuousTimeCounter = 0f;
        }
        else continuousTimeCounter += Time.deltaTime;
    }

    private void ResetShootAnim()
    {
        shotAnimTimeCounter += Time.deltaTime;
        float timeToResetAnimation = primaryWeaponLvl <= 2? shotAnimDuration : shotLvl3Duration;
        if(shotAnimTimeCounter >= timeToResetAnimation)
        {            
            anim.SetInteger(animHashes.shotLevel, 0);
            shotAnimTimeCounter = 0;
            isShooting = false;            
        }
    }

    private void Shoot()
    {       
        if(primaryWeaponLvl == 1 && Input.GetButtonDown("Fire1"))        
            InstantiatePrimaryShot();
        else if(primaryWeaponLvl == 2 && Input.GetButton("Fire1"))         
            InstantiatePrimaryShotLv2();
        else if(primaryWeaponLvl == 3 && Input.GetButtonDown("Fire1") && !isShooting)
            InstantiatePrimaryShot();
        else if(Input.GetButtonDown("Fire2") && SecondaryWeaponAmmo > 0)
            InstantiateSecondaryShot();
        else if(Input.GetButtonDown("Fire3")) UseSpecialItem();
            
        if(isShooting) ResetShootAnim();
    }

    public void SetAmmo(WeaponType weaponType, int ammo)
    {
        if(weaponType == WeaponType.Primary && primaryWeaponLvl > 1)           
            PrimaryWeaponAmmo = ammo;        
        else if(weaponType == WeaponType.Secondary)
            SecondaryWeaponAmmo = ammo;
    }

    public void TakeDamage(int dmg)
    {
        if(HasArmor) ArmorLife -= dmg;
        else Life -= dmg;
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