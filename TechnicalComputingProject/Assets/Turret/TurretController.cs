using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{


    PlayerManager playerManagerScript;


    Transform targetTransform;

    [SerializeField]
    float fireRate = 1;
    float range;

    [SerializeField]
    GameObject projectileSpawn;
    [SerializeField]
    GameObject basicProjectile;
    [SerializeField]
    [Tooltip("0 = None, 1 = Basic")]
    int projectileType;
    [SerializeField]
    float projectileSpeed;
    bool fired;
    int turretCost = 0;

    SphereCollider objCollider;
    List<GameObject> targetsInRange;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();
        objCollider = this.GetComponent<SphereCollider>();
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
                if (targetsInRange.Count > 0)                                                   // Checking if targets in range is still more than 0
                {
                    targetTransform = targetsInRange[0].transform;

                    switch (projectileType)
                    {
                        case (0):
                            if (!fired)                                                                         // No projectile
                            {
                                switch(playerManagerScript.currentTurretType)
                                {
                                    case (PlayerManager.TurretTypes.Instant):
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
                            break;
                        case (1):                                                                               // Basic projectile
                            if (!fired)
                            {
                                if (targetsInRange.Count > 0)
                                {
                                    StartCoroutine(BasicAttack());
                                }
                                else
                                {
                                    StopCoroutine(BasicAttack());
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        targetsInRange.Add(other.gameObject);
        //while(InRange(other.gameObject))
        //{
        //    target = other.gameObject;
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        targetsInRange.Remove(targetsInRange[0]);
    }

    IEnumerator BasicAttack()
    {
        fired = true;
        yield return new WaitForSeconds(fireRate);

        if(targetsInRange.Count > 0)
        {
            GameObject newProjectile = Instantiate(basicProjectile, projectileSpawn.transform.position, Quaternion.identity);
            TurretProjectile projectileScript = newProjectile.GetComponent<TurretProjectile>();
            projectileScript.Init(projectileType, projectileSpeed, projectileSpawn.transform.position, targetTransform.position);
            fired = false;
        }
    }

    IEnumerator InstantAttack()
    {
        fired = true;
        yield return new WaitForSeconds(fireRate);

        if (targetsInRange.Count > 0)
        {
            foreach (GameObject target in targetsInRange)
            {
                UnitScript unitScript = target.GetComponent<UnitScript>();
                if(unitScript is IDamageable)
                {
                    unitScript.TakeDamage(2);
                }
            }
            print("Instant Damage!");
            fired = false;
        }
    }

    bool InRange(GameObject other)
    {
        return Vector3.Distance(transform.position, other.transform.position) <= range;
    }
}
