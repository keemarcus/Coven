using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateManager : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
        animator.SetBool("Combo", false);
        animator.GetComponent<AnimationManager>().ResetComboFlag();
        animator.GetComponent<AnimationManager>().SetCanDoCombo(0);
        animator.GetComponent<AnimationManager>().SetIsInteracting(1);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<AnimationManager>().SetIsInteracting(0);
        PlayerManager playerManager = animator.transform.parent.GetComponent<PlayerManager>();
        if(playerManager != null) { playerManager.attackCooldown = .7f; }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
