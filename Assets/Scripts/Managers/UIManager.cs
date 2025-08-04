using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject MenuScreen;

    [SerializeField] private TextMeshProUGUI menuHighScore;
    [SerializeField] private TextMeshProUGUI gameOverScore;

    [Header("Player HUD")]
    [SerializeField] private Slider healthBar;
    [SerializeField] Player player;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Power Ups")]

    [SerializeField] private Transform nukePanel; // UI for nuke icons
    [SerializeField] private GameObject nukeIconPrefab; // Assign nuke prefab

    private ScoreManager scoreManager;
    private InventoryManager inventory;

    private void Start()
    {
        scoreManager = GameManager.GetInstance().scoreManager;
        menuHighScore.text = "HIGH SCORE: " + scoreManager.GetHighScore().ToString(); // set high score in menu
        GameManager.GetInstance().OnGameStart += GameStarted;
        GameManager.GetInstance().OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        player.OnHealthSet -= setHealthBar;
        player.OnHealthUpdate -= UpdateHealth;
    }

    public void setHealthBar(float maxHealth, float currentHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        healthBar.value = currentHealth;
    }

    public void UpdateScore()
    {
        scoreText.text = "Score: " + GameManager.GetInstance().scoreManager.GetScore().ToString();
    }

    public void UpdateHighScore()
    {
        highScoreText.text = "High Score: " + scoreManager.GetHighScore().ToString();
        menuHighScore.text = "HIGH SCORE: " + scoreManager.GetHighScore().ToString();
    }

    public void UpdateNukeCount(int nukeCount)
    {
        // Clear old icons
        foreach (Transform child in nukePanel)
        {
            Destroy(child.gameObject);
        }

        // Add one icon per nuke
        for (int i = 0; i < nukeCount; i++)
        {
            Instantiate(nukeIconPrefab, nukePanel);
        }
    }

    public void GameStarted()
    {
        player = GameManager.GetInstance().GetPlayer();
        player.OnHealthSet += setHealthBar;
        player.OnHealthUpdate += UpdateHealth;

        MenuScreen.SetActive(false);
        GameOverScreen.SetActive(false);

    }

    public void GameOver()
    {
        MenuScreen.SetActive(false);
        GameOverScreen.SetActive(true);

        gameOverScore.text = "SCORE: " + GameManager.GetInstance().scoreManager.GetScore().ToString();
    }

    public void Menu()
    {
        MenuScreen.SetActive(true);
        GameOverScreen.SetActive(false);
    }
}
