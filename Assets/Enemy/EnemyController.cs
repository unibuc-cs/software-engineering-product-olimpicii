using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float pullStrength;
    public float distanceToPlayer;

    public GameObject enemyPrefab;
    public GameObject player;
    private List<GameObject> enemies = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Update()
    {
        if (enemies.Count > 0)
        {
            PullEnemiesToPlayer();
        }
    }

    public void SpawnEnemies(int count)
    {
        int gridRows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int gridCols = Mathf.CeilToInt((float)count / gridRows);
        float spacing = 1.8f;

        for (int i = 0; i < count; i++)
        {
            int row = i / gridCols;
            int col = i % gridCols;

            Vector3 spawnOffset = new Vector3((col - gridCols / 2f) * spacing, 0, (row - gridRows / 2f) * spacing);
            Vector3 spawnPosition = transform.position + spawnOffset;


            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.SetParent(null);
            enemies.Add(enemy);
        }
    }


    private void PullEnemiesToPlayer()
    {
        for (int i = 0; i <= enemies.Count - 1; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
                i--;
                continue;
            }
            GameObject enemy = enemies[i];

            Rigidbody rb = enemy.GetComponent<Rigidbody>();

            Vector3 direction = (player.transform.position - rb.position).normalized;
            rb.AddForce(direction * pullStrength, ForceMode.Impulse);
        }
        
    }
}
