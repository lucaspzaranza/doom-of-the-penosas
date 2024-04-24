using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyWeapon", menuName = "ScriptableObjects/EnemyWeapon")]
public class EnemyWeaponUnit : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private byte _id;
    public int ID => _id;

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    [Tooltip("Check this if this weapon will use pooling to get the projectiles. " +
    "If unchecked, it'll use GameObject Instantiating instead.")]
    [SerializeField] private bool _usePooling;
    public bool UsePooling => _usePooling;

    [SerializeField] private GameObject _projectile;
    public GameObject Projectile => _projectile;

    [Tooltip("Check this if this weapon has continuous fire, like a machinegun, for example.")]
    [SerializeField] private bool _isContinuous;
    public bool IsContinuous => _isContinuous;

    [Tooltip("It'll be used only when the \"Is Continuous\" checkbox be marked.")]
    [DrawIfBoolEqualsTo("_isContinuous", comparedValue: true, elseDrawItDisabled: true)]
    [SerializeField] private float _continuousRate;
    public float ContinuousRate => _continuousRate;

    [SerializeField] private int _damageHit;
    public int DamageHit => _damageHit;

    private GameController _gameControllerInstance;

    private void OnEnable()
    {
        _gameControllerInstance = FindObjectOfType<GameController>();
    }

    private void OnDisable()
    {
        _gameControllerInstance = null;
    }

    public void Shoot(Vector2 coordinates, int currentDirection)
    {
        GameObject newProjectile = null;

        if(UsePooling) 
        {
            // Do the pooling
            if (_gameControllerInstance != null)
            {
                newProjectile = _gameControllerInstance.GetProjectileFromPool(_projectile);
                newProjectile.transform.position = coordinates;
            }
            else
            {
                Debug.LogWarning($"{_projectile.name} not found on Object Pool.");
                return;
            }
        }
        else
            newProjectile = Instantiate(Projectile, coordinates, Quaternion.identity);

        newProjectile.transform.localScale =
                new Vector2(newProjectile.transform.localScale.x * currentDirection,
                newProjectile.transform.localScale.y);

        var projectileScript = newProjectile.GetComponent<Projectile>();
        projectileScript.Speed = Mathf.Abs(projectileScript.Speed) * currentDirection;

        if (newProjectile.TryGetComponent(out EnemyProjectile enemyProjectile))
            enemyProjectile.SetEnemyWeaponUnit(this);
    }
}
