using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyWeapon", menuName = "ScriptableObjects/EnemyWeapon")]
public class EnemyWeaponUnit : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Image _image;
    public Image Image => _image;

    [SerializeField] private GameObject _projectile;
    public GameObject Projectile => _projectile;

    [Tooltip("Check this if this weapon has continuous fire, like a machinegun, for example.")]
    [SerializeField] private bool _isContinuous;
    public bool IsContinuous => _isContinuous;

    [SerializeField] private float _continuousRate;
    public float ContinuousRate => _continuousRate;

    [SerializeField] private int _damageHit;
    public int DamageHit => _damageHit;

    public void Shoot(Vector2 coordinates)
    {

    }
}
