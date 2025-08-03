using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private float spawnRate;

    [SerializeField] private PickupSpawner pickupSpawner;

    [Space]

    [SerializeField] private GameObject playerPrefab;

    [Space]
    [SerializeField] private GameObject[] enemies;

    private Player player;

    private bool isEnemySpawning = true;

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
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    CreateEnemy();
        //}

        if (GameOver & Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private GameObject tempEnemy;

    private void CreateEnemy()
    {
        tempEnemy = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Length)]);
        tempEnemy.transform.position = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)].position;
    }

    private IEnumerator EnemySpawner()
    {
        while (isEnemySpawning)
        {
            yield return new WaitForSeconds(1f / spawnRate);
            CreateEnemy();
        }
    }

    public void DisableSpawning()
    {
        isEnemySpawning = false;
    }

    public void NotifyDeath(Enemy enemy)
    {
        pickupSpawner.SpawnPickup(enemy.transform.position);
    }


    public void FindPlayer()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        } catch (NullReferenceException e)
        {
            Debug.Log("There is no player in the scene. Details: " + e.Message);
        }
    }

    public void StartGame()
    {
        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();
        GameOver = false;

        player.OnDeath += StopGame;

        OnGameStart.Invoke();
        StartCoroutine(GameStarter());
    }

    IEnumerator GameStarter()
    {
        yield return new WaitForSeconds(2f);
        isEnemySpawning = true;
        StartCoroutine(EnemySpawner());
    }

    public void StopGame()
    {
        isEnemySpawning = false;
        scoreManager.SetHighScore();


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



    public Player GetPlayer() { return player; }
}
