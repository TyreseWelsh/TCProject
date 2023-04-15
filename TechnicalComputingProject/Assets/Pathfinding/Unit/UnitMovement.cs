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

    [SerializeField]
    UnitManager unitManager;
    PlayerManager playerManagerScript;

    private void Awake()
    {
        nodeGridObj = GameObject.Find("PathfindingGrid");
        nodeGridScript = nodeGridObj.GetComponent<NodeGrid>();

        GameObject playerManager = GameObject.Find("PlayerManager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();
    }

    private void Start()
    {
        GetPath();
    }
    private void Update()
    {
        if (transform.position.x == nodeGridScript.flowTargetTransform.position.x && transform.position.z == nodeGridScript.flowTargetTransform.position.z)
        {
            print("Reached goal");
            playerManagerScript.TakeDamage(2);
            Destroy(gameObject);
        }
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
                if (neighbourNode.gCost < nextNode.gCost)                                                           // Going through all neighbours g costs to determine lowest one to move to
                    {
                        nextNode = neighbourNode;
                    }
            }

            if (transform.position.x == nextNode.nodeWPosition.x && transform.position.z == nextNode.nodeWPosition.z)
            {
                currentNode = nextNode;
            }

            transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, 1, transform.position.z), new Vector3(nextNode.nodeWPosition.x, 1, nextNode.nodeWPosition.z), speed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator FollowDirectPath()
    {
        Vector3 currentWaypoint = path[0];
        while(true)
        {
            //print("unit: " + transform.position + " w: " + currentWaypoint);
            
            if (transform.position.x == currentWaypoint.x && transform.position.z == currentWaypoint.z)
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
            if(playerManagerScript.GetTurretType() == PlayerManager.TurretTypes.Slow)
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
            if (playerManagerScript.GetTurretType() == PlayerManager.TurretTypes.Slow)
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
