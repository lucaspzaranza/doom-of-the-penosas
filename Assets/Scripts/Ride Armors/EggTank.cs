using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTank : RideArmor
{
    public override void Move(Vector2 direction)
    {
        base.Move(direction);

        if (SharedFunctions.HitWall(_wallCheckCollider, _terrainLayerMask, out Collider2D hitWall))
            return;

        RigiBody2DComponent.velocity = direction;
        if (direction.x != 0)
            Player.transform.position = new Vector2(transform.position.x, Player.transform.position.y);
    }

    public override void Eject()
    {
        base.Eject();
        RigiBody2DComponent.velocity = Vector2.zero;
    }
}
