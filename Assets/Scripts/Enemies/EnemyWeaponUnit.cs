using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyWeapon", menuName = "ScriptableObjects/EnemyWeapon")]
public class EnemyWeaponUnit : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [Tooltip("Check this if this weapon will use pooling to get the projectiles. " +
    "If unchecked, it'll use GameObject Instantiating instead.")]
    [SerializeField] private bool _usePooling;
    public bool UsePooling => _usePooling;

    [SerializeField] private GameObject _projectile;
    public GameObject Projectile => _projectile;

    [SerializeField] private float _attackDuration;
    public float AttackDuration => _attackDuration;

    [Tooltip("Check this if this weapon has continuous fire, like a machinegun, for example.")]
    [SerializeField] private bool _isContinuous;
    public bool IsContinuous => _isContinuous;

    [Tooltip("It'll be used only when the \"Is Continuous\" option be unchecked.")]
    [DrawIfBoolEqualsTo("_isContinuous", comparedValue: false, elseDrawItDisabled: true)]
    [SerializeField] private float _fireRate;
    public float FireRate => _fireRate;

    [Tooltip("It'll be used only when the \"Is Continuous\" option be marked.")]
    [DrawIfBoolEqualsTo("_isContinuous", comparedValue: true, elseDrawItDisabled: true)]
    [SerializeField] private float _continuousFireRate;
    public float ContinuousFireRate => _continuousFireRate;    

    [SerializeField] private int _damageHit;
    public int DamageHit => _damageHit;

    private GameController _gameControllerInstance;

    private void OnEnable()
    {
        _gameControllerInstance = FindObjectOfType<GameController>();
    }

    public float GetFireRate() => IsContinuous ? ContinuousFireRate : FireRate;

    public void Shoot(Transform spawnTransform, int currentDirection)
    {
        GameObject newProjectile = null;
        Vector2 coordinates = spawnTransform.position;
        Quaternion newRotation = spawnTransform.rotation;

        if (UsePooling) 
        {
            if(_gameControllerInstance == null)
                _gameControllerInstance = FindObjectOfType<GameController>();

            if (_gameControllerInstance != null)
            {
                newProjectile = _gameControllerInstance.GetProjectileFromPool(_projectile);
                newProjectile.transform.position = coordinates;
                newProjectile.transform.rotation = newRotation;
            }
            else
            {
                WarningMessages.EnemyWeaponUnitCouldNotFoundGameControllerInstance();
                return;
            }
        }
        else
            newProjectile = Instantiate(Projectile, coordinates, newRotation);

        newProjectile.transform.localScale =
                new Vector2(newProjectile.transform.localScale.x * currentDirection,
                newProjectile.transform.localScale.y);

        var projectileScript = newProjectile.GetComponent<Projectile>();
        projectileScript.Speed = Mathf.Abs(projectileScript.Speed) * currentDirection;

        if (newProjectile.TryGetComponent(out EnemyProjectile enemyProjectile))
            enemyProjectile.SetEnemyWeaponUnit(this);
    }
}
