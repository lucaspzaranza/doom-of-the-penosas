using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class EnemyWeaponGameObjectData : MonoBehaviour
{
    private const float LowerLimitDefault = 0.05f;

    [SerializeField] private bool _isBackWeapon;
    public bool IsBackWeapon => _isBackWeapon;

    [SerializeField] private Transform _referencePoint;
    public Transform ReferencePoint => _referencePoint;

    [SerializeField] private Transform _spawnTransform;
    public Transform SpawnTransform => _spawnTransform;
    
    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;
    public SpriteRenderer WeaponSpriteRenderer => _weaponSpriteRenderer;

    [SerializeField] private GameObject _rotationPivot;
    public GameObject RotationPivot => _rotationPivot;

    [SerializeField] private PlayerDetector _playerDetector;
    public PlayerDetector PlayerDetector => _playerDetector;
    [SerializeField] private bool _fireInVerticalAxis;
    public bool FireInVerticalAxis => _fireInVerticalAxis;

    [SerializeField] private bool _overwriteScale;
    public bool OverwriteScale => _overwriteScale;

    [DrawIfBoolEqualsTo("_overwriteScale", true)]
    [SerializeField] private Vector2 _weaponScale;

    [SerializeField] private bool _rotateTowardsPlayer;
    public bool RotateTowardsPlayer => _rotateTowardsPlayer;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed => _rotationSpeed;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField] private bool _useRotationLimit;
    public bool UseRotationLimit => _useRotationLimit;

    [DrawIfBoolEqualsTo("_useRotationLimit", true)]
    [SerializeField] private float _rotationUpperLimit;
    public float RotationUpperLimit => _rotationUpperLimit;

    [DrawIfBoolEqualsTo("_useRotationLimit", true)]
    [SerializeField] private float _rotationLowerLimit;
    public float RotationLowerLimit => _rotationLowerLimit;

    private bool _hasDetectedPlayer;
    public bool HasDetectedPlayer => _hasDetectedPlayer;

    private DamageableObject _detectedPlayer;
    private float _playerAngle = 0f;
    private float _defaultZValue;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField]
    [DrawItDisabled]
    private float _upperLimit;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField]
    [DrawItDisabled]
    private float _lowerLimit;

    [SerializeField]
    [DrawItDisabled]
    private bool _hasFlipped;

    private bool _playerChangedPosition;
    private Vector2 _playerPrevPos;
    private float _prevAngle;

    private void OnEnable()
    {
        if(_weaponSpriteRenderer == null)
            _weaponSpriteRenderer = GetComponent<SpriteRenderer>();

        _defaultZValue = RotationPivot.transform.rotation.eulerAngles.z;

        if(RotationLowerLimit == 0f)
            _rotationLowerLimit = LowerLimitDefault;

        _upperLimit = FireInVerticalAxis ? RotationUpperLimit : _defaultZValue + RotationUpperLimit;
        _lowerLimit = FireInVerticalAxis ? RotationLowerLimit : _defaultZValue + RotationLowerLimit;

        if(IsBackWeapon && WeaponIsLeftOriented())
        {
            _upperLimit = -RotationLowerLimit;
            _lowerLimit = -RotationUpperLimit;
        }

        if(!FireInVerticalAxis)
            _hasFlipped = IsBackWeapon? WeaponIsLeftOriented() : false;
        else
            _hasFlipped = !WeaponIsUpOriented();

        if(_playerDetector == null)
            _playerDetector = GetComponent<PlayerDetector>();

        _playerPrevPos = Vector2.zero;
        _playerChangedPosition = false;
    }

    public void SetWeaponSprite(Sprite newSprite)
    {
        WeaponSpriteRenderer.sprite = newSprite;
        
        if(OverwriteScale)
            WeaponSpriteRenderer.transform.localScale = new Vector2(_weaponScale.x, _weaponScale.y);
    }

    private void Update()
    {
        //print("WeaponIsUpOriented? " + WeaponIsUpOriented());

        if (!RotateTowardsPlayer)
            return;

        if(PlayerDetector == null)
        {
            WarningMessages.PlayerDetectorNotFound();
            return;
        }

        _hasDetectedPlayer = 
                PlayerDetector.DetectedPlayerNearObject(out _detectedPlayer);

        if(_hasDetectedPlayer)
        {
            Vector2 directionVector = RotationPivot.transform.position - _detectedPlayer.transform.position;

            if(!FireInVerticalAxis)
            {
                directionVector = WeaponIsLeftOriented() ?
                    RotationPivot.transform.position - _detectedPlayer.transform.position:
                    _detectedPlayer.transform.position - RotationPivot.transform.position;
            }
            else
            {
                directionVector = WeaponIsUpOriented() ?
                    RotationPivot.transform.position - _detectedPlayer.transform.position:
                    _detectedPlayer.transform.position - RotationPivot.transform.position;
            }

            _playerAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            _playerChangedPosition = _detectedPlayer.transform.position.y != _playerPrevPos.y;

            _playerPrevPos = _detectedPlayer.transform.position;
        }
    }

    public virtual bool WeaponIsLeftOriented()
    {
        return RotationPivot.transform.position.x > ReferencePoint.transform.position.x;
    }

    public virtual bool WeaponIsUpOriented()
    {
        //print("rotation: " + RotationPivot.transform.position.y + " reference: " + ReferencePoint.transform.position.y);
        return RotationPivot.transform.position.y > ReferencePoint.transform.position.y;
    }

    public void RotateWeaponTowardsPlayer()
    {
        if (!RotateTowardsPlayer)
            return;                

        if(_hasDetectedPlayer)
        {
            float current = RotationPivot.transform.rotation.eulerAngles.z;

            if (FireInVerticalAxis)
                current -= 360f;
            else if (WeaponIsLeftOriented())
                current -= 360f;

            if (current == _playerAngle)
                return;

            //print($"current before: {current}, player angle: {_playerAngle}");
            current = Mathf.MoveTowardsAngle(current, _playerAngle, RotationSpeed);
            //print($"current after: {current}, player angle: {_playerAngle}");

            if (UseRotationLimit)
            {
                UpdateRotationLimits();
                current = Mathf.Clamp(current, _lowerLimit, _upperLimit);
            }

            SetRotation(current);
        }
    }

    private void SetRotation(float value)
    {
        Quaternion newRotation = Quaternion.AngleAxis(value, Vector3.forward);
        RotationPivot.transform.rotation = newRotation;
        _prevAngle = value;
    }

    public void UpdateRotationLimits()
    {
        if (!FireInVerticalAxis) // Horizontal weapon
        {
            if (WeaponIsLeftOriented() && !_hasFlipped) // Horizontal weapon has flipped to the left <-
            {
                float sum = _upperLimit + _lowerLimit;
                _upperLimit -= sum;
                _lowerLimit -= sum;

                _hasFlipped = true;
            }
            else if (!WeaponIsLeftOriented() && _hasFlipped) // Horizontal weapon has flipped to the right -> 
            {
                _upperLimit = _defaultZValue + RotationUpperLimit;
                _lowerLimit = _defaultZValue + RotationLowerLimit;
                _hasFlipped = false;
            }
        }
    }

    public void Flip()
    {
        if (FireInVerticalAxis)
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y); 
    }
}
