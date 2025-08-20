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

    [Header("Shield Indicator")]
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject shieldIndicator;
    [SerializeField] private Slider shieldDurationBar;
    [SerializeField] private TextMeshProUGUI shieldTimeText;
    private bool shieldEnabled = false;
    private bool isTrackingShield = false;
    private float shieldDuration;
    private float shieldStartTime;

    [Header("Multi-Bullet UI")]
    [SerializeField] private GameObject multiBulletIndicator;
    [SerializeField] private Slider multiBulletSlider;
    [SerializeField] private TMP_Text multiBulletStackText;

    private bool isTrackingMultiBullet = false;
    private float multiBulletStartTime;
    private float multiBulletDuration;

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = GameManager.GetInstance().scoreManager;

        menuHighScore.text = "HIGHSCORE: " + scoreManager.GetHighScore().ToString();

        GameManager.GetInstance().OnGameStart += GameStarted;
        GameManager.GetInstance().OnGameOver += GameOver;

        // Hide indicators initially
        if (rapidFireIndicator != null)
            rapidFireIndicator.SetActive(false);

        if (multiBulletIndicator != null)
            multiBulletIndicator.SetActive(false);

        if (scatterShotIndicator != null)
            scatterShotIndicator.SetActive(false);

        if (shieldIndicator != null)
            shieldIndicator.SetActive(false);
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

        if (isTrackingMultiBullet)
        {
            UpdateMultiBulletIndicator();
        }

        if (isTrackingShield)
        {
            UpdateShieldIndicator();
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

    public void StartShieldIndicator(float duration)
    {
        // Start UI tracking
        isTrackingShield = true;
        shieldEnabled = true;
        shieldStartTime = Time.time;
        shieldDuration = duration;

        // Show shield UI indicator
        if (shieldIndicator != null)
            shieldIndicator.SetActive(true);

        if (shieldDurationBar != null)
        {
            shieldDurationBar.maxValue = duration;
            shieldDurationBar.value = duration;
        }

        // Create shield visual
        if (shieldPrefab != null && player != null)
        {
            // Destroy any existing shield first
            GameObject existingShield = player.transform.Find("Shield")?.gameObject;
            if (existingShield != null)
            {
                Destroy(existingShield);
            }

            // Create new shield
            var shield = Instantiate(shieldPrefab, player.transform);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localScale = Vector3.one;
            shield.name = "Shield";

            // Set up the collider for bullet detection
            CircleCollider2D shieldCollider = shield.GetComponent<CircleCollider2D>();
            if (shieldCollider != null)
            {
                // Use trigger for bullet detection, but also add a solid collider for enemies
                shieldCollider.isTrigger = true; // Trigger for bullet detection
                shieldCollider.radius = 0.7f;
            }

            // Add another collider for enemy blocking (solid)
            CircleCollider2D enemyBlocker = shield.AddComponent<CircleCollider2D>();
            enemyBlocker.isTrigger = false; // Solid for enemy blocking
            enemyBlocker.radius = 0.7f;

            // Add the shield collision handler
            ShieldCollider shieldScript = shield.GetComponent<ShieldCollider>();
            if (shieldScript == null)
            {
                shield.AddComponent<ShieldCollider>();
            }

            Debug.Log("Shield created with bullet filtering!");
        }
    }

    public void StopShieldIndicator()
    {
        isTrackingShield = false;
        shieldEnabled = false;

        if (shieldIndicator != null)
            shieldIndicator.SetActive(false);

        // Destroy shield visual
        if (player != null)
        {
            GameObject existingShield = player.transform.Find("Shield")?.gameObject;
            if (existingShield != null)
            {
                Destroy(existingShield);
                Debug.Log("Shield deactivated and visual destroyed!");
            }
        }
    }

    private void UpdateShieldIndicator()
    {
        float timeRemaining = shieldDuration - (Time.time - shieldStartTime);

        if (timeRemaining <= 0)
        {
            StopShieldIndicator();
            return;
        }

        // Update progress bar
        if (shieldDurationBar != null)
        {
            shieldDurationBar.value = timeRemaining;
        }

        // Update time text
        if (shieldTimeText != null)
        {
            shieldTimeText.text = $"Shield: {timeRemaining:F1}s";
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

    public void StartMultiBulletIndicator(float duration, int currentStacks, int maxStacks)
    {
        isTrackingMultiBullet = true;
        multiBulletStartTime = Time.time;
        multiBulletDuration = duration;

        if (multiBulletIndicator != null)
            multiBulletIndicator.SetActive(true);

        if (multiBulletSlider != null)
        {
            multiBulletSlider.maxValue = duration;
            multiBulletSlider.value = duration;
        }

        if (multiBulletStackText != null)
        {
            multiBulletStackText.text = $"Multi-Shot: {currentStacks}/{maxStacks}";
        }
    }

    public void StopMultiBulletIndicator()
    {
        isTrackingMultiBullet = false;

        if (multiBulletIndicator != null)
            multiBulletIndicator.SetActive(false);
    }

    private void UpdateMultiBulletIndicator()
    {
        float timeRemaining = multiBulletDuration - (Time.time - multiBulletStartTime);

        if (timeRemaining <= 0)
        {
            StopMultiBulletIndicator();
            return;
        }

        if (multiBulletSlider != null)
        {
            multiBulletSlider.value = timeRemaining;
        }

        if (multiBulletStackText != null)
        {
            PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
            if (playerInput != null)
            {
                int currentStacks = playerInput.GetMultiBulletStacks();
                multiBulletStackText.text = $"Multi-Shot: {currentStacks}/5 ({timeRemaining:F1}s)";
            }
        }
    }
}
