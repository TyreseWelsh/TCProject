using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public int waveNum = 1;
    public float maxWaveEnemies;
    [HideInInspector]
    public int enemiesToNextWave;

    [SerializeField]
    TMP_Text waveText;

    SpawnerController spawnController;

    private void Awake()
    {
        GameObject spawnControllerObj = GameObject.Find("UnitSpawner");
        spawnController = spawnControllerObj.GetComponent<SpawnerController>();

        enemiesToNextWave = (int)maxWaveEnemies;

        waveText.gameObject.SetActive(true);
        StartCoroutine(WaitForNextWave(4));
    }

    private void Start()
    {
        print(spawnController.enableSpawning);
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
        if(waveNum < 10)
        {
            print("NEXT WAVE");
            waveNum++;

            spawnController.unitsSpawned = 0;
            spawnController.unitHealth *= Mathf.FloorToInt(1.5f);
            spawnController.unitMoveSpeed *= 1.3f;
            spawnController.spawnRate *= 0.8f;

            maxWaveEnemies += 10;
            enemiesToNextWave = (int)maxWaveEnemies;
            print("Enemies: " + enemiesToNextWave);
            waveText.text = "Wave " + waveNum;
            waveText.gameObject.SetActive(true);

            StartCoroutine(WaitForNextWave(8));
        }
        else
        {
            SceneManager.LoadScene("EndScreen");
        }
    }

    IEnumerator WaitForNextWave(int waitTime)
    {
        spawnController.enableSpawning = false;
        yield return new WaitForSeconds(waitTime);

        waveText.gameObject.SetActive(false);
        spawnController.enableSpawning = true;
    }
}