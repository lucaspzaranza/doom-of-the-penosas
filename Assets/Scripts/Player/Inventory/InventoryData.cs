using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventoryListItem : IInventoryListItem
{
    public InventoryListItem(SpecialItemType itemType, byte amount, Sprite sprite)
    {
        SpecialItemType = itemType;
        Amount = amount;
        ItemSprite = sprite;
    }

    private SpecialItemType _specialItemType;
    public SpecialItemType SpecialItemType
    {
        get => _specialItemType;
        set => _specialItemType = value;
    }

    private byte _amount;
    public byte Amount
    {
        get => _amount;
        set => _amount = value;
    }

    [SerializeField] private Sprite _itemSprite;
    public Sprite ItemSprite
    {
        get => _itemSprite;
        set => _itemSprite = value;
    }
}

[Serializable]
public class InventoryData : IInventoryData
{
    [SerializeField] private List<InventoryListItem> _specialItems;
    public IReadOnlyList<IInventoryListItem> SpecialItems => _specialItems;

    [SerializeField] private Penosa _player;
    public IPlayerCharacter Player => _player;

    public InventoryData(IPlayerCharacter player)
    {
        _player = player as Penosa;
        _specialItems = new List<InventoryListItem>();
    }

    public void SetPlayer(IPlayerCharacter player)
    {
        _player = player as Penosa;
    }

    public void AddSpecialItem(IInventoryListItem item)
    {
        _specialItems.Add(item as InventoryListItem);
    }

    public void RemoveSpecialItem(IInventoryListItem item)
    {
        _specialItems.Remove(item as InventoryListItem);
    }

    public void UpdateData(IInventoryListItem inventoryListItem)
    {
        if (SpecialItems == null)
            _specialItems = new List<InventoryListItem>();

        var itemData = SpecialItems.SingleOrDefault(item => item.SpecialItemType == inventoryListItem.SpecialItemType);

        if (itemData != null)
        {
            itemData.Amount = inventoryListItem.Amount;
            if (itemData.Amount <= 0)
                RemoveSpecialItem(itemData);
        }
        else
            AddSpecialItem(inventoryListItem);
    }

    public void ClearInventoryData()
    {
        _specialItems = null;
    }
}