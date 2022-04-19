using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int Position;
    public float GCost, HCost;
    public float FCost { get { return GCost + HCost; } }
    public Node Parent;
    public GameObject occupyingElement = null;

    public Node(Vector2Int position, Node parent, float gCost, float hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }

}
