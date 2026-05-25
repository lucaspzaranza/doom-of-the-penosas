using System;
using System.Collections;
using UnityEngine;

public static class EnemyEvents
{
    public static Action<IEnemy> OnEnemyDeath;
    public static Action OnBossDefeated;
}