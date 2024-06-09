using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponGameObjectData : MonoBehaviour
{
    [SerializeField] private Transform _spawnTransform;
    public Transform SpawnTransform => _spawnTransform;

    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;
    public SpriteRenderer WeaponSpriteRenderer => _weaponSpriteRenderer;

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
    //[DrawItDisabled]
    private float _upperLimit;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField] 
    //[DrawItDisabled] 
    private float _lowerLimit;

    private bool _hasFlipped;

    private void OnEnable()
    {
        if(_weaponSpriteRenderer == null)
            _weaponSpriteRenderer = GetComponent<SpriteRenderer>();

        _defaultZValue = transform.rotation.eulerAngles.z;

        _upperLimit = FireInVerticalAxis? RotationUpperLimit : _defaultZValue + RotationUpperLimit;
        _lowerLimit = FireInVerticalAxis? RotationLowerLimit : _defaultZValue + RotationLowerLimit;

        _hasFlipped = false;
    }

    public void SetWeaponSprite(Sprite newSprite)
    {
        WeaponSpriteRenderer.sprite = newSprite;
        
        if(OverwriteScale)
            WeaponSpriteRenderer.transform.localScale = new Vector2(_weaponScale.x, _weaponScale.y);
    }

    private void Update()
    {
        if (!RotateTowardsPlayer)
            return;

        _hasDetectedPlayer = 
                PlayerDetector.DetectedPlayerNearObjectUsingOverlapArea(transform.position, out _detectedPlayer);

        if(_hasDetectedPlayer)
        {
            Vector2 directionVector = PlayerDetector.IsLeft ?
                transform.position - _detectedPlayer.transform.position :
                _detectedPlayer.transform.position - transform.position;

            _playerAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        }
    }

    public void RotateWeaponTowardsPlayer()
    {
        if (!RotateTowardsPlayer)
            return;

        if(_hasDetectedPlayer)
        {
            float current = (transform.rotation.z >= 0) ? transform.rotation.eulerAngles.z :
                transform.rotation.eulerAngles.z - 360f;

            if (current == _playerAngle)
                return;

            current = Mathf.MoveTowardsAngle(current, _playerAngle, RotationSpeed);

            if(UseRotationLimit)
            {
                UpdateRotationLimits(current);
                current = Mathf.Clamp(current, _lowerLimit, _upperLimit);
            }

            Quaternion newRotation = Quaternion.AngleAxis(current, Vector3.forward);
            transform.rotation = newRotation;
        }
    }

    private void UpdateRotationLimits(float current = 0f)
    {
        print(PlayerDetector.IsLeft);

        if (PlayerDetector.IsLeft && !_hasFlipped) // Left rotation
        {
            print("Is Left and hasn't flipped");
            if (!FireInVerticalAxis) // Left and Horizontal
            {
                float sum = _upperLimit + _lowerLimit;
                _upperLimit -= sum;
                _lowerLimit -= sum;
                _hasFlipped = true;
                print($"isLeft. current: {current}, upper: {_upperLimit}, lower: {_lowerLimit}");
            }
            else // Left and Vertical
            {
                current = -current;
                _hasFlipped = true;
                print($"isLeft. current: {current}, upper: {_upperLimit}, lower: {_lowerLimit}");
            }
        }
        else if (!PlayerDetector.IsLeft && _hasFlipped) // Right rotation
        {
            if(!FireInVerticalAxis) // Right and Horizontal
            {
                _upperLimit = _defaultZValue + RotationUpperLimit;
                _lowerLimit = _defaultZValue + RotationLowerLimit;
                _hasFlipped = false;
                print($"isRight. current: {current}, upper: {_upperLimit}, lower: {_lowerLimit}");
            }
            else // Right and Vertical
            {
                _upperLimit = RotationUpperLimit;
                _lowerLimit = RotationLowerLimit;
                _hasFlipped = false;
                print($"isRight. current: {current}, upper: {_upperLimit}, lower: {_lowerLimit}");
            }
        }
    }
}
