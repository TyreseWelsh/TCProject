using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public int waveNum = 0;
    public float maxWaveEnemies;
    [HideInInspector]
    public int enemiesToNextWave;

    [SerializeField]
    TMP_Text waveText;

    SpawnerController spawnController;

    private void Start()
    {
        GameObject spawnControllerObj = GameObject.Find("UnitSpawner");
        spawnController = spawnControllerObj.GetComponent<SpawnerController>();

        enemiesToNextWave = (int)maxWaveEnemies;
    }

    public void ReduceNumEnemies()
    {
        enemiesToNextWave--;

        if (enemiesToNextWave <= 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        print("NEXT WAVE");
        waveNum++;
        maxWaveEnemies *= 1.5f;
        enemiesToNextWave = (int)maxWaveEnemies;
        waveText.text = "Wave " + waveNum;
        waveText.gameObject.SetActive(true);

        StartCoroutine(WaitForNextWave());
    }

    IEnumerator WaitForNextWave()
    {
        spawnController.enableSpawning = false;
        yield return new WaitForSeconds(8);

        waveText.gameObject.SetActive(false);
        spawnController.enableSpawning = true;
    }
}