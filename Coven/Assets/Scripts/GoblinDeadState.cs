using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDeadState : AIState
{
    public GoblinIdleState idleState;
    public override AIState Tick(EnemyManager enemyManager, AnimationManager enemyAnimationManager)
    {   
        if(enemyManager.characterStats.CurrentHP > 0)
        {
            enemyManager.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            return idleState;
        }
        else
        {
            return this;
        }
    }
}
