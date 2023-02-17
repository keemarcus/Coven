using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbSpell : Spell
{
    public override void Create(Vector2 origin, GameObject caster)
    {
        PlantManager nearestTarget = CheckForPlant(origin);
        if (nearestTarget != null)
        {
            nearestTarget.Absorb();
        }
    }

    public PlantManager CheckForPlant(Vector2 origin)
    {
        PlantManager nearestTarget = null;

        foreach (PlantManager plantManager in FindObjectsOfType<PlantManager>())
        {
            if (plantManager.isDead)
            {
                continue;
            }

            if (Vector2.Distance(plantManager.transform.position, origin) <= range)
            {
                if (nearestTarget == null || Vector2.Distance(plantManager.transform.position, origin) < Vector2.Distance(nearestTarget.transform.position, origin))
                {
                    //Debug.Log(enemyManager.gameObject.name);
                    nearestTarget = plantManager;
                }
            }

            plantManager.selectedForAbsorb = false;
        }

        if (nearestTarget != null)
        {
            nearestTarget.selectedForAbsorb = true;
        }

        return nearestTarget;
    }

    public override EnemyManager CheckForTarget(Vector2 origin)
    {
        CheckForPlant(origin);
        return null;
    }
}
