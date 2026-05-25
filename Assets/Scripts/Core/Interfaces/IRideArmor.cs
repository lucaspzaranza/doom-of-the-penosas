using SharedData.Enumerations;
using System;

public interface IRideArmor
{
    event Action<int> OnRideArmorLifeChanged;

    RideArmorType Type { get; }
    IPlayerCharacter Player { get; }
    bool Required { get; set; }
    public int Life { get; set; }

    void Equip(IPlayerCharacter player, IPlayerController playerController);
    void Eject();
}