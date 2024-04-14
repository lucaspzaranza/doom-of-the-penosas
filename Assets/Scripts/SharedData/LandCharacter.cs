using UnityEngine;

[System.Serializable]
public class LandCharacterProps
{
    [SerializeField] public Transform GroundCheck = null;
    [SerializeField] public Collider2D CharacterCollider = null;
    [SerializeField] public Collider2D GroundCheckCollider = null;
    [SerializeField] public Collider2D WallCheckCollider = null;
    [Tooltip("It'll be used to detect if the character is in the ground.")]
    [SerializeField] public LayerMask TerrainLayerMask;
    [Tooltip("It'll be used to detect if the character can move forward.")]
    [SerializeField] public LayerMask TerrainWithoutPlatformLayerMask;
    [SerializeField] public LayerMask WaterLayerMask;
    [SerializeField] public LayerMask PlatformOnlyLayerMask;

    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsOnPlatform;
}
