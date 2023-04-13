using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using System.Linq;
using System;

public class UnitMovement : MonoBehaviour
{
    Transform target;
    [SerializeField] float speed;
    Vector3[] path;
    int targetIndex;

    GameObject nodeGridObj;
    NodeGrid nodeGridScript;


    private void Awake()
    {
        nodeGridObj = GameObject.Find("PathfindingGrid");
        nodeGridScript = nodeGridObj.GetComponent<NodeGrid>();
    }

    private void Start()
    {
        GetPath();
    }
    private void Update()
    {

    }

    public void GetPath()
    {
        switch (nodeGridScript.currentPathFindingType)
        {
            case (NodeGrid.PathfindingType.AStar):
                UnityEngine.Debug.Log("Following A*");
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                break;

            case (NodeGrid.PathfindingType.FlowField):
                UnityEngine.Debug.Log("Following flow field");
                StopCoroutine("FollowFlowField");
                StartCoroutine("FollowFlowField");
                break;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowDirectPath");
            StartCoroutine("FollowDirectPath");
        }
    }

    IEnumerator FollowFlowField()
    {
        Node currentNode = nodeGridScript.NodeFromWorldPoint(transform.position);
        //Debug.Log(currentNode.nodeWPosition);
        Node endNode = nodeGridScript.NodeFromWorldPoint(nodeGridScript.flowTargetTransform.position);

        while (currentNode.id != endNode.id)
        {
            Node nextNode = nodeGridScript.GetNeighbours(currentNode)[0];
            foreach(Node neighbourNode in nodeGridScript.GetNeighbours(currentNode))
            {
                    if (neighbourNode.gCost < nextNode.gCost)
                    {
                        nextNode = neighbourNode;
                    }
            }

            if (transform.position == nextNode.nodeWPosition)
            {
                currentNode = nextNode;
            }
            //Debug.Log("Next node pos: " + nextNode.nodeWPosition);
            //Debug.Log("Current node pos: " + currentNode.nodeWPosition);

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextNode.nodeWPosition.x, 1, nextNode.nodeWPosition.z), speed * Time.deltaTime);

            yield return null;
        }


    }

    IEnumerator FollowDirectPath()
    {
        Vector3 currentWaypoint = path[0];
        while(true)
        {
            //print("unit: " + transform.position + " w: " + currentWaypoint);
            
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    path = new Vector3[0];
                    yield break;
                }

                currentWaypoint = path[targetIndex];
                //print(currentWaypoint.x + " " + currentWaypoint.y + " " + currentWaypoint.z);
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, 1, currentWaypoint.z), speed * Time.deltaTime);

            yield return null;
        }
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void SetMoveSpeed(float _speed)
    {
        speed = _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Turret"))
        {
            TurretController turretController = other.GetComponent<TurretController>();
            if(turretController.GetTurretType() == TurretController.TurretType.Slow)
            {
                speed /= 2;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            TurretController turretController = other.GetComponent<TurretController>();
            if (turretController.GetTurretType() == TurretController.TurretType.Slow)
            {
                speed *= 2;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 0.25f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
