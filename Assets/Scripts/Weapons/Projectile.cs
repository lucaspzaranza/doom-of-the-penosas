using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Action<GameObject> OnReturnProjectileToPool;

    [SerializeField] private float _speed;
    public int damage;
    public bool isMissile = false;
    public LayerMask interactableLayerMask;

    public float Speed 
    {
        get => _speed;
        set => _speed = value;
    }

    protected bool TouchedProjectileInteractable => 
        Physics2D.OverlapCircle(transform.position, 0.1f, interactableLayerMask);
    
    public virtual void Update()
    {
        transform.Translate(Vector3.right * Speed * Time.deltaTime);
        if (TouchedProjectileInteractable)
        {
            if (!isMissile)
                OnReturnProjectileToPool?.Invoke(gameObject);
            else
                Destroy(gameObject);
        }
    }
}