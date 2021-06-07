using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class PlayerHUD
{
    [SerializeField] private string _name;
    [SerializeField] private byte _id;
    [SerializeField] private int _lives;
    [SerializeField] private int _life;
    [SerializeField] private int _armorLife;
    [SerializeField] private Text _nameTxt = null;
    [SerializeField] private Text _livesTxt = null;
    [SerializeField] private Image _lifebarImg = null;
    [SerializeField] private Image _armorLifebarImg = null;
    [SerializeField] private Image _iconImg = null;
    [SerializeField] private Image _specialItemImg = null;
    [SerializeField] private GameObject _hudContainer = null;
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

    public byte ID => _id;
    public void SetSpecialItemIconSprite(Sprite newSprite)
    {
        _specialItemImg.sprite = newSprite;
        _specialItemImg.gameObject.SetActive(newSprite != null);
    }
    public void GetHUDValues(PlayerData playerData)
    {
        _id = playerData.ID;
        Name = playerData.name;
        Lives = playerData.Lives;
        Life = playerData.Life;
        ArmorLife = playerData.ArmorLife;
    }
}
