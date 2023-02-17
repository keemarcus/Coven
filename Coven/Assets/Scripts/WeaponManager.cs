using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

public class WeaponManager : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    //public Texture2D spriteSheet;
    //Sprite[] attackSprites;
    BoxCollider2D damageCollider;

    CharacterManager character;

    [Header("Weapon Stats")]
    public string damageType;
    public int baseDamage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = null;

        damageCollider = GetComponent<BoxCollider2D>();

        character = this.transform.GetComponent<CharacterManager>();

        // load all the individual sprites from the sprite sheet
        //attackSprites = Resources.LoadAll<Sprite>("4d_attacks/rmsheets/" + spriteSheet.name);
    }

    public void SetSprite(int index)
    {
        // set the current sprite, triggered from player animations
        if(index < 0) { spriteRenderer.sprite = null; }
        else
        {
            //spriteRenderer.sprite = attackSprites[index];
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterManager hitCharacterManager = collision.gameObject.GetComponent<CharacterManager>();

        if(hitCharacterManager != null)
        {
            //hitCharacterManager.TakeDamage(CalculateDamage(hitCharacterManager), damageType);
        }
    }

    private int CalculateDamage(CharacterManager collidedCharacter)
    {
        // start with the base damage
        int damage = baseDamage;

        // check for stats on attacking character that effect the damage
        damage += character.characterStats.HitBonus;

        // check for stats on the character we collided with that effect the damage
        damage -= collidedCharacter.characterStats.DamageResistance;

        return damage;
    }
}
