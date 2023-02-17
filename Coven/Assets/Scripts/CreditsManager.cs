using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public string[] credits;
    public TMPro.TextMeshProUGUI creditText;
    float timer;
    int index;

    private void Awake()
    {
        creditText = this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        timer = 0f;
        index = 0;
        ChangeCredit(index);
    }

    private void Update()
    {
        if(timer >= 7f)
        {
            index++;
            ChangeCredit(index);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void ChangeCredit(int index)
    {
        if(creditText == null)
        {
            Debug.Log("Text object not found");
            return;
        }else if(index >= credits.Length)
        {
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        }
        else
        {
            creditText.text = credits[index];
        }
    }

}
