using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using System;
using System.IO;

public class Pathfinding : MonoBehaviour
{
    public NodeGrid nodeGrid;
    PathRequestManager pRequestManager;

    private void Awake()
    {
        pRequestManager = GetComponent<PathRequestManager>();
    }

    //public Transform AISeekerTf;
    //public Transform targetTf;

    // Update is called once per frame
    //void Update()
    //{
        //if (Input.GetKeyDown(KeyCode.N))                                                                                                 // Update Grid when Key is pressed
        //{
        //    if (nodeGrid.gridLSize.x > 0 && nodeGrid.gridLSize.y > 0)
        //    {
        //        FindPath(AISeekerTf.position, targetTf.position);
        //    }
        //}
    //}

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];                                                                               // Array to store path waypoint positions
        bool pathSuccess = false;

        Node startNode = nodeGrid.NodeFromWorldPoint(startPos);
        startNode.gCost = 0;

        Node endNode = nodeGrid.NodeFromWorldPoint(endPos);
        //print("start: " + startNode.nodeWPosition + " end: " + endNode.nodeWPosition);
        if (endNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(nodeGrid.MaxSize);                                                                              // Creating list to store open nodes
            List<Node> closedSet = new List<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                //for (int i = 1; i < openSet.Count; i++)                                                                          // Looping through open set and checking f costs to decide which node to
                //{
                //    if (openSet[i].fCost < currentNode.fCost ||                                                                 // (Will be node with lowest F cost in open list)
                //        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))                          // If F cost is the same, compare h cost and pick
                //    {                                                                                                           // one closest (lowest) to end node
                //        currentNode = openSet[i];
                //    }
                //}
                //
                //openSet.Remove(currentNode);

                closedSet.Add(currentNode);

                if (currentNode.id == endNode.id)                                                                                     // Have found the target node
                {
                    sw.Stop();
                    //print("Path found in: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbourNode in nodeGrid.GetNeighbours(currentNode))
                {
                    neighbourNode.gCost = 0;
                    neighbourNode.hCost = 0;
                    if (!neighbourNode.walkable || closedSet.Exists(x => x.id == neighbourNode.id))                             // Checking if neighbour node is not walkable or is in the closed set
                    {                                                                                                           // so we can just skip to the next neighbor in list
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode);               // Distance/cost from current node to neighbour node to
                                                                                                                                // calculate costs for new neighbour node
                    if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))     // Checking if new neighbour G cost is less than its current cost to see if it needs
                    {                                                                                                           // to be reduced or is not in the open list (not already going to be checked)
                        neighbourNode.gCost = newMovementCostToNeighbour;                                                       // Set current G cost to new G cost (better path from start node found)
                        neighbourNode.hCost = GetDistance(neighbourNode, endNode);                                              // Set H cost as distance to end node
                        neighbourNode.parentNode = currentNode;                                                                 // Setting parent node to look back on

                        if (!openSet.Contains(neighbourNode))
                        {
                            openSet.Add(neighbourNode);                                                                         // Adding chosen valid neighbours node to open list
                        }
                        else
                        {
                            openSet.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
        }
        
        yield return null;

        if(pathSuccess)
        {
            waypoints = RetracePath(startNode, endNode);
        }

        pRequestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();                                                                                 // Creating new list for the path backwards from the end node

        Node currentNode = endNode;                                                                                         // Starting node for path will be the end node

        while (currentNode.id != startNode.id)
        {
            path.Add(currentNode);                                                                                          // Add the current node to the path
            currentNode = currentNode.parentNode;                                                                           // Make the new current node, the old current nodes parent going back
        }                                                                                                                   // until the current node is == the start node
        path.Add(currentNode);

        Vector3[] waypoints = SimplifyPath(path, startNode);

        Array.Reverse(waypoints);                                                                                           // Reverse path to get path from start node to end node

        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path, Node startNode)
    {
        if (path.Count < 1)
        {
            return new Vector3[0];                                                                                                  // If path is empty
        }

        List<Vector3> waypointList = new List<Vector3>();                                                                           // New list of waypoints for path
        //Vector2 directionOld = Vector2.zero;

        waypointList.Add(path[0].nodeWPosition);


        for (int i = 1; i < path.Count-1; i++)
        {

            Vector2 newDirection = new Vector2(path[i + 1].gridXPos - path[i].gridXPos, path[i + 1].gridYPos - path[i].gridYPos);
            Vector2 oldDirection = new Vector2(path[i].gridXPos - path[i - 1].gridXPos, path[i].gridYPos - path[i - 1].gridYPos);

            if (newDirection != oldDirection)
            {
                waypointList.Add(path[i-1].nodeWPosition);
            }


            //Vector2 directionNew = new Vector2(path[i - 1].gridXPos - path[i].gridXPos, path[i - 1].gridYPos - path[i].gridYPos);   // Gets a vector2 representing the current direction the unit is moving
            //if (directionNew != directionOld)                                                                                        // If the current direction is different to old direction
            //{
            //    waypointList.Add(path[i-1].nodeWPosition);                                                                            // Add a new waypoint to list
            //}
            //directionOld = directionNew;                                                                                            // Setting old direction to current direction to compare with future directions

            if (i == path.Count - 1 && oldDirection != new Vector2(path[i].gridXPos, path[i].gridYPos) - new Vector2(startNode.gridXPos, startNode.gridYPos))
            {
                waypointList.Add(path[path.Count-1].nodeWPosition);
            }
        }
        //waypointList.Add(path[path.Count - 1].nodeWPosition);
        return waypointList.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridXPos - nodeB.gridXPos);
        int dstY = Mathf.Abs(nodeA.gridYPos - nodeB.gridYPos);

        // Diagonal distance to get on same horizontal/vertical line as end node always = lower distance
        // so then rest of the distance == highest distance - lowest distance
        // (Multiplying by 14 for diagonal and 10 for non diagonal (1.4*10 , 1.0*10)
        if(dstX > dstY)                                                                                                 // If Y distance to end node < X distance,
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
