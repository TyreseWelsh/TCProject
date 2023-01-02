using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public bool drawAllGizmos = true;
    LayerMask environmentMask;
    LayerMask placedObjMask;
    public Vector2 gridWSize;                                                                                                       // Grid size in world
    Node [,] grid;
    public float nodeSize;
    public Vector2Int gridLSize = new Vector2Int(0, 0);                                                                                    // Grid local size
    public Transform AIChar;

    public List<Node> path;


    // Start is called before the first frame update
    void Start()
    {
        environmentMask = LayerMask.GetMask("Environment");
        placedObjMask = LayerMask.GetMask("PlacedObject");
    }

    // Update is called once per frame
    void Update()
    {
            CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridLSize.x * gridLSize.y;
        }
    }

    // Function to create/update grid for pathfinding
    void CreateGrid()
    {
        int nodeID = 0;
        gridLSize.x = (int)Math.Round(gridWSize.x / nodeSize);
        gridLSize.y = (int)Math.Round(gridWSize.y / nodeSize);

        grid = new Node[gridLSize.x, gridLSize.y];
        Vector3 wBottomLeft = transform.position - Vector3.right * gridWSize.x / 2 - Vector3.forward * gridWSize.y / 2;           // Bottom left position of world
        Vector3 worldPoint;

        for (int x = 0; x < gridLSize.x; x++)                                                                                           // Looping through each row (x)
        {
            for(int y = 0; y < gridLSize.y; y++)                                                                                        // Looping through each column (y)
            {
                worldPoint = wBottomLeft + Vector3.right * (x * nodeSize + (nodeSize / 2))                                            // World position of node in grid
                    + Vector3.forward * (y * nodeSize + (nodeSize / 2));

                bool walkable = !(Physics.CheckSphere(worldPoint, (nodeSize / 2 - nodeSize / 3), environmentMask | placedObjMask));          // Setting if the current node is walkable or not using collisions      
                grid[x, y] = new Node(nodeID, walkable, worldPoint, x, y);                                                                            // Create a new node at this index with the right variables
                nodeID++;
            }
        }
    }

    // Get current node AI is on using AI's world position
    public Node NodeFromWorldPoint(Vector3 wPosition)
    {
        float percentX = ((wPosition.x - transform.position.x) + gridWSize.x / 2) / gridWSize.x;                                          // Position of player in grid by getting % on x and y
        float percentY = ((wPosition.z - transform.position.z) + gridWSize.y / 2) / gridWSize.y;                                          //

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt(Mathf.Min(gridLSize.x * percentX, gridLSize.x - 1));
        int y = Mathf.FloorToInt(Mathf.Min(gridLSize.y * percentY, gridLSize.y - 1));

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();                                                                                       // Creating new list of possible neighbours of current node

        for (float x = -1; x <= 1; x++)                                                                                                 // Looping through nodes 1 away in each direction of current node     
        {                                                                                                                               // using nodeSize to get the distance between the center of each
            for (float y = -1; y <= 1; y++)                                                                                             //
            {                                                                                                                           //
                if(x == 0 && y == 0)                                                                                                    // If x and y = 0 then its checking center (current) node
                {
                    continue;
                }
                float checkXPos = node.gridXPos + x;                                                                                   // Checking/getting position of surrounding nodes
                float checkYPos = node.gridYPos + y;                                                                                   //
                //Debug.Log("x= " + checkXPos + "y= " + checkYPos);
                if(checkXPos >=0 && checkXPos < gridLSize.x && checkYPos >= 0 && checkYPos < gridLSize.y)                          // Checking if node is actually inside of grid
                {
                    neighbours.Add(grid[Mathf.FloorToInt(Mathf.Min(checkXPos)), Mathf.FloorToInt(Mathf.Min(checkYPos))]);                                      // Add valid node to neighbours list
                }
            }
        }

        return neighbours;
    }

    // To render the tiles
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWSize.x, 1, gridWSize.y));                                              // Creating the basic grid shape

        if(drawAllGizmos == false)                                                                                                      // Only draw path gizmos if drawAllGizmos = false
        {
            if (path != null)
            {
                foreach (Node node in path)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));                                               // Drawing the cubes with small gap inbetween

                }
            }
        }
        else                                                                                                                                // Draw all gizmos if drawAllGizmos is true
        {
            if (grid != null)                                                                                                                // Checking if grid is valid
            {
                Node AINode = NodeFromWorldPoint(AIChar.position);

                foreach (Node node in grid)
                {
                    Gizmos.color = (node.walkable) ? Color.green : Color.red;                                                               // Setting colour of cubes to show if walkable or not

                    if (path != null)
                    {
                        if (path.Exists(x => x.id == node.id))
                        {
                            Gizmos.color = Color.magenta;
                        }
                    }

                    if (AINode.nodeWPosition == node.nodeWPosition)
                    {
                        Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));
                        Gizmos.color = Color.cyan;
                    }
                    Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));                                               // Drawing the cubes with small gap inbetween

                }
            }
        }
    }
}
