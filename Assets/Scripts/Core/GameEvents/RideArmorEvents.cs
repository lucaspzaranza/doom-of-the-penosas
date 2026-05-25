using SharedData.Enumerations;
using System;

public static class RideArmorEvents
{
    public static Action<IRideArmor> OnRideArmorEquipped;
    public static Action<RideArmorType, bool> OnRideArmorChangedRequired;
}