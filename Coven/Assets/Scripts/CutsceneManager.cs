using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    private float timer;
    private bool startedDialogue;

    private void Start()
    {
        // stop the player from moving/casting spells during the cutscene
        if(FindObjectOfType<PlayerManager>() == null) { Debug.Log("no player found"); }
        if (FindObjectOfType<EnemyManager>() == null) { Debug.Log("no enemy found"); }
        FindObjectOfType<PlayerManager>().isInteracting = true;
        FindObjectOfType<EnemyManager>().isInteracting = true;
        FindObjectOfType<EnemyManager>().animationManager.UpdateAnimator(0, 1, Vector2.up); ;

        // start the timer
        timer = 0f;
        startedDialogue = false;
    }

    void Update()
    { 
        timer += Time.deltaTime;
        //Debug.Log(timer + " " + startedDialogue);
        if(!startedDialogue && timer >= 2f)
        {
            // trigger the dialogue
            FindObjectOfType<PlayerManager>().isInteracting = false;
            FindObjectOfType<PlayerManager>().HandleInteract();
            startedDialogue = true;
        }

        if (!FindObjectOfType<PlayerManager>().isInteracting)
        {
            // trigger the fight
            FindObjectOfType<EnemyManager>().isInteracting = false;
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().ChangeSong(FindObjectOfType<PlayerManager>().bossMusic);
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().SetVolume(.15f);
        }

        if (FindObjectOfType<EnemyManager>().isDead)
        {
            // stop the music
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().StopMusic();
        }
    }
}
