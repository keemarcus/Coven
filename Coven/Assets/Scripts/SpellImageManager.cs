using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellImageManager : MonoBehaviour
{
    public int spellIndex;
    public bool isActive;
    
    public void SetActive(bool newStatus)
    {
        isActive = newStatus;
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (this.isActive)
        {
            this.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(40, 40);
        }
        else
        {
            this.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(30, 30);
        }
    }

    public void SetImage(Sprite newImage)
    {
        this.gameObject.GetComponent<Image>().sprite = newImage;
    }
}
