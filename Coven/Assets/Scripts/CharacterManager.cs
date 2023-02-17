using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public abstract class CharacterManager : MonoBehaviour
{
    public AnimationManager animationManager;

    [Header("AI Settings")]
    public string teamTag;
    public Vector2 sightVector;

    [Header("Combat Stuff")]
    public bool canDoCombo;
    public bool comboFlag;
    public Vector2 aimDirection;
    public bool alreadyCast;
    protected float healthRegenTimer;

    [Serializable]
    public struct CharacterStats
    {
        [Header("Character Stats")]
        public int MaxHP;
        public int CurrentHP;
        public int HitBonus;
        public int DamageResistance;
        public float EnergyLevel;

        [Header("Character Position")]
        public string CharacterScene;
        public Vector2 CharacterPosition;
        public string CharacterDirection;

        public CharacterStats(int maxHP, int hitBonus, int damageResistance, float energyLevel, string characterScene, Vector2 characterPosition, string characterDirection)
        {
            MaxHP = maxHP;
            CurrentHP = MaxHP;
            HitBonus = hitBonus;
            DamageResistance = damageResistance;
            EnergyLevel = energyLevel;
            CharacterScene = characterScene;
            CharacterPosition = characterPosition;
            CharacterDirection = characterDirection;
        }
    }

    public CharacterStats characterStats;

    //public FileManager fileManager;
    [Header("File Settings")]
    public string savePath;

    [Header("Character Flags")]
    public bool isInteracting;
    public bool isDead;

    void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        if(animationManager == null)
        {
            animationManager = GetComponentInChildren<AnimationManager>();
        }
        

        if (FileManager.GetStats(savePath + this.gameObject.name + "_stats.json").Equals(new CharacterManager.CharacterStats(0, 0, 0, 0, "", Vector2.zero, "")))
        {
            // set up a new save object for the characters stats
            characterStats = new CharacterStats(50, 0, 0, 0, SceneManager.GetActiveScene().name, Vector2.zero, "Down");
            FileManager.SaveStats(savePath + this.gameObject.name + "_stats.json", characterStats);
        }
        else
        {
            // read the existing save object for the characters stats
            characterStats = FileManager.GetStats(savePath + this.gameObject.name + "_stats.json");
        }

        // set the character position in game
        if(characterStats.CharacterScene == SceneManager.GetActiveScene().name && characterStats.CharacterPosition != Vector2.zero)
        {
            this.gameObject.transform.position = characterStats.CharacterPosition;

            float delta = Time.fixedDeltaTime;
            switch (characterStats.CharacterDirection)
            {
                case "Left":
                    animationManager.UpdateAnimator(delta, 1, new Vector2(-1, 0));
                    break;
                case "Right":
                    animationManager.UpdateAnimator(delta, 1, new Vector2(1, 0));
                    break;
                case "Up":
                    animationManager.UpdateAnimator(delta, 1, new Vector2(0, 1));
                    break;
                default:
                    animationManager.UpdateAnimator(delta, 1, new Vector2(0, -1));
                    break;
            }
        }

        // determine if the character is dead already
        if (this.characterStats.CurrentHP <= 0) { this.isDead = true; }
        else { this.isDead = false; }
        this.animationManager.animator.SetBool("Dead", this.isDead);
    }

    public void UpdateStats()
    {
        characterStats.CharacterPosition = this.gameObject.transform.position;
        FileManager.SaveStats(savePath + this.gameObject.name + "_stats.json", characterStats);
    }
    public void UpdateStats(string sceneName, Vector2 targetPosition, string targetDirection)
    {
        characterStats.CharacterScene = sceneName;
        characterStats.CharacterPosition = targetPosition;
        characterStats.CharacterDirection = targetDirection;
        FileManager.SaveStats(savePath + this.gameObject.name + "_stats.json", characterStats);
    }

    public void HandleAttack()
    {
        if (canDoCombo) { comboFlag = true; }
        if (!isInteracting) { isInteracting = true; }
        animationManager.HandleAttackAnimations();
    }

    public void TakeDamage(int damageAmount, string damageType)
    {
        // remove the damage resistance value from the incoming damage
        damageAmount = Math.Clamp(damageAmount - characterStats.DamageResistance, 0, damageAmount);

        Debug.Log(this.gameObject.name + " took " + damageAmount + " " + damageType +" damage.");
        characterStats.CurrentHP -= damageAmount;
        if(characterStats.CurrentHP <= 0)
        {
            characterStats.CurrentHP = 0;
            Debug.Log(this.gameObject.name + " died.");
            this.isDead = true;
            this.animationManager.animator.SetBool("Dead", true);
            // disable the colliders for this character when they die
            foreach(Collider2D collider in this.gameObject.GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        this.healthRegenTimer = 0f;
    }

    public void SetDirectionCHARMAN()
    {
        switch (aimDirection.x, aimDirection.y)
        {
            case (-1,0):
                SetDirection(1);
                break;
            case (1, 0):
                SetDirection(2);
                break;
            case (0, 1):
                SetDirection(3);
                break;
            default:
                SetDirection(0);
                break;
        }
    }

    public void SetDirection(int directionKey)
    {
        Debug.Log("Character Manager");
        switch (directionKey)
        {
            case 1:
                this.characterStats.CharacterDirection = "Left";
                sightVector = Vector2.left;
                break;
            case 2:
                this.characterStats.CharacterDirection = "Right";
                sightVector = Vector2.right;
                break;
            case 3:
                this.characterStats.CharacterDirection = "Up";
                sightVector = Vector2.up;
                break;
            default:
                this.characterStats.CharacterDirection = "Down";
                sightVector = Vector2.down;
                break;
        }
    }
}
