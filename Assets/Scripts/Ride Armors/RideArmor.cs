using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideArmor : MonoBehaviour
{
    [SerializeField]
    private float _upperLimitDegree = 90f;

    [SerializeField]
    private float _lowerLimitDegree = 345f;

    [SerializeField] private RideArmorType _type;
    public RideArmorType RideArmorType => _type;

    [SerializeField] private Penosa _player;
    public Penosa Player => _player;

    [SerializeField] private int _health;
    public int Health => _health;

    protected Rigidbody2D _rb;
    public Rigidbody2D RigiBody2DComponent => _rb;

    [SerializeField] protected GameObject _cannon;
    public GameObject Cannon => _cannon;

    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed => _rotationSpeed;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Move(Vector2 direction)
    {
        
    }

    public virtual void Jump()
    {

    }

    public virtual void Aim(float direction)
    {
        Cannon.transform.Rotate(Vector3.forward * direction * _rotationSpeed * Time.deltaTime);

        float eulerZ = _cannon.transform.localEulerAngles.z;

        if (direction == 1 && eulerZ > _upperLimitDegree && eulerZ < _lowerLimitDegree)
            Cannon.transform.localEulerAngles = new Vector3(0f, 0f,
                Mathf.Clamp(eulerZ, 0f, _upperLimitDegree));
        else if (direction == -1 && eulerZ > _upperLimitDegree && eulerZ < 360f && eulerZ < _lowerLimitDegree)
            Cannon.transform.localEulerAngles = new Vector3(0f, 0f, _lowerLimitDegree);
    }

    public virtual void Shoot()
    {

    }

    public void Equip(Penosa player)
    {
        _player = player;
    }

    public virtual void Eject()
    {
        _player = null;
    }

    public void TakeDamage(int damage)
    {
        _health -= Mathf.Clamp(damage, 0, PlayerConsts.Max_Life);
    }
}
