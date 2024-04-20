using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Action<GameObject> OnReturnProjectileToPool;

    [SerializeField] private float _speed;
    [SerializeField] protected Collider2D _collider;
    public int damage;
    public bool isMissile = false;
    public LayerMask interactableLayerMask;

    public float Speed 
    {
        get => _speed;
        set => _speed = value;
    }

    public virtual void Update()
    {
        transform.Translate(Vector3.right * Speed * Time.deltaTime);
        if (SharedFunctions.HitSomething(_collider, interactableLayerMask, out Collider2D hitObject))
        {
            TryToDamageEnemy(ref hitObject);

            if (!isMissile)
                OnReturnProjectileToPool?.Invoke(gameObject);
            else
                Destroy(gameObject);
        }
    }

    public virtual void TryToDamageEnemy(ref Collider2D hitObject)
    {
        var enemy = hitObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            print($"{enemy.name} will take damage of {damage}.");
            enemy.TakeDamage(damage);
        }
    }
}