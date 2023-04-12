using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;

public class NodeGrid : MonoBehaviour
{
    public bool drawAllGizmos = true;
    LayerMask environmentMask;
    LayerMask placedObjMask;
    public Vector2 gridWSize;                                                                                                       // Grid size in world
    public Node [,] grid;
    public float nodeSize;
    public Vector2Int gridLSize = new Vector2Int(0, 0);                                                                                    // Grid local size
    public Transform AIChar;

    public List<Node> path;

    public Transform flowTargetTransform;
    bool flowFieldCreated;

    [SerializeField]
    [Tooltip("0 for A* , 1 for Flow Field")]
    public enum PathfindingType
    {
        AStar,
        FlowField
    }
    public PathfindingType currentPathFindingType = PathfindingType.FlowField;

    // Start is called before the first frame update
    void Awake()
    {
        environmentMask = LayerMask.GetMask("Environment");
        placedObjMask = LayerMask.GetMask("PlacedObject");

        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int MaxSize
    {
        get
        {
            return gridLSize.x * gridLSize.y;
        }
    }

    // Function to create/update grid for pathfinding
    public void CreateGrid()
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

        if(currentPathFindingType == PathfindingType.FlowField)
        {
            UnityEngine.Debug.Log("Flow Field Pathfinding");
            CreateFlowField(flowTargetTransform.position);
        }
    }


