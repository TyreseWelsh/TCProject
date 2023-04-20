using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantTurret : MonoBehaviour
{
    public float fireRate = 1;
    float range;
    [SerializeField]
    GameObject attack;
    [SerializeField]
    GameObject projectileSpawn;
    bool fired;
    public int turretCost = 200;
    public int turretDamage = 1;

    SphereCollider objCollider;
    List<GameObject> targetsInRange;

    // Start is called before the first frame update
    void Start()
    {
        objCollider = gameObject.GetComponent<SphereCollider>();
        range = objCollider.radius;
        targetsInRange = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetsInRange.Count > 0) 
        {
            foreach (GameObject target in targetsInRange)
            {
                if (target == null)
                {
                    targetsInRange.Remove(target);
                    break;
                }
                if (!fired)
                {
                    if (targetsInRange.Count > 0)
                    {
                        StartCoroutine(InstantAttack());
                    }
                    else
                    {
                        StopCoroutine(InstantAttack());
                    }
                    break;
                }
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Unit"))
        {
            targetsInRange.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        targetsInRange.Remove(targetsInRange[0]);
    }

    IEnumerator InstantAttack()
    {
        fired = true;
        yield return new WaitForSeconds(fireRate);

        if (targetsInRange.Count > 0)
        {
            foreach(GameObject target in targetsInRange)
            {
                if (target == null)
                {
                    targetsInRange.Remove(target);
                }
            }
            if(targetsInRange.Count > 0)                                                                                        // If after removing null targets, there are still targets in attack range
            {
                Instantiate(attack, projectileSpawn.transform.position, Quaternion.identity);

                foreach (GameObject target in targetsInRange)
                {
                    UnitScript unitScript = target.GetComponent<UnitScript>();
                    if (unitScript is IDamageable)
                    {
                        unitScript.TakeDamage(turretDamage);
                    }
                }
            }
        }
        fired = false;
    }
}
