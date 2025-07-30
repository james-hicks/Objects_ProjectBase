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

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = GameManager.GetInstance().scoreManager;

        menuHighScore.text = "HIGHSCORE: " + scoreManager.GetHighScore().ToString();

        GameManager.GetInstance().OnGameStart += GameStarted;
        GameManager.GetInstance().OnGameOver += GameOver;
    }

    //private void OnDisable()
    //{
    //    player.OnHealthSet -= SetHealthBar;
    //    player.OnHealthUpdate -= UpdateHealth;
    //}


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
}