    public void CreateFlowField(Vector3 endPos)
    {
        flowFieldCreated = false;
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node targetNode = NodeFromWorldPoint(endPos);
        //print("start: " + startNode.nodeWPosition + " end: " + endNode.nodeWPosition);
        if (targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(MaxSize);                                                                              // Creating list to store open nodes
            List<Node> closedSet = new List<Node>();

            openSet.Add(targetNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                //if (currentNode.id == endNode.id)                                                                                     // Have found the target node
                //{
                //    //print("Path found in: " + sw.ElapsedMilliseconds + " ms");
                //    break;
                //}
                foreach (Node neighbourNode in GetNeighbours(currentNode))
                {
                    if (!neighbourNode.walkable || closedSet.Exists(x => x.id == neighbourNode.id))                             // Checking if neighbour node is not walkable or is in the closed set
                    {                                                                                                           // so we can just skip to the next neighbor in list
                        continue;
                    }

                    neighbourNode.gCost = 0;
                    neighbourNode.hCost = 0;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode);               // Distance/cost from current node to neighbour node to
                                                                                                                                // calculate costs for new neighbour node

                    float percentX = ((neighbourNode.nodeWPosition.x - transform.position.x) + gridWSize.x / 2) / gridWSize.x;                                          // Position of player in grid by getting % on x and y
                    float percentY = ((neighbourNode.nodeWPosition.z - transform.position.z) + gridWSize.y / 2) / gridWSize.y;                                          //

                    percentX = Mathf.Clamp01(percentX);
                    percentY = Mathf.Clamp01(percentY);

                    int x = Mathf.FloorToInt(Mathf.Min(gridLSize.x * percentX, gridLSize.x - 1));
                    int y = Mathf.FloorToInt(Mathf.Min(gridLSize.y * percentY, gridLSize.y - 1));

                    grid[x, y].gCost = newMovementCostToNeighbour;
                    grid[x, y].parentNode = currentNode;

                    neighbourNode.gCost = newMovementCostToNeighbour;                                                           // Setting G cost of node
                    neighbourNode.parentNode = currentNode;
                    
                    if (!openSet.Contains(neighbourNode))
                    {
                        openSet.Add(neighbourNode);                                                                         // Adding chosen valid neighbours node to open list
                    }

                    //if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))     // Checking if new neighbour G cost is less than its current cost to see if it needs
                    //{                                                                                                           // to be reduced or is not in the open list (not already going to be checked)
                    //    UnityEngine.Debug.Log(newMovementCostToNeighbour);
                    //    neighbourNode.gCost = newMovementCostToNeighbour;                                                       // Set current G cost to new G cost (better path from start node found)
                    //    //neighbourNode.hCost = GetDistance(neighbourNode, endNode);                                              // Set H cost as distance to end node
                    //    neighbourNode.parentNode = currentNode;                                                                 // Setting parent node to look back on

                    //    if (!openSet.Contains(neighbourNode))
                    //    {
                    //        openSet.Add(neighbourNode);                                                                         // Adding chosen valid neighbours node to open list
                    //    }
                    //    else
                    //    {
                    //        openSet.UpdateItem(neighbourNode);
                    //    }
                    //}
                }
            }

            //foreach (Node node in grid)
            //{
            //    UnityEngine.Debug.Log("id: " + node.id + " - cost: " + node.gCost);
            //}

            flowFieldCreated = true;
            sw.Stop();
            print("Flow field created in: " + sw.ElapsedMilliseconds + " ms");
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

    public void SetFlowTarget(Transform newTargetTransform)
    {
        flowTargetTransform = newTargetTransform;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();                                                                                       // Creating new list of possible neighbours of current node

        for (float x = -1; x <= 1; x++)                                                                                                 // Looping through nodes 1 away in each direction of current node     
        {                                                                                                                               // using nodeSize to get the distance between the center of each
            for (float y = -1; y <= 1; y++)                                                                                             //
            {                                                                                                                           //
                if((x == 0 && y == 0) || (x == 1 && y == 1) || (x == -1 && y == -1) || (x == -1 && y == 1) || (x == 1 && y == -1))      // If x,y represents center node                                                                                              // If x and y = 0 then its checking center (current) node
                {
                    continue;
                }
                float checkXPos = node.gridXPos + x;                                                                                   // Checking/getting position of surrounding nodes
                float checkYPos = node.gridYPos + y;                                                                                   //

                if(checkXPos >=0 && checkXPos < gridLSize.x && checkYPos >= 0 && checkYPos < gridLSize.y)                               // Checking if node is actually inside of grid
                {
                    if(grid[Mathf.FloorToInt(Mathf.Min(checkXPos)), Mathf.FloorToInt(Mathf.Min(checkYPos))].walkable)                  // Checking if node is valid for pathfinding
                    {
                        neighbours.Add(grid[Mathf.FloorToInt(Mathf.Min(checkXPos)), Mathf.FloorToInt(Mathf.Min(checkYPos))]);                                      // Add valid node to neighbours list
                    }
                }
            }
        }

        return neighbours;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridXPos - nodeB.gridXPos);
        int dstY = Mathf.Abs(nodeA.gridYPos - nodeB.gridYPos);

        // Diagonal distance to get on same horizontal/vertical line as end node always = lower distance
        // so then rest of the distance == highest distance - lowest distance
        // (Multiplying by 14 for diagonal and 10 for non diagonal (1.4*10 , 1.0*10)
        if (dstX > dstY)                                                                                                 // If Y distance to end node < X distance,
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    // To render the tiles
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWSize.x, 1, gridWSize.y));                                              // Creating the basic grid shape

        //if (drawAllGizmos == false)                                                                                                      // Only draw path gizmos if drawAllGizmos = false
        //{
        //    if (path != null)
        //    {
        //        foreach (Node node in path)
        //        {
        //            Gizmos.color = Color.magenta;
        //            Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));                                               // Drawing the cubes with small gap inbetween

        //        }
        //    }
        //}
        if (drawAllGizmos == true)                                                                                                           // Draw all gizmos if drawAllGizmos is true
        {
            if (grid != null)                                                                                                                // Checking if grid is valid
            {
                //Node AINode = NodeFromWorldPoint(AIChar.position);

                foreach (Node node in grid)
                {
                    Gizmos.color = (node.walkable) ? Color.green : Color.red;                                                               // Setting colour of cubes to show if walkable or not

                    //if (path != null)
                    //{
                    //    if (path.Exists(x => x.id == node.id))
                    //    {
                    //        Gizmos.color = Color.magenta;
                    //    }
                    //}

                    //if (AINode.nodeWPosition == node.nodeWPosition)
                    //{
                    //    Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));
                    //    Gizmos.color = Color.cyan;
                    //}
                    Gizmos.DrawWireCube(node.nodeWPosition, Vector3.one * (nodeSize - 0.1f));                                               // Drawing the cubes with small gap inbetween

                    if (currentPathFindingType == PathfindingType.FlowField && node.walkable && flowFieldCreated)
                    {
                        //Gizmos.color = Color.yellow;
                        Handles.Label(new Vector3(node.nodeWPosition.x, 1, node.nodeWPosition.z), node.gCost.ToString());
                        //Gizmos.DrawLine(new Vector3(node.parentNode.nodeWPosition.x, 1, node.parentNode.nodeWPosition.z), new Vector3(node.nodeWPosition.x, 1, node.nodeWPosition.z));
                    }
                }
            }
        }
    }
}
