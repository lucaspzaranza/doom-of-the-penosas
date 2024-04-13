using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedData.Enumerations;

public class BasicEnemy : Enemy
{
    private void Update()
    {
        if(State == EnemyState.Idle)
        {
            Patrol();
        }
    }

    public override void Patrol()
    {

    }
}
