using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPursueTargetState : AIState
{
    public GoblinIdleState idleState;
    public GoblinDeadState deadState;
    public GoblinBlockState blockState;
    public override AIState Tick(EnemyManager enemyManager, AnimationManager enemyAnimationManager)
    {
        if (enemyManager.characterStats.CurrentHP == 0)
        {
            return deadState;
        }

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
            Vector2 walkPosition = enemyManager.target.position;

            // go after the player
            #region Chase Target
            if (blockState != null && enemyManager.shield != null && enemyManager.shield.GetNearestTeamate() != null)
            {
                Vector2 blockingPosition = enemyManager.shield.GetBlockingPosition();
                if(blockingPosition == Vector2.zero)
                {
                    enemyManager.agent.enabled = false;
                    enemyAnimationManager.UpdateAnimator(Time.fixedDeltaTime, 1, (enemyManager.target.position - enemyManager.transform.position));
                    return blockState;
                }
                else
                {
                    enemyManager.agent.enabled = true;
                    enemyManager.agent.SetDestination(blockingPosition);
                    return this;
                }
            }

            if (Vector2.Distance(this.transform.position, enemyManager.target.position) <= enemyManager.maxAttackRange && Vector2.Distance(this.transform.position, enemyManager.target.position) >= enemyManager.minAttackRange && enemyManager.agent.enabled)
            {
                enemyManager.agent.enabled = false;
                enemyAnimationManager.UpdateAnimator(Time.fixedDeltaTime, 1, (enemyManager.target.position - enemyManager.transform.position));
                enemyManager.HandleAttack();
            }
            else 
            {
                enemyManager.agent.enabled = true;

                if(Vector2.Distance(this.transform.position, enemyManager.target.position) < enemyManager.minAttackRange)
                {
                    enemyManager.agent.SetDestination(enemyManager.transform.position + (enemyManager.transform.position - enemyManager.target.position).normalized);
                }
                else
                {
                    enemyManager.agent.SetDestination(enemyManager.target.position);
                }
            }
            #endregion

            return this;
        }
    }
}
