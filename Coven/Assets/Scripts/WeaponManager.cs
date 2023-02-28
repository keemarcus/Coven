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
    private bool inAttack;

    [Header("Weapon Stats")]
    public string damageType;
    public int baseDamage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = null;

        damageCollider = GetComponent<BoxCollider2D>();

        character = this.transform.parent.GetComponent<CharacterManager>();
        inAttack = false;

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
    private void LateUpdate()
    {
        if (inAttack) { inAttack = false; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (inAttack)
        {
            return;
        }

        inAttack = true;
        CharacterManager targetHit = collision.GetComponent<CharacterManager>();
        if (targetHit != null && targetHit.teamTag != character.teamTag && targetHit.gameObject.name != this.gameObject.name)
        {
            // play sound effect
            //if (meleeHitSound != null && audioSource != null)
            {
                //audioSource.clip = meleeHitSound;
                //audioSource.Play();
            }

            // damaeg the target that was hit
            targetHit.TakeDamage(this.baseDamage, this.damageType);
        }
    }
}
