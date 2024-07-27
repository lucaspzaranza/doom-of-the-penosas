using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class used to detect Player presence near some object.
/// </summary>
public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _hitLayer;
    public LayerMask RaycastLayer => _hitLayer;

    [SerializeField] private Collider2D _collider;

    private DamageableObject _damageableObject = null;
    private ContactFilter2D _contactFilter = new ContactFilter2D();
    private List<Collider2D> results = new List<Collider2D>();

    private Enemy _enemyComponent = null;
    public Enemy EnemyComponent
    {
        get
        {
            if (_enemyComponent == null)
                this.GetComponentInAnyParent(out _enemyComponent);

            return _enemyComponent;
        }
    }
        

    private bool HasEnemyComponent =>
        _enemyComponent != null || this.GetComponentInAnyParent(out _enemyComponent);

    private void OnEnable()
    {
        _contactFilter.SetLayerMask(RaycastLayer);
    }

    /// <summary>
    /// Detects using Raycast if the player is near the Object based on the configured direction Vector2 and
    /// in determined distance.
    /// </summary>
    /// <param name="player">The detected player (if exists any).</param>
    /// <returns>True if encountered some player, otherwise returns false.</returns>
    public bool DetectedPlayerNearObject(out DamageableObject player)
    {
        bool detectedPlayer = false;
        player = null;

        if(Physics2D.OverlapCollider(_collider, _contactFilter, results) > 0)
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
}
