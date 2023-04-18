using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretProjectile : MonoBehaviour
{
    enum ProjectileType
    {
        Basic
    }
    ProjectileType projectileType = ProjectileType.Basic;
    float speed;
    Vector3 spawnPos;
    Vector3 targetPos;
    int damage = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Projectile Created");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void Init(int _projectileType, float _speed, Vector3 _spawnPos, Vector3 _targetPos, int _damage)
    {
        switch(projectileType) 
        {
            case (0):
                projectileType = ProjectileType.Basic;
                break;
            default:
                projectileType = ProjectileType.Basic;
                break;
        }

        speed = _speed;
        spawnPos = _spawnPos;
        targetPos = _targetPos;
        damage = _damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Unit"))
        {
            UnitScript unitScript = collision.gameObject.GetComponent<UnitScript>();
            if (unitScript is IDamageable)
            {
                unitScript.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
