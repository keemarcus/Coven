using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Transition Info")]
    //public Scene targetScene;
    public string targetScene;
    public Vector2 targetPosition;
    public string targetDirection;

    //private AssetBundle myLoadedAssetBundle;
    //private int targetSceneIndex;
    //BoxCollider2D collider;

    private void Awake()
    {
        //myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/scenes");
        //scene
    }

    public string GetTargetSceneName()
    {
        //return targetScene.Split('/', '.').GetValue(2).ToString();
        return targetScene;
    }

    public void TransitionScene()
    {
        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
    }
}
