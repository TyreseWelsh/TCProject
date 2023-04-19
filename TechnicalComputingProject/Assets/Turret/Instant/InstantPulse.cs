using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantPulse : MonoBehaviour
{
    private Transform pulseTransform;
    private float pulseRange = 0f;
    private float pulseRangeMax;
    private float pulseSpeed = 2.0f;

    private void Awake()
    {
        pulseTransform = GetComponent<Transform>();
        pulseTransform.localScale = new Vector3(0, 1, 0);
    }

    private void Start()
    {
        GameObject turret = GameObject.Find("InstantTurret(Clone)");
        pulseRangeMax = turret.GetComponent<SphereCollider>().radius / 3;
    }

    // Update is called once per frame
    void Update()
    {
        pulseRange += pulseSpeed * Time.deltaTime;
        if (pulseRange >= pulseRangeMax )
        {
            Destroy(gameObject);
        }

        pulseTransform.localScale = new Vector3(pulseRange, 1, pulseRange);
    }
}
