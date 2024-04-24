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
        foreach (var poolPrefab in _pool.PrefabsList)
        {
            StartCoroutine(_pool.InitializePool(poolPrefab.name));
        }
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
