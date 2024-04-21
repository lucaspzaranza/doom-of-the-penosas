using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Action<GameObject> OnReturnProjectileToPool;

    [SerializeField] private float _speed;
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    [SerializeField] private bool _isEnemyFire;
    public bool IsEnemyFire => _isEnemyFire;

    [SerializeField] private bool _isMissile = false;
    public bool IsMissile => _isMissile;

    [SerializeField] protected Collider2D _collider;
    public Collider2D Collider2D => _collider;

    [SerializeField] private int _damage;
    public int Damage => _damage;

    [SerializeField] protected LayerMask _interactableLayerMask;
    public LayerMask InteractableLayerMask => _interactableLayerMask;

    public virtual void Update()
    {
        transform.Translate(Vector3.right * Speed * Time.deltaTime);
        if (SharedFunctions.HitSomething(_collider, _interactableLayerMask, out Collider2D hitObject))
        {
            if(IsEnemyFire)
            {
                print("If hit player, do damage on him.");
            }
            else
            {
                TryToDamageEnemy(ref hitObject);

                if (!IsMissile)
                    OnReturnProjectileToPool?.Invoke(gameObject);
                else
                    Destroy(gameObject);
            }
        }
    }

    public virtual void TryToDamageEnemy(ref Collider2D hitObject)
    {
        var enemy = hitObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            print($"{enemy.name} will take damage of {Damage}.");
            enemy.TakeDamage(Damage);
        }
    }
}