using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages core game behavior including enemy spawning, game restart,
/// player reference tracking, and singleton access.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPositions; // array to hold spawn points
    [SerializeField] private float spawnRate = 1f; // rate at which enemies spawn

    [SerializeField] private PickupSpawner pickupSpawner; // reference to pickup spawner
    [SerializeField] private GameObject playerPrefab; // reference to playerSpawn

    [Space]
    [SerializeField] private GameObject[] enemies; // array to hold enemy prefabs
    private GameObject tempEnemy; // temporary reference to spawned enemy
    private bool isEnemySpawning = true; // controls whether enemies should keep spawning
    private Player player; // reference to the player
    public ScoreManager scoreManager; // reference to the score manager
    public UIManager uiManager;// ref to UI Manager
    public bool GameOver = false; // flag to track if game is over
    private Coroutine enemySpawnerRoutine; // Add this at the top with your fields

    public Action OnGameStart;
    public Action OnGameOver;

    AudioManager audioManager;


    /// <summary>
    /// Singleton Instance
    /// </summary>
    private static GameManager instance; // static instance for singleton access

    public static GameManager GetInstance()
    {
        return instance; // returns the singleton instance
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (instance != null)
        {
            Destroy(gameObject); // prevent multiple instances
        }

        instance = this; // assign current instance

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>(); 
        }
    }

    public void CreateEnemy()
    {
        // spawn a random enemy from the enemies array
        tempEnemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Length)]);
        // at a random position from the spawnPositions array
        tempEnemy.transform.position = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;
    }

    private IEnumerator EnemySpawner()
    {
        GameObject spawnPoints = GameObject.Find("SpawnPoints");
        while (isEnemySpawning && player != null && spawnPoints != null)
        {
            yield return new WaitForSeconds(1f / spawnRate); // wait based on spawn rate
            CreateEnemy(); // spawn an enemy

            // Only move spawnPoints if player still exists
            if (player != null)
                spawnPoints.transform.position = player.transform.position;

            yield return null;
        }
    }

    public void DisableSpawning()
    {
        isEnemySpawning = false; // stop enemy spawning
    }

    public void NotifyDeath(Enemy enemy)
    {
        // spawn a pickup at the enemy's death position
        pickupSpawner.SpawnPickup(enemy.transform.position);
    }

    public void FindPlayer()
    {
        try
        {
            // find player by tag and get Player component
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }
        catch (NullReferenceException e)
        {
            // log warning if player not found
            Debug.LogWarning("There is no player in the scene. Details: " + e.Message);
        }
    }

    public void StartGame()
    {
        audioManager.PlaySFX(audioManager.gameStart);
        // Check if there is already a player and destroy it
        if (player != null)
        {
            Destroy(player.gameObject); // Destroy the existing player
        }

        // Instantiate a new player at the start
        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();

        GameOver = false;

        // Set camera to follow the new player
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(player.transform);
        }

        // Attach death event to stop the game when the player dies
        player.OnDeath += StopGame;

        OnGameStart.Invoke();
        StartCoroutine(GameStarter());
        
        // Start the enemy spawner coroutine and keep a reference
        if (enemySpawnerRoutine != null)
            StopCoroutine(enemySpawnerRoutine);
        enemySpawnerRoutine = StartCoroutine(EnemySpawner());
    }


    IEnumerator GameStarter()
    {
        yield return new WaitForSeconds(2f); //give 2 secs before game actually starts
        isEnemySpawning = true; // start enemy spawning
        StartCoroutine(EnemySpawner());
        
    }

    public void StopGame()
    {
        isEnemySpawning = false; // stop spawning enemies
        scoreManager.SetHighScore(); // set High Score
        

        StartCoroutine(GameStopper());
    }

    IEnumerator GameStopper()
    {
        isEnemySpawning = false;

        yield return new WaitForSeconds(2f); //give 2 secs before game really ends
        GameOver = true;
        

        // Re-parent SpawnPoints to scene root so they're not destroyed with player
        GameObject spawnPoints = GameObject.Find("SpawnPoints");
        if (spawnPoints != null)
        {
            spawnPoints.transform.SetParent(null); // set parent to root
        }

        // Delete all enemies from screen
        foreach (Enemy item in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Destroy(item.gameObject);
        }

        // Delete all bullets from screen
        foreach (Bullet item in FindObjectsByType<Bullet>(FindObjectsSortMode.None))
        {
            Destroy(item.gameObject);
        }

        // Delete all pickups from screen
        foreach (Pickup item in FindObjectsByType<Pickup>(FindObjectsSortMode.None))
        {
            Destroy(item.gameObject);
        }

        Debug.Log("GameOver triggered! StackTrace: " + Environment.StackTrace);
        OnGameOver?.Invoke(); // call game over
        audioManager.PlaySFX(audioManager.gameOver);
    }


    public Player GetPlayer() { return player; } // returns reference to player
}
