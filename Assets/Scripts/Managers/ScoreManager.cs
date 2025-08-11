using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    private int score;
    private int highscore;
   //[SerializeField] private int nextLevel = 2000; // score to end level

    public UnityEvent OnScoreUpdated;
    public UnityEvent OnHighScoreUpdated;

    private void Start()
    {
        highscore = PlayerPrefs.GetInt("HIGH_SCORE");
        OnHighScoreUpdated?.Invoke();
        GameManager.GetInstance().OnGameStart += OnGameStart;
    }

    public void IncrementScore(int amount)
    {
        score += amount;
        OnScoreUpdated?.Invoke();

        // if (score >= nextLevel)
        // {
        //     // Notify GameManager to go to next level 
        //     GameManager.GetInstance().StopGame();
        // }


        if (score > highscore)
        {
            highscore = score;
            OnHighScoreUpdated?.Invoke();
        }
    }

    public float GetScore()
    {
        return score;
    }

    public int GetHighScore() 
    { 
        return highscore;
    }

    public void SetHighScore()
    {
        PlayerPrefs.SetInt("HIGH_SCORE", highscore);
    }

    public void OnGameStart()
    {
        score = 0;
        OnScoreUpdated?.Invoke();
    }

}
