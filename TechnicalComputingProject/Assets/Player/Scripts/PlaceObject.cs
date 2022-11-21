using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] 
    private GameObject objectToPlace;
    [SerializeField]
    private Camera c;
    int rayDistance = 300;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))                                                                     // If left mouse button is clicked
        {
            SpawnObject();
        }
    }


    // Function to get point in world space to place object using mouse position
    void SpawnObject()
    {
        Ray ray = c.ScreenPointToRay(Input.mousePosition);                                                  // Ray original position/direction = from camera to mouse on screen position
        RaycastHit hit = new RaycastHit();                                                                  // New variable to store information on raycast hit (position, object etc.)
        LayerMask g0LayerMask = LayerMask.GetMask("Ground0");
        LayerMask placedLayerMask = LayerMask.GetMask("PlacedObject");
        if (Physics.Raycast(ray, out hit, rayDistance, g0LayerMask))
        {
            if(Physics.CheckSphere(CalculatePosition(hit), 0.1f, placedLayerMask))                          // Returns true if another collision box overlaps with
            {                                                                                               // checking sphere, ignoring PlacedObject layer
                Debug.Log("Object Detected");
            }
            else 
            {
                Instantiate(objectToPlace, CalculatePosition(hit), Quaternion.identity);        // Create object at ray hit x/z position with y=1 to be above ground
            }
        }
    }

    // Function to get position to place new object
    Vector3 CalculatePosition(RaycastHit hit)
    {
        float newXPos = Mathf.Round(hit.point.x);                                               // Rounding hit positions to nearest whole number
        float newZPos = Mathf.Round(hit.point.z);

        return new Vector3(newXPos, 1.0f, newZPos);                                             // Returning new vector3 containing position to place new object
    }
}
