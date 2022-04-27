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

public class DataConverter
{
    public List<Chamber> chambers = new();
    public List<Node> nodes = new();
   // public List<Navigator> units = new();
    public List<Vector2Int> tiles;
    public List<Vector2Int> doors;
    public List<Vector2Int> corridors;

    public void ConvertDungeonData(DungeonData dataToConvert)
    {
        tiles = dataToConvert.tiles;
        doors = dataToConvert.doors;
        corridors = dataToConvert.corridors;

        foreach(KeyValuePair<Vector2Int, Node> pair in dataToConvert.nodes)
        {
            nodes.Add(pair.Value);
        }
        foreach (KeyValuePair<Vector2Int, Chamber> pair in dataToConvert.chambers)
        {
            chambers.Add(pair.Value);
        }
    }

    public DungeonData ConvertToData()
    {
        DungeonData data = new DungeonData();
        
        data.tiles = tiles;
        data.doors = doors;
        data.corridors = corridors;

        foreach(Chamber c in chambers)
        {
            data.chambers.Add(c.RelativeRoomPosition, c);
        }
        foreach (Node n in nodes)
        {
            data.nodes.Add(n.Position, n);
            if(n.occupyingElement.TryGetComponent<Navigator>(out Navigator nav))
            {
                data.units.Add(n.Position, nav);
            }
        }
        return data;
    }

}
