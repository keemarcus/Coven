using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Transition Info")]
    //public Scene targetScene;
    public string targetScene;
    public Vector2 targetPosition;
    public string targetDirection;

    public string GetTargetSceneName()
    {
        return targetScene;
    }

    public void TransitionScene()
    {
        // start to fade out
        GameObject.FindGameObjectWithTag("Crossfade").GetComponent<Animator>().SetTrigger("Out");

        // save the postion of every enemy 
        foreach (EnemyManager enemy in FindObjectsOfType<EnemyManager>())
        {
            //enemy.SetDirection();
            enemy.UpdateStats(SceneManager.GetActiveScene().name, enemy.transform.position, enemy.characterStats.CharacterDirection);
        }

        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
    }
}
