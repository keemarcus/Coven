using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpell : Spell
{
    [Header("Spell Attributes")]
    public float missileSpeed;
    public string spellType;
    public int spellEffectAmount;
    public string spellEffectType;
    public string casterTeam;
    Transform casterTransform;
    public AudioClip impactSound;

    public override void Create(Vector2 origin, GameObject caster)
    {
        var spellAngle = Mathf.Atan2(caster.GetComponent<CharacterManager>().aimDirection.x * -1, caster.GetComponent<CharacterManager>().aimDirection.y) * Mathf.Rad2Deg;
        GameObject newSpell = Instantiate(this.gameObject, origin, Quaternion.AngleAxis(spellAngle, Vector3.forward));
        newSpell.GetComponent<Rigidbody2D>().freezeRotation = true;
        newSpell.GetComponent<Rigidbody2D>().velocity = caster.GetComponent<CharacterManager>().aimDirection * missileSpeed;
        Physics2D.IgnoreCollision(newSpell.GetComponent<Collider2D>(), caster.GetComponent<Collider2D>());
        newSpell.GetComponent<MissileSpell>().casterTeam = caster.GetComponent<CharacterManager>().teamTag;
        newSpell.GetComponent<MissileSpell>().casterTransform = caster.gameObject.transform;
    }

    private void Update()
    {
        if(casterTransform == null)
        {
            Debug.Log("no caster set");
        }
        // destroy the spell if it's beyond the range away from the caster
        if(Vector2.Distance(this.transform.position, casterTransform.position) > this.range)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        // look for charcters that we hit
        CharacterManager hitCharacterManager = collision.gameObject.GetComponent<CharacterManager>();
        if (hitCharacterManager != null)
        {
            // if we hit a teamate of the caster or a dead character, ignore this collsion
            if(hitCharacterManager.teamTag == this.casterTeam || hitCharacterManager.isDead) { return; }

            // check what type of spell this is
            switch (CheckType())
            {
                // if it's an attack, damage any chatacters we hit
                case "Attack":
                    hitCharacterManager.TakeDamage(spellEffectAmount, spellEffectType);
                    break;

                default:
                    Debug.Log("No Valid Spell Type Found");
                    break;
            }
        }

        AudioSource audioSource = FindObjectOfType<PlayerManager>().audioSource;
        if (audioSource != null)
        {
            audioSource.clip = impactSound;
            audioSource.Play();
        }
        Destroy(this.gameObject);
    }

    private string CheckType()
    {
        switch (spellType)
        {
            case "Attack":
                return "Attack";

            default:
                return "No Valid Spell Type Found";
        }
    }

    public override EnemyManager CheckForTarget(Vector2 origin)
    {
        return null;
    }
}
