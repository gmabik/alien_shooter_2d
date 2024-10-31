using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{
    #region singleton
    public static Manager instance;
    private void Awake()
    {
        if(instance != null) Destroy(instance);
        instance = this;
    }
    #endregion

    private int score;
    [SerializeField] private TMP_Text scoreText;
    private int highscore;
    [SerializeField] private TMP_Text highscoreText;
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        highscoreText.text = "Highscore: " + highscore;
    }

    void FixedUpdate()
    {
        AddScore(1);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
        ManageHighscore();
    }

    private void ManageHighscore()
    {
        if (score < highscore) return;
        highscore = score;
        highscoreText.text = "Highscore: " + highscore;
        PlayerPrefs.SetInt("highscore", highscore);
    }
}
