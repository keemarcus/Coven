using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = this.gameObject.GetComponent<EnemyManager>();
    }

    public EnemyManager GetNearestTeamate()
    {
        EnemyManager nearestTeamate = null;

        foreach(EnemyManager character in FindObjectsOfType<EnemyManager>())
        {
            if(character.teamTag == enemyManager.teamTag && !character.isDead && character.maxAttackRange > 2f && (Vector2.Distance(enemyManager.transform.position, character.transform.position) < 10f)){
                if (nearestTeamate == null || Vector2.Distance(enemyManager.transform.position, character.transform.position) < Vector2.Distance(enemyManager.transform.position, nearestTeamate.transform.position)) {
                    nearestTeamate = character;
                }
            }
        }

        return nearestTeamate;
    }

    public Vector2 GetBlockingPosition()
    {
        EnemyManager nearestTeamate = GetNearestTeamate();

        if(nearestTeamate == null)
        {
            return Vector2.zero;
        }
        else
        {
            float rayDistance = Vector2.Distance(enemyManager.target.position, nearestTeamate.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(enemyManager.target.position, (nearestTeamate.transform.position - enemyManager.target.position), rayDistance);
            if(hit.collider.gameObject == enemyManager.gameObject && (Vector2.Distance(hit.point, nearestTeamate.transform.position) >  rayDistance / 3))
            {
                return Vector2.zero;
            }
            else
            {
                return new Vector2((enemyManager.target.position.x + nearestTeamate.transform.position.x) / 2, (enemyManager.target.position.y + nearestTeamate.transform.position.y) / 2);
            }
        }
    }
}
