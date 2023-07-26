using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SharedData.Enumerations;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Linq;
using System.Drawing.Printing;

[Serializable]
public class PlayerData
{
    public Action<int> OnLifeChanged;
    public Action<int> OnArmorLifeChanged;
    public Action<int> OnLivesChanged;
    public Action<WeaponType, int> OnWeaponLevelChanged;
    public Action<WeaponType, int> OnWeaponAmmoChanged;

    [SerializeField] private Penosas _character;
    [SerializeField] private InputDevice _inputDevice;
    [SerializeField] private Penosa _playerScript = null;
    [SerializeField] private byte _localID;
    [SerializeField] private GameObject _playerGameObject = null;
    [SerializeField] private int _continues;
    [SerializeField] [Range(0, PlayerConsts.Max_Lives)] private int _lives;
    [SerializeField] [Range(0, PlayerConsts.Max_Life)] private int _life;
    [SerializeField] [Range(0, PlayerConsts.Max_Life)] private int _armorLife;
    [SerializeField] [Range(1, PlayerConsts._1stWeaponMaxLvl)] private byte _1stWeaponLvl;
    [SerializeField] [Range(1, PlayerConsts._2ndWeaponMaxLvl)] private byte _2ndWeaponLvl;
    [SerializeField] private int _1stWeaponAmmo;
    [SerializeField] private int _2ndWeaponAmmo;
    [SerializeField] private List<GameObject> _1stShot;
    [SerializeField] private List<GameObject> _2ndShot;
    [SerializeField] private InventoryData _inventoryData;
    [SerializeField] private bool _gameOver;

    public bool GameOver
    {
        get => _gameOver;
        set
        {
            _gameOver = value;
            if (_gameOver)
                Player.enabled = false;
        }
    }

    public int _1stWeaponAmmoProp
    {
        get => _1stWeaponAmmo;
        set
        {
            _1stWeaponAmmo = Mathf.Clamp(value, 0, PlayerConsts.MaxAmmo);
            if (_1stWeaponAmmo == 0) _1stWeaponLevel = 1;
            OnWeaponAmmoChanged?.Invoke(WeaponType.Primary, _1stWeaponAmmo);
        }
    }

    public byte _1stWeaponLevel
    {
        get => _1stWeaponLvl;
        set 
        {
            if (value <= PlayerConsts._1stWeaponMaxLevel)
            {
                _1stWeaponLvl = value;
                OnWeaponLevelChanged?.Invoke(WeaponType.Primary, _1stWeaponLvl);
            }
        }
    }

    public int _2ndWeaponAmmoProp
    {
        get => _2ndWeaponAmmo;
        set
        {
            if (value <= PlayerConsts.MaxAmmo && value >= 0)
            {
                _2ndWeaponAmmo = value;
                if (_2ndWeaponAmmo == 0) _2ndWeaponLevel = 1;
                OnWeaponAmmoChanged?.Invoke(WeaponType.Secondary, _2ndWeaponAmmo);
            }
        }
    }

    public byte _2ndWeaponLevel
    {
        get => _2ndWeaponLvl;
        set 
        { 
            if (value <= PlayerConsts._2ndWeaponMaxLevel)
            {
                _2ndWeaponLvl = value;
                OnWeaponLevelChanged?.Invoke(WeaponType.Secondary, _2ndWeaponLvl);
            }
        }
    }

    public int ArmorLife
    {
        get => _armorLife;
        set
        {
            _armorLife = Mathf.Clamp(value, 0, PlayerConsts.Max_Life);
            OnArmorLifeChanged?.Invoke(_armorLife);
            if (value < 0) Life -= (Mathf.Abs(value));
        }
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

    public GameObject PlayerGameObject => _playerGameObject;

    public byte LocalID { get => _localID; set => _localID = value; }

    public int Life
    {
        get => _life;
        set
        {
            _life = Mathf.Clamp(value, 0, PlayerConsts.Max_Life);
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
            _lives = Mathf.Clamp(value, 0, PlayerConsts.Max_Lives);
            OnLivesChanged?.Invoke(_lives);
        }
    }

    public Penosa Player => _playerScript;

    public Penosas Character => _character;

    public InputDevice InputDevice => _inputDevice;

    public InventoryData InventoryData => _inventoryData;

    public PlayerData(Penosas newCharacter, int localID, InputDevice device = null)
    {
        _character = newCharacter;
        _localID = (byte)localID;

        _life = PlayerConsts.Max_Life;
        _armorLife = PlayerConsts.ArmorInitialLife;
        _lives = PlayerConsts.Initial_Lives;

        _1stWeaponLvl = PlayerConsts.WeaponInitialLevel;
        _1stWeaponAmmo = PlayerConsts._1stWeaponInitialAmmo;

        _2ndWeaponLvl = PlayerConsts.WeaponInitialLevel;
        _2ndWeaponAmmo = PlayerConsts._2ndWeaponInitialAmmo;

        _continues = PlayerConsts.Continues;

        _gameOver = false;

        if (device != null)
            _inputDevice = device;
    }

    public void SetProjectilesPrefabs(PlayerDataPrefabs prefabs)
    {
        _1stShot = new List<GameObject>(prefabs.ListOf1stShots);
        _2ndShot = new List<GameObject>(prefabs.ListOf2ndShots);
    }

    public void SetPlayerScriptFromInstance(Penosa playerScript)
    {
        _playerScript = playerScript;
    }

    public void SetPlayerGameObjectFromInstance(GameObject playerGameObject)
    {
        _playerGameObject = playerGameObject;
    }

    public void InventoryDataSetup(Penosa player, bool isNewGame)
    {
        if(isNewGame || _inventoryData == null)
            _inventoryData = new InventoryData(player);
        else
            _inventoryData.SetPlayer(player);
    }
}