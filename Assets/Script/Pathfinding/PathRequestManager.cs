using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;

    Pathfinding pathfindingScript;
    bool isProcessingPath;

    private void Awake()
    {
        //singleton
        instance = this;
        pathfindingScript = GetComponent<Pathfinding>();
    }

    //callback make sure path request successful
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        
        //enqueue the new request
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    //check if a path is being created, else asks the pathfinding script to process the next one
    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0) 
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfindingScript.StartFindPath(currentPathRequest.pathStart,currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;

        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 _start,Vector3 _end, Action<Vector3[],bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
