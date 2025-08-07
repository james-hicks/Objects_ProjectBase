using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private float spawnRate;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;


    [SerializeField] private PickupSpawner pickupSpawner;

    [Space]

    [SerializeField] private GameObject playerPrefab;

    [Space]
    [SerializeField] private GameObject[] enemies;

    private Player player;

    public string targetTag = "Effects";
    

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
            Debug.Log("There is no player in the scene.");
        }
    }

    public void StartGame()
    {
        
        player = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player>();
        GameOver = false;

        // Find and set the Cinemachine camera to follow the player
        CinemachineCamera cmCamera = FindFirstObjectByType<CinemachineCamera>();
        if (cmCamera != null)
        {
            cmCamera.Follow = player.transform;
            Debug.Log("Set Cinemachine camera to follow player");
        }
        else
        {
            Debug.LogError("No CinemachineCamera found in scene");
        }

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
