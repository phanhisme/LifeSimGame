using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour
{
    //public Transform[] targets;

    Vector3[] path;
    float speed = 5f;
    int targetIndex;

    public bool atTargetPosition = false;

    private Grid gridScript;

    private void Start()
    {
        //never put this on update -> Call it just once when you have to find the path
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        gridScript = FindObjectOfType<Grid>();
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            StopCoroutine(FollowPath());
            targetIndex = 0;
            path = newPath;
            //reset target index back to 0

            //atTargetPosition = false;
            //StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        if (path == null)
        {
            Debug.Log("...");
        }
        else
            Debug.Log("path is there");
        //error. 8/12 -> trying to acess a non-existing waypoints
        //first vector 3 in the path array
        Vector3 currentWaypoints = path[0];
        Debug.Log(currentWaypoints);

        while (true)
        {
            if (transform.position == currentWaypoints)
            {
                //advance to the next waypoints
                targetIndex++;

                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoints = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoints, speed * Time.deltaTime);

            yield return null;
        }
    }

    public void CheckOnTarget()
    {
        if (path != null)
        {
            //Debug.Log(path.Length);

            //float xValue = this.transform.position.x;
            //float yValue = this.transform.position.y;

            //check if the player is already at the target position;
            Node AINode = gridScript.NodeFromWorldPoint(this.transform.position);
            Node targetNode = gridScript.NodeFromWorldPoint(path[path.Length - 1]);
            if (AINode == targetNode)
            {
                atTargetPosition = true;
            }
            else
            {
                atTargetPosition = false;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for(int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}
