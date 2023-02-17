using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public float energyValue;
    public bool isDead;
    public bool selectedForAbsorb;
    PlayerManager playerManager;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        if (playerManager != null)
        {
            if (playerManager.transform.position.y > this.transform.position.y && !this.isDead)
            {
                this.GetComponent<SpriteRenderer>().sortingOrder = 7;
                this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 6;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sortingOrder = 4;
                this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 3;
            }

            if (playerManager.activeSpell.name.Contains("Absorb") && this.selectedForAbsorb)
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void Absorb()
    {
        this.gameObject.SetActive(false);
        playerManager.characterStats.EnergyLevel = Mathf.Clamp(playerManager.characterStats.EnergyLevel + this.energyValue, 0, 100);
    }
}
