using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingStateManager : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // set the damage resistance to 10 when blocking with a shield
        CharacterManager character =  animator.gameObject.GetComponent<CharacterManager>();
        if(character != null && character.gameObject.GetComponentInChildren<Shield>() != null)
        {
            character.characterStats.DamageResistance = 10;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // set the damage resistance back to 0 when leaving the blocking state
        CharacterManager character = animator.gameObject.GetComponent<CharacterManager>();
        if (character != null && character.gameObject.GetComponentInChildren<Shield>() != null)
        {
            character.characterStats.DamageResistance = 0;
        }
    }
}
