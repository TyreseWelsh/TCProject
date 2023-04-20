using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEnd : MonoBehaviour
{
    PlayerManager playerManagerScript;

    private void Awake()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Unit"))
        {
            Destroy(other.gameObject);
            playerManagerScript.TakeDamage(2);
        }
    }
}
