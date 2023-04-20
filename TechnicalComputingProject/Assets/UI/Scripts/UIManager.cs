using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    PlaceObject placeObjectScripts;

    public List<GameObject> turretTypes;
    [SerializeField]
    List<TMP_Text> turretUIStats;

    bool paused = false;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //for (int i = 0; i < turretUIStats.Count; i++)
        //{
        //    TurretController currentTurretController = placeObjectScripts.turretMeshes[i].GetComponent<TurretController>();
        //    turretUIStats[i].text = "Cost: " + currentTurretController.turretCost  + "\n" + "Damage: " + currentTurretController.turretDamage + "\n" + "Fire Rate: " + currentTurretController.fireRate;
        //}
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                SetPaused(false);
            }
            else
            {
                SetPaused(true);
            }
        }
    }

    public void GoToInstructions()
    {
        SceneManager.LoadScene("InstructionsScene");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public bool GetPaused()
    {
        return paused;
    }

    public void SetPaused(bool _paused)
    {
        paused = _paused;
        pauseMenu.SetActive(paused);
        if (paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
