using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> dialogueLines;

    GameObject dialogueWindow;
    PlayerManager playerManager;
    public NPCManager npcManager;

    private void Awake()
    {
        dialogueLines = new Queue<string>();

        dialogueWindow = this.transform.GetChild(0).gameObject;
        dialogueWindow.SetActive(false);

        playerManager = FindObjectOfType<PlayerManager>();
    }

    public void ReadLine()
    {
        // if there are no lines left, exit
        if(dialogueLines.Count == 0)
        {
            dialogueWindow.SetActive(false);
            playerManager.isInteracting = false;
            if(npcManager != null && npcManager.isTalking)
            {
                npcManager.isTalking = false;
            }
        }
        else
        {
            // read the next line
            dialogueWindow.GetComponentInChildren<Text>().text = dialogueLines.Dequeue();
            dialogueWindow.SetActive(true);

            // set player state
            if (!playerManager.isInteracting)
            {
                playerManager.isInteracting = true;
            }

            if (npcManager != null && !npcManager.isTalking)
            {
                npcManager.isTalking = true;
            }
        }
        

        
    }
}
