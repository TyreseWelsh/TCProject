using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceObject : MonoBehaviour
{
    [SerializeField]
    GameObject placementZone;
    [SerializeField]
    private Camera c;
    int rayDistance = 300;

    private GameObject placementZoneObj;
    NodeGrid nodeGridScript;
    UIManager UIManagerScript;
    PlayerManager playerManagerScript;
    
    public List<GameObject> turretMeshes;

    private void Awake()
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();

        GameObject nodeGrid = GameObject.Find("PathfindingGrid");
        nodeGridScript = nodeGrid.GetComponent<NodeGrid>();

        GameObject gameManager = GameObject.Find("GameManager");
        UIManagerScript = gameManager.GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        placementZoneObj = Instantiate(placementZone, new Vector3(0, 0, 0), Quaternion.identity);                    // Create object at ray hit x/z position with y=1 to be above ground
    }

    // Update is called once per frame
    void Update()
    {
        if(!UIManagerScript.GetPaused())
        {
            Ray ray = c.ScreenPointToRay(Input.mousePosition);                                                                 // Ray original position/direction = from camera to mouse on screen position
            RaycastHit hit = new RaycastHit();                                                                                 // New variable to store information on raycast hit (position, object etc.)

            LayerMask g0LayerMask = LayerMask.GetMask("Floor");
            LayerMask g1LayerMask = LayerMask.GetMask("Environment");
            LayerMask placedLayerMask = LayerMask.GetMask("PlacedObject");

            DisplayPlacementZone(ray, hit, g0LayerMask, g1LayerMask, placedLayerMask);

            if (Input.GetMouseButtonDown(0))                                                                                    // If left mouse button is clicked
            {
                SpawnObject(turretMeshes[(int)playerManagerScript.GetTurretType()], ray, hit, g0LayerMask, g1LayerMask, placedLayerMask);                                // Spawn turret of current type
            }
        }
    }

    // Function to display location where player will place object
    void DisplayPlacementZone(Ray ray, RaycastHit hit, LayerMask _g0LayerMask, LayerMask _g1LayerMask, LayerMask _placedLayerMask)
    {
        if (Physics.Raycast(ray, out hit, rayDistance, _g1LayerMask))
        {
            Node nodeHit = nodeGridScript.NodeFromWorldPoint(hit.point);
            if (nodeHit.aboveOccupied                                                                           // Returns true if another collision box overlaps with
                || Physics.CheckSphere(CalculatePosition(hit), 0.1f, _g0LayerMask))                           // occupied tile or checking sphere on ground1 layer
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
        if (Physics.Raycast(ray, out hit, rayDistance, g1LayerMask))
        {
            //if(playerManagerScript.GetTurretCost() <= playerManagerScript.GetSouls())
            //{
                Node nodeHit = nodeGridScript.NodeFromWorldPoint(hit.point);
                if (nodeHit.aboveOccupied                                                                       // Returns true if another collision box overlaps with
                    || Physics.CheckSphere(CalculatePosition(hit), 0.1f, g0LayerMask))                          // occupied tile or checking sphere on ground1 layer
                {
                    Debug.Log("Hit Detected");
                }
                else
                {
                    switch (playerManagerScript.currentTurretType)
                    {
                        case (PlayerManager.TurretTypes.Basic):
                            if (playerManagerScript.GetSouls() >= 100)
                            {
                                Instantiate(objectToSpawn, CalculatePosition(hit), Quaternion.identity);                    // Create object at ray hit x/z position with y=1 to be above ground
                                playerManagerScript.SetSouls(-100);
                            }
                            break;
                        case (PlayerManager.TurretTypes.Instant):
                            if (playerManagerScript.GetSouls() >= 300)
                            {
                                Instantiate(objectToSpawn, CalculatePosition(hit), Quaternion.identity);
                                playerManagerScript.SetSouls(-300);
                            }
                            break;
                        case (PlayerManager.TurretTypes.Slow):
                            if(playerManagerScript.GetSouls() >= 160)
                            {
                                Instantiate(objectToSpawn, CalculatePosition(hit), Quaternion.identity);
                                playerManagerScript.SetSouls(-160);
                            }
                            break;
                    }
                    nodeHit.aboveOccupied = true;
                }
            //}
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