using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SharedData.Enumerations;
using System.Linq;

public class Inventory : MonoBehaviour, IInventory
{
    public event Action<Sprite> OnSpecialItemIconChanged;
    public event Action<IInventoryListItem> OnInventorySpecialItemAdded;
    public event Action OnInventoryCleared;

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
    private IItemSlot _selectedSlot = null;

    public byte ItemEffectDuration => itemEffectDuration;
    public float JetCopterGravity => jetCopterGravity;
    public float AdrenalineSpeedEnhancingRate => adrenalineSpeedEnhancindRate;
    public GameObject MissilePrefab => missilePrefab;

    public IReadOnlyList<IItemSlot> Slots => _slots;
    public bool IsEmpty => Slots.Count == 0;

    public GameObject GameObject => gameObject;
    public GameObject SlotGameObject => slotGameObject;

    public IItemSlot SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value as ItemSlot;
            _selectedItem = _selectedSlot == null ? null : _selectedSlot.Item as SpecialItem;
        }
    }

    public Transform Transform => transform;

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

    public void ResetTemporizerCounter()
    {
        temporizerTimeCounter = 0;
    }

    public void ClearInventory()
    {
        _slots.RemoveRange(0, Slots.Count);
        SelectedSlot = null;
        SelectSlotSprite(null);
        itemAmount.text = "0";
        OnInventoryCleared?.Invoke();
    }

    public void ShowSlot()
    {
        slotGameObject.SetActive(true);
        temporizerTimeCounter = 0;
    }

    public void AddItem(SpecialItemType itemType, byte amount, IPlayerCharacter player, Sprite itemSprite)
    {
        var matchSlot = _slots.Find(slot => slot.Item == null || slot.Item.ItemType == itemType);

        if (matchSlot != null) // already has an item
            SetItemAmount(matchSlot, (byte)(matchSlot.Amount + amount));
        else // add new item
        {
            SpecialItem specialItem = (SpecialItem)GetComponent(itemType.ToString());
            var newSlot = new ItemSlot(specialItem, amount, player, itemSprite);
            _slots.Add(newSlot);
            if (Slots.Count == 1)
            {
                SelectedSlot = newSlot;
                SelectSlotSprite(_slots[0].Sprite);
                ShowSlot();
            }
            SetItemAmount(newSlot, amount);
        }
    }

    public void LoadInventoryData(IInventoryData inventoryData)
    {
        if (inventoryData == null || inventoryData.SpecialItems == null)
            return;

        foreach (var item in inventoryData.SpecialItems)
        {
            AddItem(item.SpecialItemType, item.Amount, inventoryData.Player, item.ItemSprite);
        }
    }

    private void SetItemAmount(ItemSlot itemToUpdate, byte amount)
    {
        itemToUpdate.Amount = amount;
        if (itemToUpdate.Equals(SelectedSlot))
            itemAmount.text = amount.ToString();

        OnInventorySpecialItemAdded?.Invoke(
            new InventoryListItem(itemToUpdate.Item.ItemType, amount, itemToUpdate.Sprite));
    }

    public void DecreaseItemAmount(ItemSlot slot)
    {
        SetItemAmount(slot, (byte)(slot.Amount - 1));
        ShowSlot();
    }

    public void RemoveItem(ItemSlot slot)
    {
        int index = _slots.IndexOf(slot);
        _slots.Remove(slot);
        //Destroy(slot.Item);
        int newIndex = index == Slots.Count ? index - 1 : index;
        SelectedSlot = IsEmpty ? null : Slots[newIndex];
        SelectSlotSprite(IsEmpty ? null : _slots[newIndex].Sprite);
        itemAmount.text = IsEmpty ? "0" : SelectedSlot.Amount.ToString();
    }

    public void SelectItem(int index)
    {
        SelectedSlot = _slots[index];
        SelectSlotSprite(_slots[index].Sprite);
        itemAmount.text = SelectedSlot.Amount.ToString();
    }
    public void SelectSlotSprite(Sprite newSprite)
    {
        currentItemSprite = newSprite;
        itemChildSR.sprite = newSprite;

        OnSpecialItemIconChanged?.Invoke(currentItemSprite);
    }
}