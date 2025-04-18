using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RideArmor : DamageableObject
{
    public static Action<RideArmor> OnRideArmorEquipped;
    public static Action<RideArmorType, bool> OnRideArmorChangedRequired;

    public Action<int> OnRideArmorLifeChanged;

    [SerializeField]
    private float _upperLimitDegree = 90f;

    [SerializeField]
    private float _lowerLimitDegree = 345f;

    [SerializeField] private RideArmorType _type;
    public RideArmorType Type => _type;

    [Tooltip("Set this value if this Ride Armor is necessary to traverse the stage. For example, " +
    "a water stage must require the Jet Skinha, or an air stage will require the Chickencopter.")]
    [SerializeField] protected bool _required;
    public bool Required
    {
        get => _required;
        set
        {
            _required = value;
            OnRideArmorChangedRequired?.Invoke(_type, value);
        }
    }

    [SerializeField] private Penosa _player;
    public Penosa Player => _player;

    protected Rigidbody2D _rb;
    public Rigidbody2D RigiBody2DComponent => _rb;

    [SerializeField] protected GameObject _cannon;
    public GameObject Cannon => _cannon;

    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed => _rotationSpeed;    

    public Transform PlayerPosition => _playerPos;

    [SerializeField] protected RideArmorActivator _armorCore;
    [SerializeField] protected GameObject _eggShot;
    [SerializeField] protected GameObject _shurikenShot;
    [SerializeField] protected Transform _shotSpawnPos;
    [SerializeField] protected Transform _playerPos;
    [SerializeField] protected Collider2D _wallCheckCollider = null;
    [SerializeField] protected LayerMask _terrainLayerMask;
    [SerializeField] protected LayerMask _terrainWithoutPlatformLayerMask;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected SpriteRenderer _cannonSR;
    [SerializeField] protected List<GameObject> _equippedPlayerList;

    protected GameObject _equippedPlayer;

    private PlayerController _playerController;
    private float _continuousTimeCounter;
    private int _direction = 1;

    public virtual void OnEnable()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Move(Vector2 direction)
    {
        if (_player != null)
            _player.transform.localPosition = PlayerPosition.localPosition;
    }

    public virtual void Jump()
    {

    }

    public virtual void Aim(float direction)
    {
        Cannon.transform.Rotate(Vector3.forward * direction * _rotationSpeed * Time.deltaTime);

        float eulerZ = _cannon.transform.localEulerAngles.z;

        if (direction == 1 && eulerZ > _upperLimitDegree && eulerZ < _lowerLimitDegree)
            Cannon.transform.localEulerAngles = new Vector3(0f, 0f,
                Mathf.Clamp(eulerZ, 0f, _upperLimitDegree));
        else if (direction == -1 && eulerZ > _upperLimitDegree && eulerZ < 360f && eulerZ < _lowerLimitDegree)
            Cannon.transform.localEulerAngles = new Vector3(0f, 0f, _lowerLimitDegree);
    }

    public virtual void Shoot()
    {
        if(_continuousTimeCounter >= PlayerConsts.RideArmorMachineGunInterval)
        {
            _continuousTimeCounter = 0;

            GameObject newBullet = _playerController.RequestProjectileFromGameController
                (_player.PlayerData.Character == Penosas.Geruza? _eggShot : _shurikenShot);
            newBullet.transform.position = _shotSpawnPos.position;
            newBullet.transform.rotation = _cannon.transform.rotation;
            newBullet.transform.localScale =
                new Vector2(newBullet.transform.localScale.x * _direction, newBullet.transform.localScale.y);

            _player.SetShotLevel2VariationRate(ref newBullet);
            newBullet.GetComponent<Projectile>().Speed = _player.ShotSpeed * _direction;
        }
        else
            _continuousTimeCounter += Time.deltaTime;
    }

    public void SetDirection(int direction)
    {
        _direction = direction;
    }

    public virtual void Equip(Penosa player, PlayerController playerController)
    {
        _player = player;        
        _playerController = playerController;

        if(Type != RideArmorType.EggTank)
            SetEquippedPlayerActivation(player, true);

        OnRideArmorEquipped?.Invoke(this);
    }

    public virtual void DestroyRideArmor()
    {
        _player.EjectRideArmor();
        Destroy(gameObject);
    }

    public virtual void Eject()
    {
        _player = null;
    }

    public virtual void Blink(Color newColor)
    {
        _spriteRenderer.color = newColor;
        _cannonSR.color = newColor;

        if(_equippedPlayer != null)
            _equippedPlayer.GetComponent<SpriteRenderer>().color = newColor;
    }

    public override void TakeDamage(int damage, bool force = false)
    {
        if(Player == null) 
            return;

        base.TakeDamage(damage, force);
    }

    protected override void SetLife(int value)
    {
        if (Player == null)
            return;

        _life = Mathf.Clamp(value, 0, PlayerConsts.Max_Life); ;
        OnRideArmorLifeChanged?.Invoke(_life);

        if (_life == 0)
            DestroyRideArmor();
    }

    protected void SetEquippedPlayerActivation(Penosa player, bool value)
    {
        var playerToShow = _equippedPlayerList
            .FirstOrDefault(p => p.name == player.PlayerData.Character.ToString());

        if (playerToShow != null)
        {
            _equippedPlayer = playerToShow;
            _equippedPlayer.SetActive(value);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(Type != RideArmorType.JetSkinha && other.gameObject.layer == 4) //Water layer
            Life = 0;
    }
}
