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

    [SerializeField] private bool _rotateTowardsPlayer;
    public bool RotateTowardsPlayer => _rotateTowardsPlayer;

    [SerializeField] private bool _overwriteScale;
    public bool OverwriteScale => _overwriteScale;

    [DrawIfBoolEqualsTo("_overwriteScale", true)]
    [SerializeField] private Vector2 _weaponScale;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed => _rotationSpeed;

    [DrawIfBoolEqualsTo("_rotateTowardsPlayer", true)]
    [SerializeField] private bool _useRotationLimit;
    public bool UseRotationLimit => _useRotationLimit;

    [DrawIfBoolEqualsTo("_useRotationLimit", true)]
    [SerializeField] private float _rotationLimit;
    public float RotationLimit => _rotationLimit;

    private bool _hasDetectedPlayer;
    public bool HasDetectedPlayer => _hasDetectedPlayer;

    private DamageableObject _detectedPlayer;
    private float _playerAngle = 0f;
    private float _upperLimit;
    private float _lowerLimit;

    private void OnEnable()
    {
        if(_weaponSpriteRenderer == null)
            _weaponSpriteRenderer = GetComponent<SpriteRenderer>();

        _upperLimit = transform.rotation.eulerAngles.z + RotationLimit;
        _lowerLimit = transform.rotation.eulerAngles.z - RotationLimit;
    }

    public void SetWeaponSprite(Sprite newSprite)
    {
        WeaponSpriteRenderer.sprite = newSprite;
        
        if(OverwriteScale)
            transform.localScale = new Vector2(_weaponScale.x, _weaponScale.y);
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
            float current = transform.rotation.eulerAngles.z;

            if (current == _playerAngle)
                return;
            
            if(UseRotationLimit)
            {
                // Ver pq tá dando errado com lower limit da Direita, e com os limites da Esquerda
                print("current: " + current);
                print("_playerAngle: " + _playerAngle);
                print("_upperLimit: " + _upperLimit);
                print("_lowerLimit: " + _lowerLimit);

                if (current < _playerAngle && current >= _upperLimit) // Rotatin up and passed the limit
                    return;
                else if (current <= _lowerLimit) // Rotatin down and passed the limit
                    return;
            }

            current = Mathf.MoveTowardsAngle(current, _playerAngle, RotationSpeed);
            Quaternion newRotation = Quaternion.AngleAxis(current, Vector3.forward);
            transform.rotation = newRotation;
        }
    }
}
