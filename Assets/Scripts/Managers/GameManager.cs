using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawning References")]
    [SerializeField] private EnemySpawning enemySpawning;
    [SerializeField] private PickupSpawner pickupSpawner;

    private Player player;
    public bool GameOver = false;

    public ScoreManager scoreManager;

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

        // Auto-find components if not assigned
        if (enemySpawning == null)
        {
            enemySpawning = GetComponent<EnemySpawning>();
        }

        if (pickupSpawner == null)
        {
            pickupSpawner = FindFirstObjectByType<PickupSpawner>();
        }
    }

    private void Update()
    {
        if (GameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void NotifyDeath(Enemy enemy)
    {
        if (enemySpawning != null)
        {
            enemySpawning.NotifyEnemyDeath(enemy.transform.position);
        }
    }

    public void StartGame()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned!");
            return;
        }

        if (enemySpawning == null)
        {
            Debug.LogError("EnemySpawning component not found!");
            return;
        }

        // Stop any existing spawning
        enemySpawning.StopSpawning();

        // Create player
        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();
        GameOver = false;

        // Set up camera
        CinemachineCamera cmCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cmCamera != null)
        {
            cmCamera.Follow = player.transform;
        }

        // Subscribe to player death
        if (player != null)
        {
            player.OnDeath += StopGame;
        }

        // Notify game started
        OnGameStart?.Invoke();

        // Start enemy spawning
        StartCoroutine(StartGameWithDelay());
    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Small delay to ensure everything is set up

        if (player != null && enemySpawning != null && !GameOver)
        {
            enemySpawning.StartSpawning(player.transform);
            Debug.Log("Enemy spawning started!");
        }
    }

    public void StopGame()
    {
        // Stop enemy spawning
        if (enemySpawning != null)
        {
            enemySpawning.StopSpawning();
        }

        // Update high score
        if (scoreManager != null)
        {
            scoreManager.SetHighScore();
        }

        StartCoroutine(GameStopper());
    }

    private IEnumerator GameStopper()
    {
        yield return new WaitForSeconds(2f);
        GameOver = true;

        // Clean up all enemies and effects
        if (enemySpawning != null)
        {
            enemySpawning.CleanupEnemies();
        }

        OnGameOver?.Invoke();
    }

    public Player GetPlayer()
    {
        return player;
    }
}
