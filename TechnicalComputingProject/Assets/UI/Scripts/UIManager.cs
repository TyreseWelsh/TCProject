using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text soulsText;
    int playerSouls = 0;

    public void SetSouls(int newSouls)
    {
        playerSouls += newSouls;
        soulsText.text = "Souls: " + playerSouls;
    }

    public int GetScore()
    {
        return playerSouls;
    }
}
