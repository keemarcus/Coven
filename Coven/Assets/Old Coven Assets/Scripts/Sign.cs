using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : Interactable
{
    [Header("Dialogue")]
    public List<string> signText;

    PlayerManager playerManager;
    SpriteRenderer textBubbleRenderer;

    DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        textBubbleRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public override void Interact()
    {
        if (!FindObjectOfType<PlayerManager>().isInteracting)
        {
            foreach(string line in signText)
            {
                dialogueManager.dialogueLines.Enqueue(line);
            }
        }
        
        if (this.gameObject.GetComponent<NPCManager>() != null)
        {
            dialogueManager.npcManager = this.gameObject.GetComponent<NPCManager>();
        }
        else
        {
            dialogueManager.npcManager = null;
        }

        dialogueManager.ReadLine();
    }

    private void Update()
    {
        if (playerManager.gameObject.activeInHierarchy && (Vector2.Distance(this.transform.position, playerManager.transform.position) < 1.5f))
        {
            if (!textBubbleRenderer.enabled)
            {
                // set the interactable indicator to active
                textBubbleRenderer.enabled = true;
            }
            
        }
        else if(textBubbleRenderer.enabled)
        {
            // set the interactable indicator to inactive
            textBubbleRenderer.enabled = false;
        }
    }
}
