using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [Header("Dynamic Spawning Settings")]
    [SerializeField] private int numberOfSpawnPoints = 8;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float spawnDistance = 15f; // How far from player to spawn
    [SerializeField] private float updateInterval = 0.5f; // How often to update spawn positions

    [Header("Spawning")]
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private GameObject[] enemies;

    private Transform player;
    private Vector3[] spawnPositions;
    private bool isSpawning = false;

    void Start()
    {
        spawnPositions = new Vector3[numberOfSpawnPoints];
        StartCoroutine(UpdateSpawnPositions());
    }

    public void StartSpawning(Transform playerTransform)
    {
        player = playerTransform;
        isSpawning = true;
        StartCoroutine(SpawnEnemies());
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private IEnumerator UpdateSpawnPositions()
    {
        while (true)
        {
            if (player != null)
            {
                UpdateSpawnPoints();
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateSpawnPoints()
    {
        Vector3 playerPosition = player.position;

        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            float angle = (360f / numberOfSpawnPoints) * i * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0
            );

            spawnPositions[i] = playerPosition + direction * spawnDistance;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(1f / spawnRate);

            if (player != null && spawnPositions.Length > 0)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        if (enemies.Length == 0 || spawnPositions.Length == 0) return;

        // Choose random spawn position
        Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];

        // Choose random enemy
        GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];

        // Spawn the enemy
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    
}
