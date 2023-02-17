using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdleState : NPCState
{
    public NPCTalkingState talkingState;
    public override NPCState Tick(NPCManager npcManager, AnimationManager enemyAnimationManager)
    {
        if (npcManager.agent.isStopped)
        {
            npcManager.agent.isStopped = false;
        }

        // if the player is talking to NPC, return the talking state
        if (npcManager.isTalking)
        {
            return talkingState;
        }
        else // if not, stay in idle state
        {
            // wander around
            #region Wander
            // see if we're currently moving
            if (npcManager.idleWanderTimer >= 3f && (!npcManager.agent.hasPath || npcManager.agent.velocity == Vector3.zero))
            {
                // if not, find a new random destination nearby and start heading there
                npcManager.agent.SetDestination(npcManager.RandomNavmeshLocation(5f));
                npcManager.idleWanderTimer = 0f;
            }
            else
            {
                if (npcManager.idleWanderTimer < 3f && (!npcManager.agent.hasPath || npcManager.agent.velocity == Vector3.zero))
                {
                    npcManager.idleWanderTimer += Time.deltaTime;
                }
            }
            #endregion
            return this;
        }
    }
}
