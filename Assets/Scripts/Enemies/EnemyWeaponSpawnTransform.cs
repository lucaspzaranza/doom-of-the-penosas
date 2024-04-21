using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponSpawnTransform : MonoBehaviour
{
    [SerializeField] private Transform _spawnTransform;
    public Transform SpawnTransform => _spawnTransform;
}
