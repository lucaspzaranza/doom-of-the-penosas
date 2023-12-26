using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideArmor : MonoBehaviour
{
    [SerializeField] private RideArmorType _type;
    public RideArmorType RideArmorType => _type;

    [SerializeField] private Penosa _player;
    public Penosa Player => _player;

    [SerializeField] private int _health;
    public int Health => _health;

    protected Rigidbody2D _rb;
    public Rigidbody2D RigiBody2DComponent => _rb;

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
