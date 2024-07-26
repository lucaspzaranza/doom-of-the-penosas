using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class used to detect Player presence near some object.
/// </summary>
public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private bool _isLeft;
    public bool IsLeft
    {
        get => _isLeft;
        set => _isLeft = value;
    }

    [SerializeField] private bool _useOverlapArea;

    [SerializeField] private LayerMask _raycastLayer;
    public LayerMask RaycastLayer => _raycastLayer;

    [Header("Raycast Data")]

    [Tooltip("Insert here the number of possibles results the RaycastHit2D[] array may store " +
    "to check if this detector has found some player.")]
    //[DrawIfBoolEqualsTo("_useOverlapArea", false)]
    [SerializeField] private int _resultsLimit;

    //[DrawIfBoolEqualsTo("_useOverlapArea", false)]
    [SerializeField] private float _raycastDistance;
    public float RaycastDistance => _raycastDistance;    

    //[DrawIfBoolEqualsTo("_useOverlapArea", false)]
    [SerializeField] private Vector2 _direction;

    [Header("Overlap Area Data")]

    [DrawIfBoolEqualsTo("_useOverlapArea", true, true)]
    [SerializeField] private Vector2 _areaSize;
    [DrawIfBoolEqualsTo("_useOverlapArea", true, true)]
    [SerializeField] private Vector2 _offset;

    private RaycastHit2D[] _raycasts;
    private DamageableObject _damageableObject = null;

    private ContactFilter2D contactFilter = new ContactFilter2D();
    private List<Collider2D> results = new List<Collider2D>();
    private Enemy _enemyComponent = null;
    private Vector2 _prevPos;
    private Vector2 _initialAreaSize;

    private void OnEnable()
    {
        _prevPos = transform.position;
        _initialAreaSize = _areaSize;
    }

    /// <summary>
    /// Detects using Raycast if the player is near the Object based on the configured direction Vector2 and
    /// in determined distance.
    /// </summary>
    /// <param name="player">The detected player (if exists any).</param>
    /// <returns>True if encountered some player, otherwise returns false.</returns>
    public bool DetectedPlayerNearObject(Vector2 basePosition, out DamageableObject player)
    {
        bool detectedPlayer = false;
        player = null;
        _raycasts = new RaycastHit2D[_resultsLimit];

        if (Physics2D.RaycastNonAlloc(basePosition, _direction, 
        _raycasts, RaycastDistance, RaycastLayer) > 0)
        {
            detectedPlayer = _raycasts.First(raycast =>
                raycast.collider != null                                       // Found some object at the Layer Mask
                && raycast.collider.TryGetComponent(out _damageableObject)     // And is something which the enemy can damage
                && SharedFunctions.DamageableObjectIsPlayer(_damageableObject) // And this object is a player.
            );

            if (detectedPlayer)
                player = _damageableObject;
        }

        return detectedPlayer;
    }

    /// <summary>
    /// Detects using OverlapArea if the player is near the Object based on the given position 
    /// and area size.
    /// </summary>
    /// <param name="player">The detected player (if exists any).</param>
    /// <returns>True if encountered some player, otherwise returns false.</returns>
    public bool DetectedPlayerNearObjectUsingOverlapArea(Vector2 basePosition, out DamageableObject player)
    {
        bool detectedPlayer = false;
        player = null;      
        contactFilter.SetLayerMask(RaycastLayer);

        basePosition += _offset;
        // Found some object at the Layer Mask inside the box area...
        if (Physics2D.OverlapArea(basePosition, _areaSize, contactFilter, results) > 0)
        {
            detectedPlayer = results.First(contactedObject =>
                contactedObject.TryGetComponent(out _damageableObject)          // ... and is something which the enemy can damage
                && SharedFunctions.DamageableObjectIsPlayer(_damageableObject)  // ... and this object is a player.
            );

            if (detectedPlayer)
                player = _damageableObject;
        }

        return detectedPlayer;
    }

    /// <summary>
    /// Call this function to invert the raycast direction vector horizontally or vertically.
    /// </summary>
    /// <param name="horizontalFlip">Check this as false if you want to invert vertically.</param>
    /// <param name="invertOverlapArea">Check this as true if you want to invert Overlap area, if used.</param>
    public void Flip(FlipType type, bool invertOverlapArea = false)
    {
        float enemyPosX = 0f;
        float enemyPosY = 0f;

        if(_enemyComponent != null || this.GetComponentInAnyParent(out _enemyComponent))
        {
            enemyPosX = _enemyComponent.gameObject.transform.localPosition.x;
            enemyPosY = _enemyComponent.gameObject.transform.localPosition.y;
        }

        switch (type)
        {
            case FlipType.Horizontal:

                _direction = new Vector2(-_direction.x, _direction.y);
                bool isNegative = _direction.x < 0;
                if (_useOverlapArea && invertOverlapArea)
                {
                    _areaSize = new Vector2((isNegative? -_initialAreaSize.x : _initialAreaSize.x) + enemyPosX, _initialAreaSize.y);
                    _offset = new Vector2(-_offset.x, _offset.y);
                }

                break;

            case FlipType.Vertical:

                _direction = new Vector2(_direction.x, -_direction.y);
                isNegative = _direction.y < 0;
                if (_useOverlapArea && invertOverlapArea)
                {
                    _areaSize = new Vector2(_initialAreaSize.x, -_initialAreaSize.y);
                    _areaSize = new Vector2(_initialAreaSize.x, (isNegative ? -_initialAreaSize.y : _initialAreaSize.y) + enemyPosY);
                    _offset = new Vector2(_offset.x, -_offset.y);
                }

                break;

            case FlipType.Both:

                _direction = new Vector2(-_direction.x, -_direction.y);
                if (_useOverlapArea && invertOverlapArea)
                {
                    _areaSize = new Vector2(-_initialAreaSize.x, -_initialAreaSize.y);
                    _offset = new Vector2(-_offset.x, _offset.y);
                }

                break;

            default:
                break;
        }

        UpdateOrientation();
    }

    public void UpdateOrientation()
    {
        if(this.GetComponentInAnyParent(out _enemyComponent))
        {
            IsLeft = _enemyComponent.IsLeft;
            return;
        }

        if (_direction.x != 0)
            IsLeft = _direction.x < 0;
        else
            IsLeft = !IsLeft;
    } 

    public void UpdateOverlapAreaPosition(Vector2 pos)
    {
        Vector2 deltaPos = pos - _prevPos;
        deltaPos = new Vector2(Mathf.Abs(deltaPos.x), Mathf.Abs(deltaPos.y));
        Vector2 result = Vector2.zero;

        if (_enemyComponent != null || this.GetComponentInAnyParent(out _enemyComponent))
        {
            if(_enemyComponent.EnemyType == EnemyType.Land || 
            (_enemyComponent.EnemyType == EnemyType.Flying && _enemyComponent.FlyingChaseMode == FlyingChaseMode.Horizontal))
            {
                if (!IsLeft)
                    result = new Vector2(_areaSize.x + deltaPos.x, _areaSize.y);
                else
                    result = new Vector2(_areaSize.x - deltaPos.x, _areaSize.y);
            }
            else if (_enemyComponent.EnemyType == EnemyType.Flying && _enemyComponent.FlyingChaseMode == FlyingChaseMode.Vertical)
            {
                if(_enemyComponent.IsDown)
                    result = new Vector2(_areaSize.x, _areaSize.y - deltaPos.y);
                else
                    result = new Vector2(_areaSize.x, _areaSize.y + deltaPos.y);
            }
        }

        _areaSize = result;
        _prevPos = pos;
    }
}
