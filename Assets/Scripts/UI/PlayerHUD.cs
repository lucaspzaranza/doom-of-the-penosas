using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Text _nameTxt = null;
    [SerializeField] private Text _livesTxt = null;
    [SerializeField] private Image _lifebarImg = null;
    [SerializeField] private Image _armorLifebarImg = null;
    [SerializeField] private Image _iconImg = null;
    [SerializeField] private Image _specialItemImg = null;
    [SerializeField] private Penosa player;
    [SerializeField] private GameObject _hudContainer = null;

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

    public void SetSpecialItemIconSprite(Sprite newSprite)
    {
        _specialItemImg.sprite = newSprite;
        _specialItemImg.gameObject.SetActive(newSprite != null);
    }

    public void SetHUDValues()
    {
        Name = player.PlayerData.name;
        Lives = player.PlayerData.Lives;
        Life = player.PlayerData.Life;
        ArmorLife = player.PlayerData.ArmorLife;
    }

    private void OnEnable()
    {
        player.PlayerData.OnArmorLifeChanged += newValue => ArmorLife = newValue;
        player.PlayerData.OnLifeChanged += newValue => Life = newValue;
        player.PlayerData.OnLivesChanged += newValue => Lives = newValue;

        SetHUDValues();
    }

    private void Start()
    {
        player.Inventory.OnSpecialItemIconChanged += SetSpecialItemIconSprite;
    }
}