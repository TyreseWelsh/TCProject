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
    public GameObject nodeGrid;
    NodeGrid nodeGridScript;
    public GameObject unit;
    UnitMovement unitMovementScript;
    public UIManager uiManager;


    private void Awake()
    {
        nodeGridScript = nodeGrid.GetComponent<NodeGrid>();
        unitMovementScript = unit.GetComponent<UnitMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        placementZoneObj = Instantiate(placementZone, new Vector3(100, 100, 100), Quaternion.identity);                    // Create object at ray hit x/z position with y=1 to be above ground
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = c.ScreenPointToRay(Input.mousePosition);                                                                 // Ray original position/direction = from camera to mouse on screen position
        RaycastHit hit = new RaycastHit();                                                                                 // New variable to store information on raycast hit (position, object etc.)

        LayerMask g0LayerMask = LayerMask.GetMask("Floor");
        LayerMask g1LayerMask = LayerMask.GetMask("Environment");
        LayerMask placedLayerMask = LayerMask.GetMask("PlacedObject");

        DisplayPlacementZone(ray, hit, g0LayerMask, g1LayerMask, placedLayerMask);

        if (Input.GetMouseButtonDown(0))                                                                                    // If left mouse button is clicked
        {
            SpawnObject(objectToPlace, ray, hit, g0LayerMask, g1LayerMask, placedLayerMask);                                // Spawn Object to block path
        }
        else if(Input.GetMouseButtonDown(1))
        {
            DeleteObject(ray, hit, placedLayerMask);
        }
    }

    // Function to display location where player will place object
    void DisplayPlacementZone(Ray ray, RaycastHit hit, LayerMask g0LayerMask, LayerMask g1LayerMask, LayerMask placedLayerMask)
    {
        if (Physics.Raycast(ray, out hit, rayDistance, g0LayerMask))
        {
            if (Physics.CheckSphere(CalculatePosition(hit), 0.1f, placedLayerMask)                           // Returns true if another collision box overlaps with
                || Physics.CheckSphere(CalculatePosition(hit), 0.1f, g1LayerMask))                           // checking sphere on PlacedObject or ground1 layer
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
    void SpawnObject(GameObject objectToSpawn, Ray ray, RaycastHit hit, LayerMask g0LayerMask, LayerMask g1LayerMask, LayerMask placedLayerMask)
    {
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
                nodeGridScript.CreateGrid();
                unitMovementScript.GetPath();
                uiManager.SetScore(-10);
            }
        }
    }

    // Function to get position to place new object
    Vector3 CalculatePosition(RaycastHit hit)
    {
        float newXPos = (float)Math.Round(hit.point.x, MidpointRounding.AwayFromZero);                      // Rounding hit positions to nearest even whole number (using AwayFromZero)
        float newZPos = (float)Math.Round(hit.point.z, MidpointRounding.AwayFromZero);

        return new Vector3(newXPos, 1.0f, newZPos);                                                         // Returning new vector3 containing position to place new object
    }

    // Function to delete object placed by player
    void DeleteObject(Ray ray, RaycastHit hit, LayerMask placedLayerMask)
    {
        if(Physics.Raycast(ray, out hit, rayDistance, placedLayerMask))
        {
            Destroy(hit.collider.gameObject);
        }
    }
}