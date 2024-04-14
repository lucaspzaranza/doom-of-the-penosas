using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;

public class BasicEnemy : Enemy
{
    private void Update()
    {
        Patrol();
    }

    public override void Patrol()
    {
        Move();
    }

    protected override void Move()
    {
        print("MOVIN', MOVIN'!");
    }

    protected override void CheckForNewState()
    {

    }
}
