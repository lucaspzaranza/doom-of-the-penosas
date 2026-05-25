using System.Collections.Generic;

public interface IInventoryData
{
    IReadOnlyList<IInventoryListItem> SpecialItems { get; }
    IPlayerCharacter Player { get; }

    void UpdateData(IInventoryListItem inventoryListItem);
    void ClearInventoryData();
}