using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //true by default to stop the delay casusing by grids
    public bool displayGridGizmos;
    //public Transform playerPos;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        //playerPos = GameObject.Find("AI").transform;

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public bool CheckWalkable(SmartObject selectedObject)
    {
        if (grid != null)
        {
            Node itemNode = NodeFromWorldPoint(selectedObject.gameObject.transform.position);
            if (itemNode.walkable)
            {
                //Debug.Log(selectedObject.DisplayName + " is walkable!");
                selectedObject.isWalkable = true;
            }
            else
            {
                //Debug.Log(selectedObject.name + " is not walkable!");
                selectedObject.isWalkable = false;
                return false;
            }

            //SmartObject[] items = FindObjectsOfType<SmartObject>();
            //foreach (SmartObject item in items)
            //{
            //    Node node = NodeFromWorldPoint(item.gameObject.transform.position);

            //    if (itemNode.walkable)
            //    {
            //        Debug.Log(item.name + " is walkable!");
            //        item.isWalkable = true;
            //    }
            //    else
            //    {
            //        Debug.Log(item.name + " is not walkable!");
            //        item.isWalkable = false;
            //    }
            //}
        }
        return true;
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //node given to us!
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int CheckX = node.gridX + x;
                int CheckY = node.gridY + y;

                if (CheckX >= 0 && CheckX < gridSizeX && CheckY >= 0 && CheckY < gridSizeY)
                {
                    neighbours.Add(grid[CheckX, CheckY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    //create path (for retrace)
    //public List<Node> path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            //to find the player
            //Node playerNode = NodeFromWorldPoint(playerPos.position);
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.red;

                //if (path != null)
                //{
                //    if (path.Contains(node))
                //    {
                //        Gizmos.color = Color.black;
                //    }
                //}

                //player location to cyan
                //if (playerNode == node)
                //{
                //    Gizmos.color = Color.cyan;
                //}

                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }

        }
    }
}
