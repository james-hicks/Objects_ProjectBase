using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    [Range(1, 100)]
    public int spawnWeight = 10; // Higher = more frequent
    [Tooltip("Common = 30, Uncommon = 15, Rare = 5, Very Rare = 1")]
    public string rarity = "Common";
}

public class EnemySpawning : MonoBehaviour
{
    [Header("Dynamic Spawning Settings")]
    [SerializeField] private int numberOfSpawnPoints = 8;
    [SerializeField] private float spawnDistance = 15f;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float updateInterval = 0.5f;

    [Header("Enemy Spawn Data")]
    [SerializeField] private EnemySpawnData[] enemySpawnData;

    [Header("Legacy Enemy Prefabs (Optional)")]
    [SerializeField] private GameObject[] enemies; // Keep for backward compatibility

    [Header("References")]
    [SerializeField] private PickupSpawner pickupSpawner;

    private Transform player;
    private Vector3[] dynamicSpawnPositions;
    private bool isEnemySpawning = false;
    private Coroutine spawnerCoroutine;
    private Coroutine positionUpdateCoroutine;
    private int totalSpawnWeight;

    void Start()
    {
        dynamicSpawnPositions = new Vector3[numberOfSpawnPoints];

        // Auto-find PickupSpawner if not assigned
        if (pickupSpawner == null)
        {
            pickupSpawner = FindFirstObjectByType<PickupSpawner>();
        }

        // Calculate total spawn weight
        CalculateTotalSpawnWeight();
    }

    private void CalculateTotalSpawnWeight()
    {
        totalSpawnWeight = 0;

        if (enemySpawnData != null && enemySpawnData.Length > 0)
        {
            foreach (var spawnData in enemySpawnData)
            {
                if (spawnData.enemyPrefab != null)
                {
                    totalSpawnWeight += spawnData.spawnWeight;
                }
            }
        }

        Debug.Log($"Total spawn weight calculated: {totalSpawnWeight}");
    }

    public void StartSpawning(Transform playerTransform)
    {
        player = playerTransform;
        isEnemySpawning = true;

        // Start updating spawn positions
        if (positionUpdateCoroutine != null)
        {
            StopCoroutine(positionUpdateCoroutine);
        }
        positionUpdateCoroutine = StartCoroutine(UpdateSpawnPositionsContinuously());

        // Start spawning enemies after a delay
        if (spawnerCoroutine != null)
        {
            StopCoroutine(spawnerCoroutine);
        }
        spawnerCoroutine = StartCoroutine(DelayedSpawning());
    }

    public void StopSpawning()
    {
        isEnemySpawning = false;

        if (spawnerCoroutine != null)
        {
            StopCoroutine(spawnerCoroutine);
            spawnerCoroutine = null;
        }

        if (positionUpdateCoroutine != null)
        {
            StopCoroutine(positionUpdateCoroutine);
            positionUpdateCoroutine = null;
        }
    }

    private IEnumerator DelayedSpawning()
    {
        // Wait before starting to spawn enemies
        yield return new WaitForSeconds(2f);

        if (player == null || !isEnemySpawning)
        {
            yield break;
        }

        // Start the main spawning coroutine
        spawnerCoroutine = StartCoroutine(EnemySpawner());
    }

    private IEnumerator EnemySpawner()
    {
        while (isEnemySpawning && player != null)
        {
            float waitTime = 1f / spawnRate;
            yield return new WaitForSeconds(waitTime);

            if (isEnemySpawning && player != null)
            {
                CreateEnemy();
            }
        }
    }

    private IEnumerator UpdateSpawnPositionsContinuously()
    {
        while (isEnemySpawning && player != null)
        {
            UpdateDynamicSpawnPositions();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateDynamicSpawnPositions()
    {
        if (player == null) return;

        Vector3 playerPosition = player.position;

        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            float angle = (360f / numberOfSpawnPoints) * i * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            dynamicSpawnPositions[i] = playerPosition + direction * spawnDistance;
        }
    }

    private void CreateEnemy()
    {
        GameObject enemyPrefab = null;

        // Use new weighted system if available
        if (enemySpawnData != null && enemySpawnData.Length > 0 && totalSpawnWeight > 0)
        {
            enemyPrefab = GetWeightedRandomEnemy();
        }
        // Fallback to old system
        else if (enemies != null && enemies.Length > 0)
        {
            enemyPrefab = enemies[Random.Range(0, enemies.Length)];
        }

        if (enemyPrefab == null)
        {
            Debug.LogWarning("No valid enemy prefab found to spawn!");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("Player reference is null in EnemySpawning!");
            return;
        }

        if (dynamicSpawnPositions.Length == 0)
        {
            Debug.LogWarning("No spawn positions available!");
            return;
        }

        // Choose random spawn position
        Vector3 spawnPos = dynamicSpawnPositions[Random.Range(0, dynamicSpawnPositions.Length)];

        // Spawn the enemy
        GameObject tempEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private GameObject GetWeightedRandomEnemy()
    {
        if (totalSpawnWeight <= 0) return null;

        int randomWeight = Random.Range(0, totalSpawnWeight);
        int currentWeight = 0;

        foreach (var spawnData in enemySpawnData)
        {
            if (spawnData.enemyPrefab == null) continue;

            currentWeight += spawnData.spawnWeight;
            if (randomWeight < currentWeight)
            {
                return spawnData.enemyPrefab;
            }
        }

        // Fallback to first valid enemy
        foreach (var spawnData in enemySpawnData)
        {
            if (spawnData.enemyPrefab != null)
            {
                return spawnData.enemyPrefab;
            }
        }

        return null;
    }

    // Called by enemies when they die to spawn pickups
    public void NotifyEnemyDeath(Vector3 deathPosition)
    {
        if (pickupSpawner != null)
        {
            pickupSpawner.SpawnPickup(deathPosition);
        }
    }

    // Cleanup all enemies and effects
    public void CleanupEnemies()
    {
        // Destroy all enemies
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        // Destroy all pickups
        Pickup[] pickups = FindObjectsOfType<Pickup>();
        foreach (Pickup pickup in pickups)
        {
            if (pickup != null)
            {
                Destroy(pickup.gameObject);
            }
        }

        // Destroy effects
        GameObject[] effects = GameObject.FindGameObjectsWithTag("Effects");
        foreach (GameObject effect in effects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
    }

    // Update spawn rate during gameplay
    public void SetSpawnRate(float newSpawnRate)
    {
        spawnRate = newSpawnRate;
    }

    // Get current spawning status
    public bool IsSpawning()
    {
        return isEnemySpawning;
    }

    // Helper method to add enemies at runtime
    public void AddEnemyType(GameObject enemyPrefab, int spawnWeight)
    {
        // This could be used for dynamic enemy addition during gameplay
        Debug.Log($"Added enemy type {enemyPrefab.name} with weight {spawnWeight}");
    }
}
