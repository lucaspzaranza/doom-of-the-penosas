using SharedData.Enumerations;

public interface ISpecialItem
{
    bool ItemInUse { get; }
    public SpecialItemType ItemType { get; }

    void Use();
}