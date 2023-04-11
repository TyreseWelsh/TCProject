using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    [SerializeField]
    int unitHealth = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("TurretAttack"))
        {
            unitHealth--;
            if(unitHealth <= 0 )
            {
                Destroy(gameObject);
            }
        }
    }
}
