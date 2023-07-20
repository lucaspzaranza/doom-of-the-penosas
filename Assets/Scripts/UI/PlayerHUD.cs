using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Penosa player;
    [SerializeField] private TMP_Text _nameTxt = null;
    [SerializeField] private TMP_Text _livesTxt = null;
    [SerializeField] private Image _lifebarImg = null;
    [SerializeField] private Image _armorLifebarImg = null;
    [SerializeField] private Image _specialItemImg = null;
    [SerializeField] private GameObject _hudContainer = null;
    [SerializeField] private byte playerID;

    [Space]
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
            _livesTxt.text = $"x{value}";
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

    public void UpdateHUDValues()
    {
        Name = player.PlayerData.Character.ToString();
        Lives = player.PlayerData.Lives;
        Life = player.PlayerData.Life;
        ArmorLife = player.PlayerData.ArmorLife;

        UpdateWeaponAmmoText(WeaponType.Primary, player.PlayerData._1stWeaponAmmoProp);
        UpdateWeaponLevelText(WeaponType.Primary, player.PlayerData._1stWeaponLevel);

        UpdateWeaponAmmoText(WeaponType.Secondary, player.PlayerData._2ndWeaponAmmoProp);
        UpdateWeaponLevelText(WeaponType.Secondary, player.PlayerData._2ndWeaponLevel);

        if(player.Inventory.SelectedSlot != null)
            SetSpecialItemIconSprite(player.Inventory.SelectedSlot.Sprite);
    }

    public void EventSetup()
    {
        if (player == null)
            return;

        player.PlayerData.OnArmorLifeChanged += UpdateArmorLife;
        player.PlayerData.OnLifeChanged += UpdateLife;
        player.PlayerData.OnLivesChanged += UpdateLives;
        player.PlayerData.OnWeaponLevelChanged += UpdateWeaponLevelText;
        player.PlayerData.OnWeaponAmmoChanged += UpdateWeaponAmmoText;

        player.Inventory.OnSpecialItemIconChanged += SetSpecialItemIconSprite;
    }

    public void EventDispose()
    {
        if (player == null) 
            return;

        player.PlayerData.OnArmorLifeChanged -= UpdateArmorLife;
        player.PlayerData.OnLifeChanged -= UpdateLife;
        player.PlayerData.OnLivesChanged -= UpdateLives;
        player.PlayerData.OnWeaponLevelChanged -= UpdateWeaponLevelText;
        player.PlayerData.OnWeaponAmmoChanged -= UpdateWeaponAmmoText;

        player.Inventory.OnSpecialItemIconChanged -= SetSpecialItemIconSprite;
    }

    public void UpdateWeaponLevelText(WeaponType weaponType, int newLvl)
    {
        if (weaponType == WeaponType.Primary)
            PrimaryWeaponText.text = $"1st  Lvl {newLvl}:";
        else
            SecondaryWeaponText.text = $"2nd Lvl {newLvl}:";
    }

    private void UpdateWeaponAmmoText(WeaponType weaponType, int newAmmo)
    {
        if (weaponType == WeaponType.Primary)
            PrimaryWeaponAmmoText.text = player.PlayerData._1stWeaponLevel > 1? newAmmo.ToString() : "---";
        else
            SecondaryWeaponAmmoText.text = newAmmo.ToString();
    }

    private void UpdateArmorLife(int newValue) => ArmorLife = newValue;
    private void UpdateLife(int newValue) => Life = newValue;
    private void UpdateLives(int newValue) => Lives = newValue;
}