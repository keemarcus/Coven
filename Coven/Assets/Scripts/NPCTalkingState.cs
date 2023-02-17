using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkingState : NPCState
{
    public NPCIdleState idleState;
    public override NPCState Tick(NPCManager npcManager, AnimationManager enemyAnimationManager)
    {
        if (!npcManager.agent.isStopped)
        {
            npcManager.agent.isStopped = true;
        }

        // if the player is not talking to NPC anymore, return the talking state
        if (!npcManager.isTalking)
        {
            return idleState;
        }
        else
        {
            return this;
        }
    }
}
