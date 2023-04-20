using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour
{
    PlayerManager playerManagerScript;


    Transform targetTransform;

    public float fireRate = 1;
    float range;

    [SerializeField]
    GameObject projectileSpawn;
    [SerializeField]
    GameObject attack;
    [SerializeField]
    float projectileSpeed;
    bool fired;
    public int turretCost = 100;
    public int turretDamage = 0;

    SphereCollider objCollider;
    List<GameObject> targetsInRange;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();
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
                        StartCoroutine(BasicAttack());
                    }
                    else
                    {
                        StopCoroutine(BasicAttack());
                    }
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
        if(targetsInRange.Count > 0) 
        {
            targetsInRange.Remove(targetsInRange[0]);
        }
    }

    IEnumerator BasicAttack()
    {
        fired = true;
        yield return new WaitForSeconds(fireRate);

        targetTransform = null;
        for(int i = 0; i < targetsInRange.Count; i++)
        {
            if (targetsInRange[i] != null)
            {
                targetTransform = targetsInRange[i].transform;
                break;
            }
        }
        if(targetTransform == null)
        {
            fired = false;
            yield break;
        }

        if (targetsInRange.Count > 0)
        {
            GameObject newProjectile = Instantiate(attack, projectileSpawn.transform.position, Quaternion.identity);
            BasicProjectile projectileScript = newProjectile.GetComponent<BasicProjectile>();
            projectileScript.Init(projectileSpeed, projectileSpawn.transform.position, targetTransform.position, turretDamage);
        }

        fired = false;
    }
}
