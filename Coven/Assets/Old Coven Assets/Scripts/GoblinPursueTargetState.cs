using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPursueTargetState : AIState
{
    public GoblinIdleState idleState;
    public override AIState Tick(EnemyManager enemyManager, AnimationManager enemyAnimationManager)
    {
        // look for a potential target
        #region Target Detection
        enemyManager.LookForEnemy();
        #endregion

        if (!enemyManager.canSee) // go back to idle state
        {
            return idleState;
        }
        else
        {
            // go after the player
            #region Chase Target
            if (Vector2.Distance(this.transform.position, enemyManager.target.position) <= 1.2f && enemyManager.agent.enabled)
            {
                enemyManager.agent.enabled = false;
                enemyManager.isInteracting = true;
                enemyManager.HandleAttack();
            }
            else 
            {
                enemyManager.agent.enabled = true;
                enemyManager.isInteracting = false;
            }

            if (enemyManager.agent.enabled) { enemyManager.agent.SetDestination(enemyManager.target.position); }
            #endregion

            return this;
        }
    }
}
