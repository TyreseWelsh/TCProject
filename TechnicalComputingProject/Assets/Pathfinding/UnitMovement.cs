using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using System.Linq;
using System;

public class UnitMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] float speed;
    Vector3[] path;
    int targetIndex;

    public GameObject nodeGridObj;
    NodeGrid nodeGridScript;


    private void Awake()
    {
        nodeGridScript = nodeGridObj.GetComponent<NodeGrid>();
    }

    private void Start()
    {
        GetPath();
    }
    private void Update()
    {
        //if((nodeGridScript.gridLSize.x > 0 && nodeGridScript.gridLSize.y > 0) & on == true)
        //{
        //    on = false;
        //    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        //}
    }

    public void GetPath()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        print("once");
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
                print(currentWaypoint.x + " " + currentWaypoint.y + " " + currentWaypoint.z);
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            yield return null;
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
