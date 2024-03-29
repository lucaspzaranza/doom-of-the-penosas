using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : ControllerUnit
{
    private GameObject _poolInstance;

    [Space]
    [SerializeField] private GameObject _poolPrefab;


    [SerializeField] private ObjectPool _pool;
    public ObjectPool Pool => _pool;

    public override void Setup()
    {
        base.Setup();

        _poolInstance = Instantiate(_poolPrefab, transform);
        _pool = _poolInstance.GetComponent<ObjectPool>();
        Projectile.OnReturnProjectileToPool += HandleOnReturnProjectileToPool;

        InitiateProjectilesPools();
    }

    public override void Dispose()
    {
        Projectile.OnReturnProjectileToPool -= HandleOnReturnProjectileToPool;
        Destroy(_poolInstance);
    }

    private void InitiateProjectilesPools()
    {
        StartCoroutine(_pool.InitializePool(ConstantStrings.EggShot));
        StartCoroutine(_pool.InitializePool(ConstantStrings.BigEggShot));
        StartCoroutine(_pool.InitializePool(ConstantStrings.Grenade));
        StartCoroutine(_pool.InitializePool(ConstantStrings.Shuriken));
        StartCoroutine(_pool.InitializePool(ConstantStrings.FuumaShuriken));
    }

    public GameObject GetProjectile(GameObject projectile)
    {
        return _pool.GetObject(projectile);
    }

    public void HandleOnReturnProjectileToPool(GameObject projectile)
    {
        _pool.ReturnGameObject(projectile);
    }
}
