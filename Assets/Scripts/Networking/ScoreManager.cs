using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
    private int highscore;

    public bool isSpawned = false;
    public override void Spawned()
    {
        isSpawned = true;

        highscore = PlayerPrefs.GetInt("highscore", 0);
        Manager.instance.highscoreText.text = highscore.ToString();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcUpdateScore(int newScore)
    {
        Manager.instance.scoreText.text = newScore.ToString();
        ManageHighscore(newScore);
    }

    private void ManageHighscore(int newScore)
    {
        if (newScore > highscore)
        {
            highscore = newScore;
            Manager.instance.highscoreText.text = highscore.ToString();
            PlayerPrefs.SetInt("highscore", highscore);
        }
    }
}
