using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpell : Spell
{
    public GameObject creaturePrefab;
    public override void Create(Vector2 origin, GameObject caster)
    {
        var spellAngle = Mathf.Atan2(caster.GetComponent<PlayerManager>().aimDirection.x * -1, caster.GetComponent<PlayerManager>().aimDirection.y) * Mathf.Rad2Deg;
        GameObject newCreature = Instantiate(creaturePrefab, origin + (caster.GetComponent<PlayerManager>().aimDirection * range), Quaternion.identity);
        newCreature.GetComponent<CharacterManager>().teamTag = caster.GetComponent<CharacterManager>().teamTag;
    }

    public override EnemyManager CheckForTarget(Vector2 origin)
    {
        return null;
    }
}
