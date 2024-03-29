using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using System;
using System.IO;
//using UnityEditor.Experimental.GraphView;

public class Pathfinding : MonoBehaviour
{
    public NodeGrid nodeGrid;
    PathRequestManager pRequestManager;

    private void Awake()
    {
        pRequestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {                                                                                                                      // A* Pathfinding (Individual)
        StartCoroutine(FindDirectPath(startPos, targetPos));
    }

    IEnumerator FindDirectPath(Vector3 startPos, Vector3 endPos)
    {
        Vector3[] waypoints = new Vector3[0];                                                                               // Array to store path waypoint positions
        bool pathSuccess = false;

        Node startNode = nodeGrid.NodeFromWorldPoint(startPos);
        startNode.gCost = 0;

        Node endNode = nodeGrid.NodeFromWorldPoint(endPos);

        if (endNode.walkable && !endNode.aboveOccupied)
        {
            Heap<Node> openSet = new Heap<Node>(nodeGrid.MaxSize);                                                                              // Creating list to store open nodes
            List<Node> closedSet = new List<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();


                closedSet.Add(currentNode);

                if (currentNode.id == endNode.id)                                                                                     // Have found the target node
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbourNode in nodeGrid.GetNeighbours(currentNode))
                {
                    neighbourNode.gCost = 0;
                    neighbourNode.hCost = 0;
                    if (!neighbourNode.walkable || neighbourNode.aboveOccupied ||  closedSet.Exists(x => x.id == neighbourNode.id))                             // Checking if neighbour node is not walkable or is in the closed set
                    {                                                                                                           // so we can just skip to the next neighbor in list
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + nodeGrid.GetDistance(currentNode, neighbourNode);               // Distance/cost from current node to neighbour node to
                                                                                                                                // calculate costs for new neighbour node
                    if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))     // Checking if new neighbour G cost is less than its current cost to see if it needs
                    {                                                                                                           // to be reduced or is not in the open list (not already going to be checked)
                        neighbourNode.gCost = newMovementCostToNeighbour;                                                       // Set current G cost to new G cost (better path from start node found)
                        neighbourNode.hCost = nodeGrid.GetDistance(neighbourNode, endNode);                                              // Set H cost as distance to end node
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

        waypointList.Add(path[0].nodeWPosition);


        for (int i = 1; i < path.Count-1; i++)
        {

            Vector2 newDirection = new Vector2(path[i].gridXPos - path[i].gridXPos, path[i].gridYPos - path[i].gridYPos);
            Vector2 oldDirection = new Vector2(path[i].gridXPos - path[i - 1].gridXPos, path[i].gridYPos - path[i - 1].gridYPos);

            if (newDirection != oldDirection)
            {
                waypointList.Add(path[i-1].nodeWPosition);
            }

            if (i == path.Count - 1 && oldDirection != new Vector2(path[i].gridXPos, path[i].gridYPos) - new Vector2(startNode.gridXPos, startNode.gridYPos))
            {
                waypointList.Add(path[path.Count-1].nodeWPosition);
            }
        }
        //waypointList.Add(path[path.Count - 1].nodeWPosition);
        return waypointList.ToArray();
    }
}
