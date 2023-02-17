using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGameTrigger : MonoBehaviour
{
    void Update()
    {
        EnemyManager maggie = FindObjectOfType<EnemyManager>();
        if(maggie == null || maggie.isDead)
        {
            FindObjectOfType<PlayerManager>().wonGame = true; ;
        }
    }
}
