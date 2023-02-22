using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private List<Vector2> playerSpawnPoints;
    private List<Vector2> enemySpanPoints;

    public void Initialize()
    {
        // initialize all the lists of spawn points
        playerSpawnPoints = new List<Vector2>();
        enemySpanPoints = new List<Vector2>();
    }

    public void AddPlayerSpawnPoint(Vector2 position)
    {
        playerSpawnPoints.Add(position);
    }
    public void AddEnemySpawnPoint(Vector2 position)
    {
        enemySpanPoints.Add(position);
        Debug.Log(position);
    }

    public void SpawnPlayers()
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        for (int ctr = 0; ctr < players.Length; ctr++)
        {
            if (ctr < playerSpawnPoints.Count && playerSpawnPoints[ctr] != null)
            {
                players[ctr].gameObject.transform.SetPositionAndRotation(playerSpawnPoints[ctr], Quaternion.identity);
            }
            else
            {
                Debug.Log("No spawn point found for player: " + players[ctr].gameObject.name);
                players[ctr].gameObject.SetActive(false);
            }

        }
    }

    public void SpawnEnemies()
    {
        EnemyManager[] enemies = FindObjectsOfType<EnemyManager>();
        for (int ctr = 0; ctr < enemies.Length; ctr++)
        {
            Debug.Log(enemies[ctr]);
            if (ctr < enemySpanPoints.Count && enemySpanPoints[ctr] != null)
            {
                enemies[ctr].agent.enabled = false;
                enemies[ctr].gameObject.transform.SetPositionAndRotation(enemySpanPoints[ctr], Quaternion.identity);
                enemies[ctr].agent.enabled = true; ;
                Debug.Log(enemies[ctr].transform.position);
            }
            else
            {
                Debug.Log("No spawn point found for enemy: " + enemies[ctr].gameObject.name);
                enemies[ctr].gameObject.SetActive(false);
            }

        }
    }
}
