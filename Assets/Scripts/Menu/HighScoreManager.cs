using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    int score = 0;
    int highScore = 0;
    [SerializeField] Text[] highScoreText;
    [SerializeField] Text scoreText;

    private void Start()
    {
        foreach(Text t in highScoreText)
            t.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    public void ResetCounter()
    {
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }

    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        foreach (Text t in highScoreText)
            t.text = "High Score: " + highScore;
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0); highScore = 0;

        foreach (Text t in highScoreText)
            t.text = "High Score: " + highScore;
    }
}
