using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    private EnemyWeaponUnit _enemyWeaponUnit;

    public void SetEnemyWeaponUnit(EnemyWeaponUnit enemyWeaponUnit)
    {
        _enemyWeaponUnit = enemyWeaponUnit;
    }

    protected override void CheckForCollision()
    {
        if (SharedFunctions.HitSomething(_collider, _interactableLayerMask, out Collider2D hitObject))
        {
            if(hitObject.TryGetComponent(out Penosa penosa))
                penosa.TakeDamage(Damage);
            else if(hitObject.TryGetComponent(out RideArmor rideArmor))
                rideArmor.Player?.TakeDamage(Damage);

            if(_enemyWeaponUnit.UsePooling)
                OnReturnProjectileToPool?.Invoke(gameObject);
            else
                Destroy(gameObject);
        }
    }    
}
