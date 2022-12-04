using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    LayerMask environmentMask;
    LayerMask placedObjMask;
    public Vector2 gridWSize;
    Node [,] grid;
    public float nodeSize;
    Vector2Int gridLSize = new Vector2Int(0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        environmentMask = LayerMask.GetMask("Environment");
        placedObjMask = LayerMask.GetMask("PlaceObject");
    }

    void CreateGrid()
    {
        gridLSize.x = (int)Math.Round(gridWSize.x / nodeSize);
        gridLSize.y = (int)Math.Round(gridWSize.y / nodeSize);

        grid = new Node[gridLSize.x, gridLSize.y];
        Vector3 wBottomLeft = transform.position - Vector3.right * gridWSize.x / 2 - Vector3.forward * gridWSize.y / 2;           // Bottom left position of world
        Vector3 worldPoint;

        for (int i = 0; i < gridLSize.x; i++)                                                                                           // Looping through each row (x)
        {
            for(int j = 0; j < gridLSize.y; j++)                                                                                        // Looping through each column (y)
            {
                worldPoint = wBottomLeft + Vector3.right * (i * nodeSize + (nodeSize / 2))                                            // World position of node in grid
                    + Vector3.forward * (j * nodeSize + (nodeSize / 2));

                bool walkable = !( (Physics.CheckSphere(worldPoint, (nodeSize / 2 - nodeSize / 3), environmentMask))                    // Setting if the current node is walkable or not using collisions
                    || (Physics.CheckSphere(worldPoint, (nodeSize / 2 - nodeSize / 3), placedObjMask)) );
                grid[i, j] = new Node(walkable, worldPoint);                                                                            // Create a new node at this index with the right variables
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))                                                                                                 // Update Grid when Key is pressed
        {
            CreateGrid();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWSize.x, 1, gridWSize.y));                                              // Creating the basic grid shape

        if (grid != null)                                                                                                                // Checking if grid is valid
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.green : Color.red;                                                               // Setting colour of cubes to show if walkable or not
                Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));                                               // Drawing the cubes with small gap inbetween
            }
        }
    }
}
