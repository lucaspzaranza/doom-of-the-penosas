using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour, IController
{
    [SerializeField] private GameObject _poolPrefab;

    private GameObject _poolInstance;

    public ObjectPool Pool => _pool;
    [SerializeField] private ObjectPool _pool;

    public void Setup()
    {
        _poolInstance = Instantiate(_poolPrefab);
        _pool = _poolInstance.GetComponent<ObjectPool>();

        InitiateProjectilesPools();
    }

    public void Dispose()
    {
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
}
