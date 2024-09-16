using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFromPlatform : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _terrainLayer;
    [Tooltip("Distance from ground to check if there is some terrain block which may block the player to " +
    "fall from the platform.")]
    [SerializeField] private float _distance;

    private bool _canFall;
    public bool CanFallFromPlatform => _canFall;

    private BoxCollider2D _collider;
    public BoxCollider2D Collider => _collider;

    private PlatformPlayerDetector _playerDetector;
    public PlatformPlayerDetector PlayerDetector => _playerDetector;

    private List<Penosa> _playersOnPlatform = new List<Penosa>();

    void OnEnable()
    {
        _collider = GetComponent<BoxCollider2D>();
        _playerDetector = GetComponentInChildren<PlatformPlayerDetector>();
    }

    public void AddPlayerOnPlatform(Penosa player)
    {
        if(!_playersOnPlatform.Contains(player))
            _playersOnPlatform.Add(player);
    }

    public bool HasTerrainAbovePlatform(Vector2 playerPosition)
    {
        playerPosition = new Vector2 (playerPosition.x, playerPosition.y - _distance);
        return Physics2D.OverlapCircle(playerPosition, PlayerConsts.OverlapCircleDiameter, _terrainLayer);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == ConstantStrings.PlayerTag && other.transform.position.y > transform.position.y) // Is above from
        {
            var player = other.gameObject.GetComponent<Penosa>();
            AddPlayerOnPlatform(player);
            player.SetPlatform(this);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == ConstantStrings.PlayerTag)
        {
            var player = other.gameObject.GetComponent<Penosa>();
            player.SetPlatform(null);
            _playersOnPlatform.Remove(player);
        }
    }
}
