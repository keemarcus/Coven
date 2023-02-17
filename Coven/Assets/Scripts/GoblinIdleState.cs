using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinIdleState : AIState
{
    public GoblinPursueTargetState pursueTargetState;
    public GoblinDeadState deadState;
    public override AIState Tick(EnemyManager enemyManager, AnimationManager enemyAnimationManager)
    {
        if(enemyManager.characterStats.CurrentHP == 0)
        {
            return deadState;
        }

        // look for a potential target
        #region Target Detection
        enemyManager.LookForEnemy();
        #endregion

        // if enemy can see the player, switch to pursue target state
        if (enemyManager.canSee)
        {
            return pursueTargetState;
        } 
        else // if not, stay in idle state
        {
            // wander around
            #region Wander
            // see if we're currently moving
            if(enemyManager.idleWanderTimer >= 3f && (!enemyManager.agent.hasPath || enemyManager.agent.velocity == Vector3.zero))
            {
                //Debug.Log("Not Moving");
                // if not, find a new random destination nearby and start heading there
                enemyManager.agent.enabled = true;
                enemyManager.agent.SetDestination(enemyManager.RandomNavmeshLocation(5f));
                enemyManager.idleWanderTimer = 0f;
            }
            else 
            {
                //Debug.Log("Moving");
                if (enemyManager.idleWanderTimer < 3f && (!enemyManager.agent.hasPath || enemyManager.agent.velocity == Vector3.zero))
                {
                    enemyManager.idleWanderTimer += Time.deltaTime;
                }
            }
            #endregion
            return this;
        }
    } 
}
