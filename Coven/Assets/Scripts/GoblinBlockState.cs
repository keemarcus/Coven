using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBlockState : AIState
{
    public GoblinDeadState deadState;
    public GoblinPursueTargetState pursueState;
    public override AIState Tick(EnemyManager enemyManager, AnimationManager enemyAnimationManager)
    {
        if (enemyManager.characterStats.CurrentHP == 0)
        {
            enemyAnimationManager.animator.SetBool("Blocking", false);
            return deadState;
        }

        if (enemyManager.shield.GetNearestTeamate() != null && enemyManager.shield.GetBlockingPosition() == Vector2.zero)
        {
            enemyAnimationManager.animator.SetBool("Blocking", true);
            return this;
        }
        else
        {
            enemyAnimationManager.animator.SetBool("Blocking", false);
            return pursueState;
        }
    }
}
