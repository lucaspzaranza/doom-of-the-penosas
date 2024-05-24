using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class used to detect Player presence near some object.
/// </summary>
public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastLayer;
    public LayerMask RaycastLayer => _raycastLayer;

    [Tooltip("Insert here the number of possibles results the RaycastHit2D[] array may store " +
        "to check if this detector has found some player.")]
    [SerializeField] private int _resultsLimit;

    [SerializeField] private float _raycastDistance;
    public float RaycastDistance => _raycastDistance;

    [SerializeField] private bool _isLeft;
    public bool IsLeft
    {
        get => _isLeft;
        set => _isLeft = value;
    }

    [SerializeField] private Vector2 _direction;

    private RaycastHit2D[] _raycasts;
    private DamageableObject _damageableObject = null;

    /// <summary>
    /// Detects if the player is near the Object based on the configured direction Vector2 and
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
            detectedPlayer = _raycasts.Any(raycast =>
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
    /// Call this function to invert the raycast direction vector horizontally or vertically.
    /// </summary>
    /// <param name="horizontalFlip">Check this as false if you want to invert vertically.</param>
    /// <param name="invertBoth">Check this as true if you want to invert both axes.</param>
    public void Flip(bool horizontalFlip = true, bool invertBoth = false)
    {
        if(invertBoth)
        {
            _direction = _direction = new Vector2(-_direction.x, -_direction.y);
            return;
        }

        if (horizontalFlip)
            _direction = new Vector2(-_direction.x, _direction.y);
        else
            _direction = new Vector2(_direction.x, -_direction.y);
    }
}
