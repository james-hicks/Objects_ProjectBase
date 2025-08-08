using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Dynamic Spawning")]
    [SerializeField] private int numberOfSpawnPoints = 8;
    [SerializeField] private float spawnDistance = 15f;
    [SerializeField] private float spawnRate = 2f;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private PickupSpawner pickupSpawner;

    [Space]
    [SerializeField] private GameObject playerPrefab;

    [Space]
    [SerializeField] private GameObject[] enemies;

    private Player player;
    private Vector3[] dynamicSpawnPositions;

    public string targetTag = "Effects";

    private bool isEnemySpawning = false;
    private Coroutine spawnerCoroutine;

    public ScoreManager scoreManager;
    public bool GameOver = false;

    public Action OnGameStart;
    public Action OnGameOver;

    /// <summary>
    /// Singleton GameManager
    /// </summary>
    private static GameManager instance;

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        dynamicSpawnPositions = new Vector3[numberOfSpawnPoints];
    }

       

    private void Update()
    {
        if (GameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    private void UpdateDynamicSpawnPositions()
    {
        if (player == null) return;

        Vector3 playerPosition = player.transform.position;

        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            float angle = (360f / numberOfSpawnPoints) * i * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            dynamicSpawnPositions[i] = playerPosition + direction * spawnDistance;
        }
    }

    private void CreateEnemy()
    {
        if (enemies.Length == 0)
        {            
            return;
        }

        if (player == null)
        {            
            return;
        }

        UpdateDynamicSpawnPositions();
        Vector3 spawnPos = dynamicSpawnPositions[UnityEngine.Random.Range(0, dynamicSpawnPositions.Length)];
        GameObject enemyPrefab = enemies[UnityEngine.Random.Range(0, enemies.Length)];

        if (enemyPrefab == null)
        {            
            return;
        }

        GameObject tempEnemy = Instantiate(enemyPrefab);
        tempEnemy.transform.position = spawnPos;

    }

    private IEnumerator EnemySpawner()
    {
        int spawnCount = 0;

        while (isEnemySpawning && !GameOver && player != null)
        {
            float waitTime = 1f / spawnRate;            

            yield return new WaitForSeconds(waitTime);

            if (isEnemySpawning && !GameOver && player != null)
            {                
                CreateEnemy();
                spawnCount++;
            }
            else
            {
              
                break;
            }
        }

    }

    public void DisableSpawning()
    {        
        isEnemySpawning = false;

        if (spawnerCoroutine != null)
        {
            StopCoroutine(spawnerCoroutine);
            spawnerCoroutine = null;           
        }
    }

    public void NotifyDeath(Enemy enemy)
    {
        pickupSpawner.SpawnPickup(enemy.transform.position);
    }

    public void StartGame()
    {
        if (playerPrefab == null)
        {            
            return;
        }

        if (enemies.Length == 0)
        {            
            return;
        }

        DisableSpawning();

        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();
        GameOver = false;


        // Set up camera
        CinemachineCamera cmCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cmCamera != null)
        {
            cmCamera.Follow = player.transform;
        }

        if (player != null)
        {
            player.OnDeath += StopGame;
        }

        OnGameStart?.Invoke();

        StartCoroutine(GameStarter());
    }

    IEnumerator GameStarter()
    {
        yield return new WaitForSeconds(2f);

        if (player == null)
        {            
            yield break;
        }

        if (GameOver)
        {
            yield break;
        }

        isEnemySpawning = true;

        spawnerCoroutine = StartCoroutine(EnemySpawner());

    }

    public void StopGame()
    {
        DisableSpawning();
        if (scoreManager != null)
        {
            scoreManager.SetHighScore();
        }
        StartCoroutine(GameStopper());
    }

    IEnumerator GameStopper()
    {
        yield return new WaitForSeconds(2f);
        GameOver = true;

        foreach (Enemy item in FindObjectsOfType(typeof(Enemy)))
        {
            Destroy(item.gameObject);
        }

        foreach (Pickup item in FindObjectsOfType(typeof(Pickup)))
        {
            Destroy(item.gameObject);
        }

        GameObject[] gameObjectsToDestroy = GameObject.FindGameObjectsWithTag("Effects");
        foreach (GameObject go in gameObjectsToDestroy)
        {
            Destroy(go);
        }

        OnGameOver?.Invoke();
    }

    public Player GetPlayer() { return player; }
}
