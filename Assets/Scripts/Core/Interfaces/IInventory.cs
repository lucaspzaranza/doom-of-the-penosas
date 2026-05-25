using SharedData.Enumerations;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory
{
    event Action<IInventoryListItem> OnInventorySpecialItemAdded;
    event Action OnInventoryCleared;
    event Action<Sprite> OnSpecialItemIconChanged;

    float JetCopterGravity { get; }
    IItemSlot SelectedSlot { get; set; }
    IReadOnlyList<IItemSlot> Slots { get; }
    bool IsEmpty { get; }
    Transform Transform { get; }
    GameObject GameObject { get; }
    GameObject SlotGameObject { get; }

    void LoadInventoryData(IInventoryData inventoryData);
    void ClearInventory();
    void AddItem(SpecialItemType itemType, byte amount, IPlayerCharacter player, Sprite itemSprite);
    void SelectItem(int index);
    void ShowSlot();
    void ResetTemporizerCounter();
}