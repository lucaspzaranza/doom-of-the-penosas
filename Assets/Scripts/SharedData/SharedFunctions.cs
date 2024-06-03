using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SharedFunctions
{
    /// <summary>
    /// Considering a 2-Player Multiplayer game, it'll return the complementary index number<br>
    /// with 2 players as a basis. If you pass 0, it'll return 1, and vice versa. </br>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetComplementaryIndex(int value)
    {
        return (value + 1) % 2;
    }

    public static bool HitSomething(Collider2D colliderToCheck, LayerMask layerMask, out Collider2D hitSomething)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        contactFilter.SetLayerMask(layerMask);
        colliderToCheck.OverlapCollider(contactFilter, results);
        hitSomething = results[0];
        return hitSomething != null;
    }

    /// <summary>
    /// Checks if the DamageableObject is a Player or if it's a Ride Armor with some player inside.
    /// </summary>
    /// <param name="dmgObject"></param>
    /// <returns></returns>
    public static bool DamageableObjectIsPlayer(DamageableObject dmgObject)
    {
        return 
            dmgObject.TryGetComponent(out Penosa penosa) ||         // Is a player?
            (dmgObject.TryGetComponent(out RideArmor rideArmor) &&  // Or is it a Ride Armor... 
            rideArmor.Player != null);                              // ... with some player inside?
    }
}
