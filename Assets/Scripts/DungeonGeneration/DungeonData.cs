using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public Dictionary<Vector2Int, Navigator> units = new();
    public Dictionary<Vector2Int, Node> nodes = new();
    public Dictionary<Vector2Int, GameObject> moveRangeTiles = new();
    public Dictionary<Vector2Int, GameObject> attackRangeTiles = new();

    public Dictionary<Vector2Int, Chamber> chambers = new();
    public List<Vector2Int> tiles = new();
    public List<Vector2Int> doors = new();
    public List<Vector2Int> corridors = new();
}
