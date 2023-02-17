using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    SceneTransitionManager sceneTransitionManager;
    public bool allGoblinsDead;
    
    // Start is called before the first frame update
    void Start()
    {
        sceneTransitionManager = GetComponent<SceneTransitionManager>();
        allGoblinsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyManager[] goblins = FindObjectsOfType<EnemyManager>();
        allGoblinsDead = true;
        foreach (EnemyManager goblin in goblins)
        {
            if (!goblin.isDead && goblin.teamTag != "Player")
            {
                allGoblinsDead = false;
                break;
            }
        }
        
        if (allGoblinsDead)
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().ChangeSong(FindObjectOfType<PlayerManager>().normalMusic);
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().SetVolume(.5f);
            sceneTransitionManager.targetScene = "WoodsEnd";
        }else
        {
            sceneTransitionManager.targetScene = "Woods";
        }
    }
}
