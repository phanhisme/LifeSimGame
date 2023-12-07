using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics; //calculating time for heaping!

public class Pathfinding : MonoBehaviour
{
    private Heap<Node> openSet;

    PathRequestManager requestManager;
    Grid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        openSet = new Heap<Node>(grid.MaxSize);
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos,Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (targetNode.walkable)
        {
            //Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            //clearing does not mean the data is gone, it makes the function ignore the data instead
            openSet.Clear();
            HashSet<Node> closedSet = new HashSet<Node>();

            //open: the set of nodes to be evaluated
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                //loop: current = node in OPEN with the lowest f_cost
                Node node = openSet.RemoveFirst();

                //add current to CLOSED (the set of nodes already evaluated)
                closedSet.Add(node);

                //if current is the target node (path has been found) -> return
                if (node == targetNode)
                {
                    //PATH FOUND! stop the stopwatch and check the time!
                    sw.Stop();
                    //cannot use debug in system.diagnostics
                    print("Path found: " + sw.ElapsedMilliseconds + " ms!");
                    pathSuccess = true;
                    break;
                }

                //for each neighbour of the current node
                //if neighbour is not traversavel or neighbour is in CLOSED => skip to the next neighbour
                //if new path to the nrightbour is shorter OR neighbour is not in OPEN
                //-> set f_cost of neighbour, set parent of neighbour to current
                //if neighbour is not in OPEN -> add neighbour to OPEN
                foreach (Node neighbour in grid.GetNeighbours(node))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        //hCost = distance from node to the end node
                        neighbour.hCost = GetDistance(neighbour, targetNode);

                        //set parent
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        //retrace to get the path
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path,startNode);
        Array.Reverse(waypoints);
        return waypoints;

        //grid.path = path;
    }

    Vector3[] SimplifyPath(List<Node> path, Node startNode)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            //change direction, i-1 since we started at 1
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

            if(directionNew!= directionOld)
            {
                waypoints.Add(path[i - 1].worldPosition);
            }

            directionOld = directionNew;

            //if i = the last node in the list, make sure the direction between this node and the startNode does not match directionOld
            //if true => add this new node as a waypoint
            //if (i == path.Count - 1 && directionOld != new Vector2(path[i].gridX, path[i].gridY) - new Vector2(startNode.gridX, startNode.gridY))
            //{
            //    waypoints.Add(path[path.Count - 1].worldPosition);
            //}
        }

        return waypoints.ToArray();
    }

    //horizontal first then vertical/horizontal
    //14y + 10.(x-y) if x > y
    //14x + 10.(y-x) if y > x
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
            return 14 * distX + 10 * (distY - distX);
    }
}