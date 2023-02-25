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
                enemyManager.agent.enabled = true;

                // check to see if they're too far from their leader
                if (enemyManager.leader != null && Vector2.Distance(enemyManager.transform.position, enemyManager.leader.transform.position) >= 5f)
                {
                    enemyManager.agent.SetDestination(enemyManager.transform.position + ((enemyManager.leader.transform.position - enemyManager.transform.position) / 2));
                }
                else // if not, find a new random destination nearby and start heading there
                {
                    enemyManager.agent.SetDestination(enemyManager.RandomNavmeshLocation(5f));
                }
                
                enemyManager.idleWanderTimer = 0f;
            }
            else 
            {
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
