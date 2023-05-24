using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Penosa player;
    [SerializeField] private Text _nameTxt = null;
    [SerializeField] private Text _livesTxt = null;
    [SerializeField] private Image _lifebarImg = null;
    [SerializeField] private Image _armorLifebarImg = null;
    [SerializeField] private Image _specialItemImg = null;
    [SerializeField] private GameObject _hudContainer = null;
    [SerializeField] private byte playerID;
    [Header("Ammo Text")]
    [SerializeField] private TMP_Text PrimaryWeaponText;
    [SerializeField] private TMP_Text PrimaryWeaponAmmoText;
    [SerializeField] private TMP_Text SecondaryWeaponText;
    [SerializeField] private TMP_Text SecondaryWeaponAmmoText;

    private string _name;
    private int _lives;
    private int _life;
    private int _armorLife;

    public GameObject HUDContainer => _hudContainer;

    public int Life
    {
        get => _life;
        set
        {
            _life = value;
            float res = _life;
            _lifebarImg.fillAmount = res / 100;
        }
    }

    public int ArmorLife
    {
        get => _armorLife;
        set
        {
            _armorLife = value;
            float res = _armorLife;
            _armorLifebarImg.fillAmount = res / 100;
        }
    }

    public int Lives
    {
        get => _lives;
        set
        {
            _lives = value;
            _livesTxt.text = $"x{value.ToString()}";
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _nameTxt.text = _name;
        }
    }

    public Penosa Player
    {
        get => player;
        set => player = value;
    }

    public void SetSpecialItemIconSprite(Sprite newSprite)
    {
        _specialItemImg.sprite = newSprite;
        _specialItemImg.gameObject.SetActive(newSprite != null);
    }

    public void SetHUDValues()
    {
        Name = player.PlayerData.Character.ToString();
        Lives = player.PlayerData.Lives;
        Life = player.PlayerData.Life;
        ArmorLife = player.PlayerData.ArmorLife; 
    }

    private void OnEnable()
    {
        if(player == null)
        {
            var players = FindObjectsOfType<Penosa>().OrderBy(penosa => penosa.PlayerData.LocalID).ToArray();
            player = players.Length > 0? players[playerID] : null;
        }

        if (player == null) return;

        player.PlayerData.OnArmorLifeChanged += newValue => ArmorLife = newValue;
        player.PlayerData.OnLifeChanged += newValue => Life = newValue;
        player.PlayerData.OnLivesChanged += newValue => Lives = newValue;
        player.PlayerData.OnWeaponLevelChanged += UpdateWeaponLevelText;
        player.PlayerData.OnWeaponAmmoChanged += UpdateWeaponAmmoText;

        SetHUDValues();
    }

    private void OnDisable()
    {
        if (player == null) return;
        player.PlayerData.OnWeaponLevelChanged -= UpdateWeaponLevelText;
        player.PlayerData.OnWeaponAmmoChanged -= UpdateWeaponAmmoText;
    }

    private void Start()
    {
        if (player == null) return;
        player.Inventory.OnSpecialItemIconChanged += SetSpecialItemIconSprite;
    }

    public void UpdateWeaponLevelText(WeaponType weaponType, int newLvl)
    {
        if (weaponType == WeaponType.Primary)
            PrimaryWeaponText.text = $"1st  Lvl {newLvl}:";
        else //Secondary
            SecondaryWeaponText.text = $"2nd Lvl {newLvl}:";
    }

    private void UpdateWeaponAmmoText(WeaponType weaponType, int newAmmo)
    {
        if (weaponType == WeaponType.Primary)
            PrimaryWeaponAmmoText.text = player.PlayerData._1stWeaponLevel > 1? newAmmo.ToString() : "---";
        else //Secondary
            SecondaryWeaponAmmoText.text = newAmmo.ToString();
    }
}