using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField]
    GameObject unit;
    [SerializeField]
    Transform unitTarget;
    [SerializeField]
    GameObject spawnPoint;

    [SerializeField]
    int unitHealth;
    [SerializeField]
    float unitMoveSpeed;

    [SerializeField]
    float spawnRate;

    UnitManager unitManager;
    GameManagerScript gameManagerScript;
    public bool enableSpawning = true;
    int unitsSpawned;
    bool spawned;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        unitManager = playerManager.GetComponent<UnitManager>();

        GameObject gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enableSpawning && !spawned)
        {
            if(unitsSpawned < gameManagerScript.maxWaveEnemies)
            {
                StartCoroutine(SpawnUnit());
            }
        }
    }

    IEnumerator SpawnUnit()
    {
        spawned = true;
        yield return new WaitForSeconds(spawnRate);

        GameObject newUnit = Instantiate(unit, spawnPoint.transform.position, Quaternion.identity);
        UnitScript unitScript = newUnit.GetComponent<UnitScript>();
        UnitMovement unitMovementScript = newUnit.GetComponent<UnitMovement>();


        unitScript.Init(unitHealth, unitMoveSpeed);
        unitMovementScript.SetTarget(unitTarget);

        unitManager.unitList.Add(newUnit);
        unitsSpawned++;

        spawned = false;
    }
}
