using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateManager : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.ResetTrigger("Attack");

        animator.ResetTrigger("Attack");
        animator.SetBool("Combo", false);
        animator.GetComponent<AnimationManager>().ResetComboFlag();
        animator.GetComponent<AnimationManager>().SetCanDoCombo(0);
        animator.GetComponent<AnimationManager>().SetIsInteracting(0);

        animator.GetComponent<AnimationManager>().SetIsInteracting(1);
        CharacterManager characterManager = animator.GetComponent<CharacterManager>();
        if(characterManager != null)
        {
            characterManager.alreadyCast = false;
        }

        //CharacterManager characterManager = animator.GetComponent<CharacterManager>();
        if (characterManager != null && characterManager.meleeAttackCollider != null)
        {
            characterManager.meleeAttackCollider.enabled = true;
        }
        else
        {
            Debug.Log("no AC");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationManager>().SetIsInteracting(0);

        CharacterManager characterManager = animator.GetComponent<CharacterManager>();
        if (characterManager != null && characterManager.meleeAttackCollider != null)
        {
            characterManager.meleeAttackCollider.enabled = false;
            characterManager.attackCooldown = .7f;
        }
    }
}
