using UnityEngine;

[System.Serializable]
public class LandCharacterProps
{
    public Transform GroundCheck = null;
    public Collider2D CharacterCollider = null;
    public Collider2D GroundCheckCollider = null;
    public Collider2D WallCheckCollider = null;

    [Tooltip("It'll be used to detect if the character is in the ground.")]
    public LayerMask TerrainLayerMask;

    [Tooltip("It'll be used to detect if the character can move forward.")]
    public LayerMask TerrainWithoutPlatformLayerMask;

    public LayerMask WaterLayerMask;
    public LayerMask PlatformOnlyLayerMask;

    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsOnPlatform;
}

[System.Serializable]
public class FlyingCharaterProps
{
    public Collider2D FlyingCheckCollider;
    public LayerMask FlyingLayerMask;
}