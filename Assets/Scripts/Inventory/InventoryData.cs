using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventoryListItem
{
    public InventoryListItem(SpecialItemType itemType, byte amount, Sprite sprite)
    {
        SpecialItemType = itemType;
        Amount = amount;
        ItemSprite = sprite;
    }

    public SpecialItemType SpecialItemType;
    public byte Amount;
    public Sprite ItemSprite;
}

[Serializable]
public class InventoryData
{
    [SerializeField] private List<InventoryListItem> _specialItems;
    public List<InventoryListItem> SpecialItems => _specialItems;

    [SerializeField] private Penosa _player;
    public Penosa Player => _player;

    public InventoryData(Penosa player)
    {
        _player = player;
        _specialItems = new List<InventoryListItem>();
    }

    public void SetPlayer(Penosa player)
    {
        _player = player;
    }

    public void UpdateData(InventoryListItem inventoryListItem)
    {
        if(_specialItems == null)
            _specialItems = new List<InventoryListItem>();

        var itemData = _specialItems.SingleOrDefault(item => item.SpecialItemType == inventoryListItem.SpecialItemType);

        if (itemData != null)
        {
            itemData.Amount = inventoryListItem.Amount;
            if(itemData.Amount <= 0)
                _specialItems.Remove(itemData);
        }
        else
            _specialItems.Add(inventoryListItem);
    }
}