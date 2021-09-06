using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    #region Variables

    public string name;
    [SerializeField] private Penosa _player = null;
    [SerializeField] private byte _ID;
    [SerializeField] private GameObject _gameObject = null;
    [SerializeField] private float _countdown;
    [SerializeField] private int _continues;
    [SerializeField] [Range(0, PlayerConsts.max_lives)] private int _lives;
    [SerializeField] [Range(0, PlayerConsts.max_life)] private int _life;
    [SerializeField] [Range(0, PlayerConsts.max_life)] private int _armorLife;
    [SerializeField] [Range(1, PlayerConsts._1stWeaponMaxLvl)] private byte __1stWeaponLvl;
    [SerializeField] [Range(1, PlayerConsts._2ndWeaponMaxLvl)] private byte __2ndWeaponLvl;
    [SerializeField] private int __1stWeaponAmmo;
    [SerializeField] private int __2ndWeaponAmmo;
    public GameObject[] _1stShot;
    public GameObject[] _2ndShot;

    #endregion

    #region Events

    public delegate void PlayerDataEvent(int newValue);
    public delegate void PlayerAmmoDataEvent(WeaponType weaponType, int newValue);
    public event PlayerDataEvent OnLifeChanged;
    public event PlayerDataEvent OnArmorLifeChanged;
    public event PlayerDataEvent OnLivesChanged;
    public event PlayerAmmoDataEvent OnWeaponLevelChanged;
    public event PlayerAmmoDataEvent OnWeaponAmmoChanged;

    #endregion

    #region Properties

    public int _1stWeaponAmmo
    {
        get => __1stWeaponAmmo;
        set
        {
            __1stWeaponAmmo = Mathf.Clamp(value, 0, PlayerConsts.maxAmmo);
            if (__1stWeaponAmmo == 0) _1stWeaponLevel = 1;
            OnWeaponAmmoChanged?.Invoke(WeaponType.Primary, __1stWeaponAmmo);
        }
    }

    public byte _1stWeaponLevel
    {
        get => __1stWeaponLvl;
        set 
        {
            if (value <= PlayerConsts._1stWeaponMaxLevel)
            {
                __1stWeaponLvl = value;
                OnWeaponLevelChanged?.Invoke(WeaponType.Primary, __1stWeaponLvl);
            }
        }
    }

    public int _2ndWeaponAmmo
    {
        get => __2ndWeaponAmmo;
        set
        {
            if (value <= PlayerConsts.maxAmmo && value >= 0)
            {
                __2ndWeaponAmmo = value;
                if (__2ndWeaponAmmo == 0) _2ndWeaponLevel = 1;
                OnWeaponAmmoChanged?.Invoke(WeaponType.Secondary, __2ndWeaponAmmo);
            }
        }
    }

    public byte _2ndWeaponLevel
    {
        get => __2ndWeaponLvl;
        set 
        { 
            if (value <= PlayerConsts._2ndWeaponMaxLevel)
            {
                __2ndWeaponLvl = value;
                OnWeaponLevelChanged?.Invoke(WeaponType.Secondary, __2ndWeaponLvl);
            }
        }
    }

    public int ArmorLife
    {
        get => _armorLife;
        set
        {
            _armorLife = Mathf.Clamp(value, 0, PlayerConsts.max_life);
            OnArmorLifeChanged?.Invoke(_armorLife);
            if (value < 0) Life -= (Mathf.Abs(value));
        }
    }

    public float Countdown
    {
        get => _countdown;
        set => _countdown = value;
    }

    public int Continues
    {
        get => _continues;
        set => _continues = value;
    }

    public GameObject Current1stShot
    {
        get
        {
            byte index = _1stWeaponLevel < 3 && _1stWeaponLevel > 0 ? (byte)0 : (byte)1;
            return _1stShot[index];
        }
    }

    public GameObject Current2ndShot => _2ndShot[_2ndWeaponLevel - 1];

    public GameObject GameObject => _gameObject;

    public byte ID { get => _ID; set => _ID = value; }

    public int Life
    {
        get => _life;
        set
        {
            _life = Mathf.Clamp(value, 0, PlayerConsts.max_life);
            OnLifeChanged?.Invoke(_life);
            if (_life == 0 && !Player.Adrenaline && !Player.IsBlinking)
            {
                Lives--;
                Player.Death();
            }
        }
    }

    public int Lives
    {
        get => _lives;
        set
        {
            _lives = Mathf.Clamp(value, 0, PlayerConsts.max_lives);
            OnLivesChanged?.Invoke(_lives);
        }
    }

    public bool OnCountdown { get; set; }

    public Penosa Player => _player;

    #endregion
}