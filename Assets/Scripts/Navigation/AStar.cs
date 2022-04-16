using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    private Dictionary<Vector2Int, Node> nodes;
    private bool canMoveDiagonally;
    public AStar(ref Dictionary<Vector2Int, Node> nodeDictionary, bool CanMoveDiagonally)
    {
        nodes = nodeDictionary;
        canMoveDiagonally = CanMoveDiagonally;
    }

    public List<Vector2Int> FindPathToTarget(Vector2Int startPosition, Vector2Int targetPosition)
    {
        List<Node> open = new();
        List<Node> closed = new();

        Node startNode = nodes[startPosition];
        open.Add(startNode);

        while (open.Count > 0)
        {
            Node current = open.OrderBy((x) => x.FCost).First();
            if (current.Position == targetPosition)
            {
                List<Vector2Int> result = new();
                result.Add(current.Position);
                while (current.Position != targetPosition)
                {
                    result.Add(current.Parent.Position);
                    current = current.Parent;
                }
                result.Reverse();
                return result;
            }
            List<Node> neighbours = GetNeighbours(current);
            foreach (Node neighbour in neighbours)
            {
                if (closed.Contains(neighbour))
                {
                    continue;
                }
                float tentativeGCost = current.GCost + GetDistance(current, neighbour);
                if (tentativeGCost < neighbour.GCost || !open.Contains(neighbour))
                {
                    neighbour.GCost = tentativeGCost;
                    neighbour.HCost = Vector2Int.Distance(neighbour.Position, targetPosition);
                    neighbour.Parent = current;
                    if(IsTraversable(neighbour) && !open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }
    private bool IsTraversable(Node nodeToEvaluate)
    {
        return true;
    }

    private List<Node> GetNeighbours(Node currentNode)
    {
        List<Node> neighbours = new();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y<= 1; y++)
            {
                if(!canMoveDiagonally && Mathf.Abs(x) == Mathf.Abs(y) || x == 0 && y == 0 || !nodes.ContainsKey(currentNode.Position + new Vector2Int(x,y)))
                {
                    continue;
                }
                neighbours.Add(nodes[currentNode.Position + new Vector2Int(x, y)]);
            }
        }
        return neighbours;
    }

    private int GetDistance(Node from, Node to)
    {
        int distanceX = from.Position.x - to.Position.x;
        int distanceY = from.Position.y - to.Position.y;
        if (canMoveDiagonally)
        {
            if(distanceX > distanceY)
            {
                return 10 * (distanceX - distanceY) + 14 * distanceY;
            }
            return 10 * (distanceY - distanceX) + 14 * distanceX;
        }
        return 10 * distanceX + 10 * distanceY;
    }


}