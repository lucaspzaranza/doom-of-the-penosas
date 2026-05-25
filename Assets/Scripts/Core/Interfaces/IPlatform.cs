using UnityEngine;

public interface IPlatform
{
    BoxCollider2D Collider { get; }

    bool HasTerrainAbovePlatform(Vector2 playerPosition);
}