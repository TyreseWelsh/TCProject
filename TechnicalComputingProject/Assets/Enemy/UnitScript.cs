using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//public interface IDamageable
//{
//    void TakeDamage(int damage);
//}

public class UnitScript : MonoBehaviour, IDamageable
{
    [SerializeField]
    int id;
    [SerializeField]
    int unitHealth = 0;

    GameManagerScript gameManagerScript;

    PlayerManager playerManagerScript;

    GameObject nodeGridObj;
    NodeGrid nodeGridScript;

    UnitMovement unitMovementScript;

    [SerializeField]
    GameObject slowingEffect;

    private void Awake()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();

        nodeGridObj = GameObject.Find("PathfindingGrid");
        nodeGridScript = nodeGridObj.GetComponent<NodeGrid>();

        GameObject playerManager = GameObject.Find("PlayerManager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();

        unitMovementScript = gameObject.gameObject.GetComponent<UnitMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x == nodeGridScript.flowTargetTransform.position.x && transform.position.z == nodeGridScript.flowTargetTransform.position.z)
        {
            playerManagerScript.TakeDamage(2);
            Destroy(gameObject);
        }
    }

    public void Init(int _health, float _speed)
    {
        unitHealth = _health;
        UnitMovement movementScript = this.GetComponent<UnitMovement>();
        movementScript.SetMoveSpeed(_speed);
    }

    public void TakeDamage(int damage)
    {
        unitHealth -= damage;

        unitHealth--;
        if (unitHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SlowingTurret>() != null)
        {
            if (gameObject.transform.Find("SlowingEffect(Clone)") == null)
            {
                GameObject slowObject = Instantiate(slowingEffect, gameObject.transform.position, Quaternion.identity);
                slowObject.transform.parent = gameObject.transform;
            }
            unitMovementScript.speed /= 1.2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SlowingTurret>() != null)
        {
            if (gameObject.transform.Find("SlowingEffect(Clone)") != null)
            {
                Destroy(gameObject.transform.Find("SlowingEffect(Clone)").gameObject);
            }
            unitMovementScript.speed *= 1.2f;
        }
    }

    private void OnDestroy()
    {
        print("Enemy Destryed");
        gameManagerScript.ReduceNumEnemies();
    }
}
