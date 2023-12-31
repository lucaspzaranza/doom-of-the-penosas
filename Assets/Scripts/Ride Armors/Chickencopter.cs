using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chickencopter : RideArmor
{
    [SerializeField] private Animator _propellerAnimator;

    public override void Equip(Penosa player, PlayerController playerController)
    {
        base.Equip(player, playerController);
        _propellerAnimator.enabled = true;
    }

    public override void Eject()
    {
        base.Eject();
        _propellerAnimator.enabled = false;
    }
}
