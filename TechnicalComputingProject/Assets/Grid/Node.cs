using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool walkable;
    public Vector3 nodeWPosition;
 
    public Node(bool newWalkable, Vector3 newNodeWPosition)
    {
        walkable = newWalkable;
        nodeWPosition = newNodeWPosition;
    }
}
