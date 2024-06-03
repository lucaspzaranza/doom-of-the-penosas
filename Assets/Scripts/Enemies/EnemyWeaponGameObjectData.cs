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

    private bool _hasDetectedPlayer;
    public bool HasDetectedPlayer => _hasDetectedPlayer;

    private DamageableObject _detectedPlayer;
    private float _playerAngle = 0f;

    private void OnEnable()
    {
        if(_weaponSpriteRenderer == null)
            _weaponSpriteRenderer = GetComponent<SpriteRenderer>();
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

    public void RotateWeaponTowardsPlayer(bool isLeft)
    {
        if (!RotateTowardsPlayer)
            return;

        if(_hasDetectedPlayer)
        {
            float current = transform.rotation.eulerAngles.z;

            if (current == _playerAngle)
                return;

            current = Mathf.MoveTowardsAngle(current, _playerAngle, RotationSpeed);
            Quaternion newRotation = Quaternion.AngleAxis(current, Vector3.forward);
            transform.rotation = newRotation;
        }
    }
}
