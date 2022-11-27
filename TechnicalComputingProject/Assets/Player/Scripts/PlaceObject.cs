using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceObject : MonoBehaviour
{
    [SerializeField]
    private GameObject placementZone;
    [SerializeField] 
    private GameObject objectToPlace;
    [SerializeField]
    private Camera c;
    int rayDistance = 300;

    private GameObject placementZoneObj;

    // Start is called before the first frame update
    void Start()
    {
        placementZoneObj = Instantiate(placementZone, new Vector3(100, 100, 100), Quaternion.identity);                    // Create object at ray hit x/z position with y=1 to be above ground
    }

    // Update is called once per frame
    void Update()
    {
        DisplayPlacementZone();

        if (Input.GetMouseButtonDown(0))                                                                     // If left mouse button is clicked
        {
            SpawnObject(objectToPlace);                                                                      // Spawn Object to block path
        }
    }

    // Function to display location where player will place object
    void DisplayPlacementZone()
    {
        Ray ray = c.ScreenPointToRay(Input.mousePosition);                                                             // Ray original position/direction = from camera to mouse on screen position
        RaycastHit hit = new RaycastHit();                                                                             // New variable to store information on raycast hit (position, object etc.)

        LayerMask g0LayerMask = LayerMask.GetMask("Ground0");
        LayerMask g1LayerMask = LayerMask.GetMask("Ground1");
        LayerMask placedLayerMask = LayerMask.GetMask("PlacedObject");

        if (Physics.Raycast(ray, out hit, rayDistance, g0LayerMask))
        {
            if (Physics.CheckSphere(CalculatePosition(hit), 0.1f, placedLayerMask)                           // Returns true if another collision box overlaps with
                || Physics.CheckSphere(CalculatePosition(hit), 0.1f, g1LayerMask))                          // checking sphere on PlacedObject or ground1 layer
            {
                placementZoneObj.SetActive(false);
            }
            else
            {
                placementZoneObj.SetActive(true);
                placementZoneObj.transform.position = CalculatePosition(hit);

            }
        }
        else
        {
            placementZoneObj.SetActive(false);
        }
    }

    // Function to get point in world space to place object using mouse position
    void SpawnObject(GameObject objectToSpawn)
    {
        Ray ray = c.ScreenPointToRay(Input.mousePosition);                                                             // Ray original position/direction = from camera to mouse on screen position
        RaycastHit hit = new RaycastHit();                                                                             // New variable to store information on raycast hit (position, object etc.)

        LayerMask g0LayerMask = LayerMask.GetMask("Ground0");
        LayerMask g1LayerMask = LayerMask.GetMask("Ground1");
        LayerMask placedLayerMask = LayerMask.GetMask("PlacedObject");

        if (Physics.Raycast(ray, out hit, rayDistance, g0LayerMask))
        {
            if(Physics.CheckSphere(CalculatePosition(hit), 0.1f, placedLayerMask)                           // Returns true if another collision box overlaps with
                || Physics.CheckSphere(CalculatePosition(hit), 0.1f, g1LayerMask))                          // checking sphere on PlacedObject or ground1 layer
            {
                Debug.Log("Object Detected");
            }
            else 
            {
                Instantiate(objectToSpawn, CalculatePosition(hit), Quaternion.identity);                    // Create object at ray hit x/z position with y=1 to be above ground
            }
        }
    }

    // Function to get position to place new object
    Vector3 CalculatePosition(RaycastHit hit)
    {
        float newXPos = (float)Math.Round(hit.point.x, MidpointRounding.AwayFromZero);                      // Rounding hit positions to nearest even whole number (using AwayFromZero)
        float newZPos = (float)Math.Round(hit.point.z, MidpointRounding.AwayFromZero);

        return new Vector3(newXPos, 1.0f, newZPos);                                             // Returning new vector3 containing position to place new object
    }
}