using System;
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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] Player player;


    [Header("Rapid Fire Indicator")]
    [SerializeField] private GameObject rapidFireIndicator;     
    [SerializeField] private Slider rapidFireDurationBar;       
    [SerializeField] private TextMeshProUGUI rapidFireTimeText; 

    private bool isTrackingRapidFire = false;
    private float rapidFireStartTime;
    private float rapidFireDuration;

    [Header("Scatter Shot Indicator")]
    [SerializeField] private GameObject scatterShotIndicator;
    [SerializeField] private Slider scatterShotDurationBar;
    [SerializeField] private TextMeshProUGUI scatterShotTimeText;
    private bool scatterShotEnabled = false;
    private float scatterShotDuration;
    private float scatterShotStartTime;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = GameManager.GetInstance().scoreManager;

        menuHighScore.text = "HIGHSCORE: " + scoreManager.GetHighScore().ToString();

        GameManager.GetInstance().OnGameStart += GameStarted;
        GameManager.GetInstance().OnGameOver += GameOver;

        // Hide rapid fire indicator initially
        if (rapidFireIndicator != null)
            rapidFireIndicator.SetActive(false);
    }

    public void Update()
    {
        // Update rapid fire indicator
        if (isTrackingRapidFire)
        {
            UpdateRapidFireIndicator();
        }

        if (scatterShotEnabled)
        {
            UpdateScatterShotIndicator();
        }
    }

    //tracking the ShotBarrage
    public void StartRapidFireIndicator(float duration)
    {
        isTrackingRapidFire = true;
        rapidFireStartTime = Time.time;
        rapidFireDuration = duration;

        if (rapidFireIndicator != null)
            rapidFireIndicator.SetActive(true);

        if (rapidFireDurationBar != null)
        {
            rapidFireDurationBar.maxValue = duration;
            rapidFireDurationBar.value = duration;
        }
    }

    public void StopRapidFireIndicator()
    {
        isTrackingRapidFire = false;

        if (rapidFireIndicator != null)
            rapidFireIndicator.SetActive(false);
    }

    //RapidFire indicator UI
    private void UpdateRapidFireIndicator()
    {
        float timeRemaining = rapidFireDuration - (Time.time - rapidFireStartTime);

        if (timeRemaining <= 0)
        {
            StopRapidFireIndicator();
            return;
        }

        // Update progress bar
        if (rapidFireDurationBar != null)
        {
            rapidFireDurationBar.value = timeRemaining;
        }

        // Update time text
        if (rapidFireTimeText != null)
        {
            rapidFireTimeText.text = $"Rapid Fire: {timeRemaining:F1}s";
        }
    }

    public void StartScatterShotIndicator(float duration)
    {
        scatterShotEnabled = true;
        scatterShotStartTime = Time.time;
        scatterShotDuration = duration;

        if (scatterShotIndicator != null)
            scatterShotIndicator.SetActive(true);

        if (scatterShotDurationBar != null)
        {
            scatterShotDurationBar.maxValue = duration;
            scatterShotDurationBar.value = duration;
        }
    }

    public void StopScatterShotIndicator()
    {
        scatterShotEnabled = false;
        if (scatterShotIndicator != null)
            scatterShotIndicator.SetActive(false);
    }

    private void UpdateScatterShotIndicator()
    {
        float timeRemaining = scatterShotDuration - (Time.time - scatterShotStartTime);

        if (timeRemaining <= 0)
        {
            StopScatterShotIndicator();
            return;
        }

        // Update progress bar
        if (scatterShotDurationBar != null)
        {
            scatterShotDurationBar.value = timeRemaining;
        }

        // Update time text
        if (scatterShotTimeText != null)
        {
            scatterShotTimeText.text = $"Scatter Shot: {timeRemaining:F1}s";
        }
    }

    public void SetHealthBar(float maxHealth, float currentHealth)
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
        Debug.Log(scoreManager.GetHighScore().ToString());

        highScoreText.text = "HighScore: " + scoreManager.GetHighScore().ToString();
        menuHighScore.text = "HIGHSCORE: " + scoreManager.GetHighScore().ToString();
    }

    public void GameStarted()
    {
        player = GameManager.GetInstance().GetPlayer();

        player.OnHealthSet += SetHealthBar;
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

    public void StartShieldIndicator(float duration)
    {
        
    }
}
