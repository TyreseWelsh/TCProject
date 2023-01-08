using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest>pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    // Function for unit to find a path from pathStart to pathEnd
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);                                                                      // Adding the new path request to the queue
        instance.TryProcessNext();                                                                                          // Try to find the next path in the queue
    }

    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)                                                                 // If a path is not already being processed and there are path requests in the queue
        {
            currentPathRequest = pathRequestQueue.Dequeue();                                                                // Remove the currently processing path from the queue
            isProcessingPath = true;                                                                                        // Set isProcessing to true so others cant be processed at this time
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);                            // Find path for currently requesting unit
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);                                                                         // Return if path was actually found and return the found path
        isProcessingPath = false;                                                                                           // Set isProcessing to false so other paths can be processed
        TryProcessNext();                                                                                                   // Attempt to process next path in queue
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    
    }
}
