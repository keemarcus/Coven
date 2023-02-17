using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateManager : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");

        animator.GetComponent<AnimationManager>().SetIsInteracting(1);
        CharacterManager characterManager = animator.GetComponent<CharacterManager>();
        if(characterManager != null)
        {
            characterManager.alreadyCast = false;
        }

        EnemyManager enemyManager = animator.GetComponent<EnemyManager>();
        if (enemyManager != null && enemyManager.meleeAttackCollider != null)
        {
            enemyManager.meleeAttackCollider.enabled = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationManager>().SetIsInteracting(0);

        EnemyManager enemyManager = animator.GetComponent<EnemyManager>();
        if (enemyManager != null && enemyManager.meleeAttackCollider != null)
        {
            enemyManager.meleeAttackCollider.enabled = false;
        }
    }
}
