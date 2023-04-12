using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    bool alive = false;
    bool slowed = false;

    // Start is called before the first frame update
    void Start()
    {
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        unitHealth -= damage;

        unitHealth--;
        if (unitHealth <= 0)
        {
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("TurretAttack"))
        {
            unitHealth--;
            if(unitHealth <= 0 )
            {
                gameObject.SetActive(false);
                //alive = false;
                //Destroy(gameObject);
            }
        }
    }
}
