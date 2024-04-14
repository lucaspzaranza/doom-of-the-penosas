using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyWeapon", menuName = "ScriptableObjects/EnemyWeapon")]
public class EnemyWeaponUnit : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [SerializeField] private GameObject _projectile;
    public GameObject Projectile => _projectile;

    [SerializeField] private Transform _shotSpawn;
    public Transform ShowSpawn => _shotSpawn;

    [Tooltip("Check this if this weapon has continuous fire, like a machinegun, for example.")]
    [SerializeField] private bool _isContinuous;
    public bool IsContinuous => _isContinuous;

    [Tooltip("It'll be used only when the \"Is Continuous\" checkbox be marked.")]
    [DrawIfBoolEqualsTo("_isContinuous", comparedValue: true, elseDrawItDisabled: true)]
    [SerializeField] private float _continuousRate;
    public float ContinuousRate => _continuousRate;

    [SerializeField] private int _damageHit;
    public int DamageHit => _damageHit;

    public void Shoot(Vector2 coordinates)
    {

    }
}
