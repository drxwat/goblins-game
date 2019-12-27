using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    List<Node> _path;

    public List<Node> path;


    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }


    public Node NodeFromWorldPosition(Vector3 position)
    {
        // Moving coordinate system to 0,0 be the bottom left of the grid
        Vector3 adjustedPosition = position + Vector3.right * gridWorldSize.x / 2 + Vector3.forward * gridWorldSize.y / 2;
        float xPercentage = Mathf.Clamp01(adjustedPosition.x / gridWorldSize.x);
        float yPercentage = Mathf.Clamp01(adjustedPosition.z / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPercentage);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodeWorldPosition = worldBottomLeft + 
                    Vector3.right * (x * nodeDiameter + nodeRadius) + 
                    Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(nodeWorldPosition, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, nodeWorldPosition, x, y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            Node playerNode = NodeFromWorldPosition(player.position);
            foreach(Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                if (playerNode == n)
                {
                    Gizmos.color = Color.green;
                }
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }

                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
