using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    SpellUIManager spellUIManager;
    public AudioSource audioSource;
    public GameObject musicPrefab;
    public AudioClip outOfEnergySound;
    public AudioClip bossMusic;
    public AudioClip normalMusic;

    public bool wonGame;
    private float deathTimer;

    private int lastHealthIncrement;

    [Header("Spells")]
    public SpellSlot activeSpell;
    public int activeSpellIndex;
    public SpellSlot[] spells;
    public GameObject spellOriginOffset;
    //public Vector2 aimDirection;
    //public bool alreadyCast;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
        spellOriginOffset = this.gameObject.transform.GetChild(1).gameObject;
        activeSpellIndex = 0;
        ChangeActiveSpell(0);
        alreadyCast = false;
        spellUIManager = FindObjectOfType<SpellUIManager>();
        UpdateSpellUI();
        spellUIManager.healthSlider.maxValue = this.characterStats.MaxHP;
        spellUIManager.healthSlider.value = this.characterStats.CurrentHP;
        audioSource = this.gameObject.GetComponent<AudioSource>();

        if(GameObject.FindGameObjectWithTag("Music") == null)
        {
            Instantiate(musicPrefab);
        }

        if(SceneManager.GetActiveScene().name == "GoblinVillage")
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().ChangeSong(bossMusic);
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().SetVolume(.15f);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().ChangeSong(normalMusic);
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().SetVolume(.5f);
        }

        deathTimer = 0f;
        wonGame = false;
    }

    new public void HandleAttack()
    {
        if (isInteracting) { return; }
        if (this.characterStats.EnergyLevel < activeSpell.spellScript.energyCost)
        {
            // play out of energy sound effect
            if (audioSource != null)
            {
                audioSource.clip = outOfEnergySound;
                audioSource.Play();
            }
            return;
        }
        animationManager.HandleCastAnimations(activeSpell.spellAnimation);
    }

    public void Cast()
    {
        if (alreadyCast || this.aimDirection == Vector2.zero || this.characterStats.EnergyLevel < activeSpell.spellScript.energyCost) { return; }
        aimDirection.Normalize();
        activeSpell.Cast(spellOriginOffset.transform.position, this.gameObject, audioSource);
        alreadyCast = true;

        // update player energy level
        this.characterStats.EnergyLevel = Mathf.Clamp(this.characterStats.EnergyLevel -= activeSpell.spellScript.energyCost, 0, 100);
        UpdateSpellUI();
    }

    public void ChangeActiveSpell(float direction)
    {
        if(direction > 0)
        {
            activeSpellIndex++;
            if(activeSpellIndex >= spells.Length || spells[activeSpellIndex] == null) { activeSpellIndex = 0; }
        }else if(direction < 0)
        {
            activeSpellIndex--;
            if (activeSpellIndex < 0) 
            {
                int i = 1;
                do
                {
                    activeSpellIndex = spells.Length - i;
                    i++;
                } while (spells[activeSpellIndex] == null);
                 
            }
        }
        else
        {
            activeSpellIndex = 0;
        }

        activeSpell = spells[activeSpellIndex];
        UpdateSpellUI();
    }
    public void UpdateSpellUI()
    {
        if(spellUIManager == null) { return; }
        spellUIManager.SetEnergyAmount(this.characterStats.EnergyLevel);
        spellUIManager.SetActiveSpell(activeSpellIndex);
        for(int i = 0; i < spells.Length; i ++)
        {
            spellUIManager.SetSpellImage(i, spells[i].spellScript.UIImage);
        }
    }

    void Update()
    {
        float delta = Time.deltaTime;
        if(isDead || wonGame)
        {
            deathTimer += delta;
            if(deathTimer >= 3.5f)
            {
                if (isDead)
                {
                    GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().StopMusic();
                    LoseGame();
                }
                else
                {
                    WinGame();
                }
            }

            // update the health bar
            if (spellUIManager.healthSlider.value != this.characterStats.CurrentHP)
            {
                spellUIManager.SetHealthAmount(this.characterStats.CurrentHP);
            }
            return;
        }

        // take player inputs
        inputManager.TickInput(delta);

        activeSpell.spellScript.CheckForTarget(spellOriginOffset.transform.position);

        // health regen
        if(!this.isDead && this.healthRegenTimer < 50f)
        {
            healthRegenTimer += delta;
            int newHealthIncrement = (int)Mathf.Floor(healthRegenTimer);
            if (this.characterStats.CurrentHP < this.characterStats.MaxHP  && newHealthIncrement > lastHealthIncrement)
            {
                lastHealthIncrement = newHealthIncrement;
                this.characterStats.CurrentHP = (int)Mathf.Clamp(this.characterStats.CurrentHP + newHealthIncrement, this.characterStats.CurrentHP, this.characterStats.MaxHP);
            }
        }


        // update the health bar
        if(spellUIManager.healthSlider.value != this.characterStats.CurrentHP)
        {
            spellUIManager.SetHealthAmount(this.characterStats.CurrentHP);
        }
    }

    private void FixedUpdate()
    {
        //if (isDead) { inputManager.moveAmount = 0; return; }
        if (isDead || isInteracting || wonGame) { inputManager.moveAmount = 0; }
        float delta = Time.fixedDeltaTime;

        // move the player
        playerLocomotion.HandleMovement(delta);

        // update the animator
        animationManager.UpdateAnimator(delta, inputManager.moveAmount, new Vector2(inputManager.horizontal, inputManager.vertical));

        // update the camera postion
        if (cameraManager != null)
        {
            cameraManager.FollowTarget(delta);
        }

        // update the spell UI components
        //UpdateSpellUI();
    }

    private void LateUpdate()
    {
        // reset all our input variables at the end of the frame so they can only be triggered once per press
        inputManager.attackInput = false;
        inputManager.interactInput = false;
        inputManager.changeSpell = false;
    }

    public void HandleInteract()
    {
        // look for nearby things to interact with
        Interactable[] interactableObjects =  FindObjectsOfType<Interactable>();
        foreach(Interactable interactable in interactableObjects)
        {
            // if we're close enough to interact with one, do so and return
            if(Vector2.Distance(this.transform.position, interactable.transform.position) < 1.5f)
            {
                interactable.Interact();
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.GetComponent<SceneTransitionManager>())
        {
            SceneTransitionManager transition = collision.collider.gameObject.GetComponent<SceneTransitionManager>();
            UpdateStats(transition.GetTargetSceneName(), transition.targetPosition, transition.targetDirection);
            transition.TransitionScene();
        }
    }

    public void LoseGame()
    {
        // clear all save date and go back to the main menu
        FileManager.ClearSaveData();
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    public void WinGame()
    {
        // clear all save date and go to the credits
        FileManager.ClearSaveData();
        SceneManager.LoadSceneAsync("EndCredits", LoadSceneMode.Single);
    }
}
