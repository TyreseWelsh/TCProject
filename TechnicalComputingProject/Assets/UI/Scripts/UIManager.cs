using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    int playerScore = 0;

    public void SetScore(int newScore)
    {
        playerScore += newScore;
        scoreText.text = "Score: " + playerScore;
    }

    public int GetScore()
    {
        return playerScore;
    }
}
