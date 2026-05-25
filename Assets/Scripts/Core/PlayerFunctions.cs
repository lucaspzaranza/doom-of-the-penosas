using UnityEngine;

public static class PlayerFunctions
{
    /// <summary>
    /// Checks if the DamageableObject is a Player or if it's a Ride Armor with some player inside.
    /// </summary>
    /// <param name="dmgObject"></param>
    /// <returns></returns>
    public static bool DamageableObjectIsPlayer(DamageableObject dmgObject)
    {
        return
            dmgObject.TryGetComponent(out IPlayerCharacter penosa) || // Is it a player?
            (dmgObject.TryGetComponent(out IRideArmor rideArmor) &&  // Or is it a Ride Armor 
            rideArmor.Player != null);                              // with some player inside?
    }
}
