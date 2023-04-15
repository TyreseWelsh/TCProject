using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 nodeWPosition;
    public int gridXPos;
    public int gridYPos;

    public int id;
    public Node parentNode;

    public int gCost;                                                                               // Node distance from starting node                                                                         
    public int hCost;                                                                               // Node distance from end/target node
    public int fCost                                                                                // Sum of G cost and H cost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public bool aboveOccupied = false;

    public Node(int newId, bool newWalkable, Vector3 newNodeWPosition, int newGridXPos, int newGridYPos)
    {
        walkable = newWalkable;
        nodeWPosition = newNodeWPosition;
        gridXPos = newGridXPos;
        gridYPos = newGridYPos;
        id = newId;
    }

    int heapIndex;
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node compareNode)
    {
        int compare = fCost.CompareTo(compareNode.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(compareNode.hCost);
        }

        return -compare;
    }
}
