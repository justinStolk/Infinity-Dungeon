using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject corridorPrefab;
    public GameObject doorwayPrefab;
    public GameObject moveRangePrefab;
    public GameObject attackRangePrefab;
    public GameObject testUnit;
    public GameObject wallSidePrefab;
    public GameObject wallTopPrefab;

    public DungeonData data;
    private GameObject dungeonParent;

    public void BuildDungeon(DungeonData dungeonData)
    {
        data = dungeonData;
        dungeonParent = new GameObject("Dungeon");
        BuildTiles();
        BuildWalls();
        foreach(KeyValuePair<Vector2Int, Chamber> pair in data.chambers)
        {
            if(pair.Value.GetType() == typeof(SpawnChamber))
            {
                SpawnChamber spawn = (SpawnChamber)pair.Value;
                Debug.Log("I found a Spawn Chamber at position: " + pair.Value.Position);
                GameObject spawner = Resources.Load("Objects/Spawner") as GameObject;
                GameObject spawnedSpawner = Instantiate(spawner, new Vector3(spawn.SpawnerPosition.x, spawn.SpawnerPosition.y, 0), Quaternion.identity);
                data.nodes[spawn.SpawnerPosition].occupyingElement = spawnedSpawner;
            }
        }
    }
    private void BuildTiles()
    {
        GameObject roomFloors = new GameObject("Rooms");
        roomFloors.transform.SetParent(dungeonParent.transform);
        GameObject corridorFloors = new GameObject("Corridors");
        corridorFloors.transform.SetParent(dungeonParent.transform);
        GameObject rangeTileParent = new GameObject("RangeTiles");
        rangeTileParent.transform.SetParent(dungeonParent.transform);
        GameObject attackTileParent = new GameObject("AttackRangeTiles");
        attackTileParent.transform.SetParent(dungeonParent.transform);

        foreach (Vector2Int pos in data.tiles)
        {
            Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, roomFloors.transform);
        }
        foreach (Vector2Int pos in data.corridors)
        {
            Instantiate(corridorPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, corridorFloors.transform);
        }
        foreach (KeyValuePair<Vector2Int, Node> pair in data.nodes)
        {
            GameObject rangeTile = Instantiate(moveRangePrefab, new Vector3(pair.Key.x, pair.Key.y, 0), Quaternion.identity, rangeTileParent.transform);
            data.moveRangeTiles.Add(pair.Key, rangeTile);
            rangeTile.SetActive(false);

            GameObject attackTile = Instantiate(attackRangePrefab, new Vector3(pair.Key.x, pair.Key.y, 0), Quaternion.identity, attackTileParent.transform);
            data.attackRangeTiles.Add(pair.Key, attackTile);
            attackTile.SetActive(false);
        }
    }
    private void BuildWalls()
    {
        List<Vector2Int> sideWalls = new();
        List<Vector2Int> topWalls = new();
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(dungeonParent.transform);
        foreach (KeyValuePair<Vector2Int, Node> pair in data.nodes)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int newWallPosition = pair.Key + new Vector2Int(x, y);
                    {
                        if (!data.nodes.ContainsKey(newWallPosition))
                        {
                            if (data.nodes.ContainsKey(newWallPosition + Vector2Int.up) && !topWalls.Contains(newWallPosition))
                            {
                                topWalls.Add(newWallPosition);
                                if (!data.nodes.ContainsKey(newWallPosition + Vector2Int.right) && !data.nodes.ContainsKey(newWallPosition + Vector2Int.left) && !sideWalls.Contains(newWallPosition - Vector2Int.up))
                                {
                                    sideWalls.Add(newWallPosition - Vector2Int.up);
                                }
                            }
                            else if (data.nodes.ContainsKey(newWallPosition - Vector2Int.up) && !sideWalls.Contains(newWallPosition))
                            {
                                topWalls.Add(newWallPosition + Vector2Int.up);
                                sideWalls.Add(newWallPosition);
                            }
                            else if (!topWalls.Contains(newWallPosition))
                            {
                                topWalls.Add(newWallPosition);
                            }
                        }
                    }
                }
            }
        }
        foreach(Vector2Int d in data.doors)
        {
            Instantiate(doorwayPrefab, new Vector3(d.x, d.y, 0), Quaternion.identity, walls.transform);
            if(d.y < 0)
            {
                GameObject unit = Instantiate(testUnit, new Vector3(d.x, d.y, 0), Quaternion.identity);
                data.nodes[d].occupyingElement = unit; 
            }
        }
        foreach (Vector2Int t in topWalls)
        {
            Instantiate(wallTopPrefab, new Vector3(t.x, t.y, 0), Quaternion.identity, walls.transform);
            Vector2Int below = t - Vector2Int.up;
            if (sideWalls.Contains(below))
            {
                Vector2Int leftSide = t + Vector2Int.left;
                Vector2Int rightSide = t + Vector2Int.right;
                if (!data.nodes.ContainsKey(leftSide) && !sideWalls.Contains(leftSide) && !topWalls.Contains(leftSide))
                {
                    Instantiate(wallTopPrefab, new Vector3(leftSide.x, leftSide.y, 0), Quaternion.identity, walls.transform);
                }
                if (!data.nodes.ContainsKey(rightSide) && !sideWalls.Contains(rightSide) && !topWalls.Contains(rightSide))
                {
                    Instantiate(wallTopPrefab, new Vector3(rightSide.x, rightSide.y, 0), Quaternion.identity, walls.transform);
                }
            }
        }
        foreach (Vector2Int s in sideWalls)
        {
            Instantiate(wallSidePrefab, new Vector3(s.x, s.y, 0), Quaternion.identity, walls.transform);
            Vector2Int above = s + Vector2Int.up;
            if (topWalls.Contains(above))
            {
                Vector2Int leftSide = s + Vector2Int.left;
                Vector2Int rightSide = s + Vector2Int.right;
                if (!data.nodes.ContainsKey(leftSide) && !sideWalls.Contains(leftSide) && !topWalls.Contains(leftSide))
                {
                    Instantiate(wallSidePrefab, new Vector3(leftSide.x, leftSide.y, 0), Quaternion.identity, walls.transform);
                }
                if (!data.nodes.ContainsKey(rightSide) && !sideWalls.Contains(rightSide) && !topWalls.Contains(rightSide))
                {
                    Instantiate(wallSidePrefab, new Vector3(rightSide.x, rightSide.y, 0), Quaternion.identity, walls.transform);
                }
            }
        }

    }
}
