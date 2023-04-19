using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BasicProjectile : MonoBehaviour
{
    float speed;
    Vector3 targetPos;
    int damage = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void Init(float _speed, Vector3 _spawnPos, Vector3 _targetPos, int _damage)
    {
        speed = _speed;
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
