using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventoryListItem
{
    public InventoryListItem(SpecialItemType itemType, byte amount)
    {
        SpecialItemType = itemType;
        Amount = amount;
    }

    public SpecialItemType SpecialItemType;
    public byte Amount;
}

[Serializable]
public class InventoryData
{
    [SerializeField] private List<InventoryListItem> _specialItems;
    public List<InventoryListItem> SpecialItems => _specialItems;

    public void UpdateData(InventoryListItem inventoryListItem)
    {
        if(_specialItems == null)
            _specialItems = new List<InventoryListItem>();

        var itemData = _specialItems.SingleOrDefault(item => item.SpecialItemType == inventoryListItem.SpecialItemType);

        if (itemData != null)
            itemData.Amount = inventoryListItem.Amount;
        else
            _specialItems.Add(inventoryListItem);
    }
}