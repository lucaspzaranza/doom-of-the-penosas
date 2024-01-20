using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireMonoWheel : RideArmor
{
    public override void Move(Vector2 direction)
    {
        if (SharedFunctions.HitWall(_wallCheckCollider, _terrainLayerMask, out Collider2D hitWall))
            return;

        RigiBody2DComponent.velocity = new Vector2(direction.x, RigiBody2DComponent.velocity.y);
        if (direction.x != 0)
            base.Move(direction);
    }

    public override void Eject()
    {
        SetEquippedPlayerActivation(Player, false);

        base.Eject();
    }
}
