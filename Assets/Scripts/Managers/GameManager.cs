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

    public Action OnGameStart;
    public Action OnGameOver;

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

    private void CreateEnemy()
    {
        // spawn a random enemy from the enemies array
        tempEnemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Length)]);
        // at a random position from the spawnPositions array
        tempEnemy.transform.position = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;
    }

    private IEnumerator EnemySpawner()
    {
        while (isEnemySpawning)
        {
            yield return new WaitForSeconds(1f / spawnRate); // wait based on spawn rate
            CreateEnemy(); // spawn an enemy
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
        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();
        GameOver = false;
        player.OnDeath += StopGame; // when notified of player's death, stop game

        OnGameStart.Invoke();
        StartCoroutine(GameStarter());
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

        OnGameOver?.Invoke(); // call game over
    }


    public Player GetPlayer() { return player; } // returns reference to player
}
