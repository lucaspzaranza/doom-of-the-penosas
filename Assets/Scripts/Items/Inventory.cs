using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedData.Enumerations;

public class Inventory : MonoBehaviour
{
    #region Vars

    [SerializeField] private SpecialItem _selectedItem;
    [SerializeField] private List<ItemSlot> _slots = null;
    
    [Header("Inventory Data")]
    [SerializeField] private byte itemEffectDuration;
    [SerializeField] private int itemTimeCounter;
    [SerializeField] private float jetCopterGravity;
    [SerializeField] private float adrenalineSpeedEnhancindRate;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private SpriteRenderer itemChildSR = null;
    [SerializeField] private Text itemAmount;
    [SerializeField] private float slotTemporizer;
    [SerializeField] private GameObject slotGameObject;

    private float temporizerTimeCounter;
    private Sprite currentItemSprite;
    private ItemSlot _selectedSlot = null;

    #endregion

    #region Props

    public byte ItemEffectDuration => itemEffectDuration;
    public float JetCopterGravity => jetCopterGravity;
    public float AdrenalineSpeedEnhancingRate => adrenalineSpeedEnhancindRate;
    public GameObject MissilePrefab => missilePrefab;

    public List<ItemSlot> Slots => _slots;
    public bool IsEmpty => Slots.Count == 0;

    public ItemSlot SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value;
            _selectedItem = _selectedSlot == null ? null : _selectedSlot.Item;
        }
    }

    #endregion

    public delegate void PlayerDataSpriteEvent(Sprite newSprite);
    public event PlayerDataSpriteEvent OnSpecialItemIconChanged;

    void FixedUpdate()
    {
        if (slotGameObject.activeSelf)
        {
            temporizerTimeCounter += Time.deltaTime;
            if (temporizerTimeCounter >= slotTemporizer)
            {
                temporizerTimeCounter = 0;
                slotGameObject.SetActive(false);
            }
        }
    }

    public void ClearInventory()
    {
        var items = new List<SpecialItem>(GetComponents<SpecialItem>());
        Slots.RemoveRange(0, Slots.Count);
        items.ForEach(item => Destroy(item));
        SelectedSlot = null;
        SelectSlotSprite(null);
        itemAmount.text = "0";
    }

    public void ShowSlot()
    {
        slotGameObject.SetActive(true);
        temporizerTimeCounter = 0;
    }

    public void AddItem(SpecialItemType itemType, byte amount, Penosa player, Sprite itemSprite)
    {
        //var matchSlot = Slots.Find(slot => slot.Item == null || slot.Item.GetType().ToString() == typeString);
        var matchSlot = Slots.Find(slot => slot.Item == null || slot.Item.ItemType == itemType);

        if (matchSlot != null) // already has an item
            SetItemAmount(matchSlot, (byte)(matchSlot.Amount + amount));
        else // add new item
        {
            SpecialItem specialItem = (SpecialItem)GetComponent(itemType.ToString());
            var newSlot = new ItemSlot(specialItem, amount, player, itemSprite);
            Slots.Add(newSlot);
            if (Slots.Count == 1)
            {
                SelectedSlot = newSlot;
                SelectSlotSprite(Slots[0].Sprite);
                ShowSlot();
            }
            SetItemAmount(newSlot, amount);
        }
    }

    private void SetItemAmount(ItemSlot item, byte amount)
    {
        item.Amount = amount;
        if (item.Equals(SelectedSlot))
            itemAmount.text = amount.ToString();
    }

    public void DecreaseItemAmount(ItemSlot slot)
    {
        SetItemAmount(slot, (byte)(slot.Amount - 1));
        ShowSlot();
    }

    public void RemoveItem(ItemSlot slot)
    {
        int index = Slots.IndexOf(slot);
        Slots.Remove(slot);
        //itemSprites.RemoveAt(index);
        Destroy(slot.Item);
        int newIndex = index == Slots.Count ? index - 1 : index;
        SelectedSlot = IsEmpty ? null : Slots[newIndex];
        //SelectSlotSprite(IsEmpty ? null : itemSprites[newIndex]);
        SelectSlotSprite(IsEmpty ? null : Slots[newIndex].Sprite);
        itemAmount.text = IsEmpty ? "0" : SelectedSlot.Amount.ToString();
    }

    public void SelectItem(int index)
    {
        SelectedSlot = Slots[index];
        SelectSlotSprite(Slots[index].Sprite);
        itemAmount.text = SelectedSlot.Amount.ToString();
    }

    public void SelectSlotSprite(Sprite newSprite)
    {
        currentItemSprite = newSprite;
        itemChildSR.sprite = newSprite;

        OnSpecialItemIconChanged?.Invoke(currentItemSprite);
    }
}