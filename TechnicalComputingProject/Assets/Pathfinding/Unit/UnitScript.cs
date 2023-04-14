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

    UnitManager unitManager;

    private void Awake()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        unitManager = playerManager.GetComponent<UnitManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void OnDestroy()
    {
        unitManager.unitList.Remove(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
    //    if(collision.gameObject.CompareTag("TurretAttack"))
    //    {
    //        unitHealth--;
    //        if(unitHealth <= 0 )
    //        {
    //            gameObject.SetActive(false);
    //            //alive = false;
    //            //Destroy(gameObject);
    //        }
    //    }
    }
}
