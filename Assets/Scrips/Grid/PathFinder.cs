using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Path
{
    public Vector3[] waypoints;
    public bool successful;

    public Path(Vector3[] _waypoints, bool _successful)
    {
        waypoints = _waypoints;
        successful = _successful;
    }
}

public class PathFinder : MonoBehaviour
{
    public static PathFinder instance;

    Grid grid;

    void Awake()
    {
        instance = this;
        grid = GetComponent<Grid>();
    }

    public Path FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3[] waypoints = null;

        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(targetPosition);

        if (!startNode.walkable || !targetNode.walkable)
        {
            return new Path(new Vector3[0], false);
        }

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            // picking node with lowest cost
            Node currentNode = openSet.RemoveFirst(); 
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                waypoints = RetraceWaypoints(startNode, targetNode);
                break;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbour);
                // opening neighbour node
                if (newMovementCostToNeighbor < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbor;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    } else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return new Path(waypoints == null ? new Vector3[0] : waypoints, true);
    }

    Vector3[] RetraceWaypoints(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
