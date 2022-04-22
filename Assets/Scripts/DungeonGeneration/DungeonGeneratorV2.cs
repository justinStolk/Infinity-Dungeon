using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorV2 : MonoBehaviour
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

    [Header("Settings")]
    public DungeonSettings settings;

    public Dictionary<Vector2Int, Node> nodes = new();
    public Dictionary<Vector2Int, GameObject> moveRangeTiles = new(); 
    public Dictionary<Vector2Int, GameObject> attackRangeTiles = new();

    private GameObject dungeonParent;
    private int roomStepLimit { get { return GetDistance(entryChamber.RelativeRoomPosition, exitChamber.RelativeRoomPosition) + settings.MaxSubChamberDetours; } }
    private Chamber entryChamber;
    private Chamber exitChamber;

    private Dictionary<Vector2Int, Chamber> chambers = new();
    private List<Vector2Int> tiles = new();
    private List<Vector2Int> corridors = new();

    // Start is called before the first frame update
    void Start()
    {
        dungeonParent = new GameObject("Dungeon");
        GenerateDungeon();
    }
    public void GenerateDungeon()
    {
        bool chained = false;
        int attempts = 0;
        while (!chained)
        {
            chambers.Clear();
            tiles.Clear();
            corridors.Clear();
            CreateCoreRooms();
            chained = ChainCoreRooms();
            attempts++;
            if (attempts >= settings.MaxAttempts)
            {
                Debug.Log("Could not generate dungeon");
                return;
            }
        }
        CreateSubRooms();
        BuildChamberTiles();
        CreateCorridors();
        BuildTiles();
        BuildWalls();
    }


    private void CreateCoreRooms()
    {
        entryChamber = new Chamber(new Vector2Int(0,0));
        chambers.Add(entryChamber.RelativeRoomPosition, entryChamber);

        exitChamber = new Chamber(new Vector2Int(0, settings.MaxVerticalRoomOffset));
        chambers.Add(exitChamber.RelativeRoomPosition, exitChamber);
    }
    private bool ChainCoreRooms()
    {
        int step = 0;
        Chamber current = entryChamber;
        while(current != exitChamber)
        {
            List<Vector2Int> directions = new List<Vector2Int>();
            directions.Add(new Vector2Int(1, 0));
            directions.Add(new Vector2Int(-1, 0));
            directions.Add(new Vector2Int(0, 1));
            directions.Add(new Vector2Int(0, -1));

            while(directions.Count > 0)
            {
                Vector2Int roomTarget = directions[Random.Range(0, directions.Count - 1)];
                Vector2Int roomPosition = current.RelativeRoomPosition + roomTarget;
                if (GetDistance(roomPosition, exitChamber.RelativeRoomPosition) > roomStepLimit - step || roomPosition.x < -settings.MaxHorizontalRoomOffset || roomPosition.x > settings.MaxHorizontalRoomOffset || roomPosition.y < 0 || roomPosition.y > settings.MaxVerticalRoomOffset)
                {
                    directions.Remove(roomTarget);
                    if(directions.Count == 0)
                    {
                        Debug.Log("Couldn't find a path from available directions!");
                        return false;
                    }
                }
                else
                {
                    step++;
                    if (roomPosition == exitChamber.RelativeRoomPosition)
                    {
                        Debug.Log("Found the exit chamber!");
                        current.connectedChambers.Add(exitChamber);
                        return true;
                    }
                    else
                    {
                        if(!chambers.ContainsKey(roomPosition))
                        {
                            Chamber newChamber = new Chamber(roomPosition);
                            newChamber.connectedChambers.Add(current);
                            chambers.Add(roomPosition, newChamber);
                            current = newChamber;
                            break;
                        }
                    }
                }
            }
        }
        return false;
    }


    private void BuildChamberTiles()
    {
        foreach (KeyValuePair<Vector2Int, Chamber> pair in chambers)
        {
            for (int x = 0; x < settings.RoomWidth; x++)
            {
                for (int y = 0; y < settings.RoomHeight; y++)
                {
                    Vector2Int tilePosition = new Vector2Int((pair.Value.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance)) + x, (pair.Value.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance)) + y);
                    tiles.Add(tilePosition);
                    nodes.Add(tilePosition, new Node(tilePosition, null, 0, 0));
                }
            }
            if(pair.Value == entryChamber)
            {
                Vector2Int position = new Vector2Int(entryChamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomWidth / 2), entryChamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) - 1);
                GameObject door = Instantiate(doorwayPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, dungeonParent.transform);
                GameObject unit = Instantiate(testUnit, new Vector3(position.x, position.y, 0), Quaternion.identity);
                nodes.Add(position, new Node(position, null, 0, 0));
                nodes[position].occupyingElement = unit;
            }
            if (pair.Value == exitChamber)
            {
                Vector2Int position = new Vector2Int(exitChamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomWidth / 2), exitChamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) + settings.RoomHeight);
                Instantiate(doorwayPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, dungeonParent.transform);
                nodes.Add(position, new Node(position, null, 0, 0));
            }
        }
    }

 
    private void CreateSubRooms()
    {
        for(int i = 0; i < settings.SubRoomIterations; i++)
        {
            List<Chamber> subRooms = new List<Chamber>();
            foreach (KeyValuePair<Vector2Int, Chamber> pair in chambers)
            {
                Chamber parent = pair.Value;
                Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.up };
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int targetPosition = direction + parent.RelativeRoomPosition;
                    if (targetPosition.x < -settings.MaxHorizontalRoomOffset || targetPosition.x > settings.MaxHorizontalRoomOffset || targetPosition.y < 0 || targetPosition.y > settings.MaxVerticalRoomOffset)
                    {
                        continue;
                    }
                    if (Random.Range(0, 100) <= settings.RoomSpawnChance)
                    {
                        if (!chambers.ContainsKey(targetPosition))
                        {
                            Chamber subRoom = new Chamber(targetPosition);
                            subRoom.connectedChambers.Add(parent);
                            subRooms.Add(subRoom);
                        }
                        else
                        {
                            parent.connectedChambers.Add(chambers[targetPosition]);
                        }
                    }
                }
            }
            foreach (Chamber c in subRooms)
            {
                if (!chambers.ContainsKey(c.RelativeRoomPosition))
                {
                    chambers.Add(c.RelativeRoomPosition, c);
                }
            }
        }
    }
    private void CreateCorridors()
    {
        foreach(KeyValuePair<Vector2Int, Chamber> pair in chambers)
        {
            Chamber chamber = pair.Value;
            foreach (Chamber neighbour in chamber.connectedChambers)
            {
                Vector2Int perceivedCenter = new Vector2Int(Mathf.RoundToInt(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) * 0.5f), Mathf.RoundToInt(chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) * 0.5f));
                for(int rw = 1; rw < settings.RoomDistance + 1; rw++)
                {
                    Vector2Int position = Vector2Int.zero;
                    if (chamber.RelativeRoomPosition.x > neighbour.RelativeRoomPosition.x)
                    {
                        position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) - rw, chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomHeight / 2 ));
                    }
                    if (chamber.RelativeRoomPosition.x < neighbour.RelativeRoomPosition.x)
                    {
                        position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + settings.RoomWidth + (rw - 1), chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomHeight / 2));
                    }
                    if (chamber.RelativeRoomPosition.y > neighbour.RelativeRoomPosition.y)
                    {
                        position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomWidth / 2), chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) - rw);
                    }
                    if (chamber.RelativeRoomPosition.y < neighbour.RelativeRoomPosition.y)
                    {
                        position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomWidth / 2), chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) + settings.RoomHeight + (rw - 1));
                    }
                    if (!corridors.Contains(position))
                    {
                        corridors.Add(position);
                        nodes.Add(position, new Node(position, null, 0, 0));
                    }
                }
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

        foreach (Vector2Int pos in tiles)
        {
            Instantiate(floorPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, roomFloors.transform);
        }
        foreach (Vector2Int pos in corridors)
        {
            Instantiate(corridorPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, corridorFloors.transform);
        }
        foreach(KeyValuePair<Vector2Int,Node> pair in nodes)
        {
            GameObject rangeTile = Instantiate(moveRangePrefab, new Vector3(pair.Key.x, pair.Key.y, 0), Quaternion.identity, rangeTileParent.transform);
            moveRangeTiles.Add(pair.Key, rangeTile);
            rangeTile.SetActive(false);

            GameObject attackTile = Instantiate(attackRangePrefab, new Vector3(pair.Key.x, pair.Key.y, 0), Quaternion.identity, attackTileParent.transform);
            attackRangeTiles.Add(pair.Key, attackTile);
            attackTile.SetActive(false);
        }
    }
    private void BuildWalls()
    {
        List<Vector2Int> sideWalls = new();
        List<Vector2Int> topWalls = new();
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(dungeonParent.transform);
        foreach (KeyValuePair<Vector2Int, Node> pair in nodes)
        {
            for(int x = -1; x<= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    Vector2Int newWallPosition = pair.Key + new Vector2Int(x, y);
                    {
                        if (!nodes.ContainsKey(newWallPosition))
                        {
                            if (nodes.ContainsKey(newWallPosition + Vector2Int.up) && !topWalls.Contains(newWallPosition))
                            {
                                topWalls.Add(newWallPosition);
                                if (!nodes.ContainsKey(newWallPosition + Vector2Int.right) && !nodes.ContainsKey(newWallPosition + Vector2Int.left) && !sideWalls.Contains(newWallPosition - Vector2Int.up))
                                {
                                    sideWalls.Add(newWallPosition - Vector2Int.up);
                                }
                            }else if (nodes.ContainsKey(newWallPosition - Vector2Int.up) && !sideWalls.Contains(newWallPosition))
                            {
                                topWalls.Add(newWallPosition + Vector2Int.up);
                                sideWalls.Add(newWallPosition);
                            }
                            else if(!topWalls.Contains(newWallPosition))
                            {
                                topWalls.Add(newWallPosition);
                            }
                        }
                    }
                }
            }
        }
        foreach(Vector2Int t in topWalls)
        {
            Instantiate(wallTopPrefab, new Vector3(t.x, t.y, 0), Quaternion.identity, walls.transform);
            Vector2Int below = t - Vector2Int.up;
            if (sideWalls.Contains(below))
            {
                Vector2Int leftSide = t + Vector2Int.left;
                Vector2Int rightSide = t + Vector2Int.right;
                if (!nodes.ContainsKey(leftSide) && !sideWalls.Contains(leftSide) && !topWalls.Contains(leftSide))
                {
                    Instantiate(wallTopPrefab, new Vector3(leftSide.x, leftSide.y, 0), Quaternion.identity, walls.transform);
                }
                if (!nodes.ContainsKey(rightSide) && !sideWalls.Contains(rightSide) && !topWalls.Contains(rightSide))
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
                if (!nodes.ContainsKey(leftSide) && !sideWalls.Contains(leftSide) && !topWalls.Contains(leftSide))
                {
                    Instantiate(wallSidePrefab, new Vector3(leftSide.x, leftSide.y, 0), Quaternion.identity, walls.transform);
                }
                if (!nodes.ContainsKey(rightSide) && !sideWalls.Contains(rightSide) && !topWalls.Contains(rightSide))
                {
                    Instantiate(wallSidePrefab, new Vector3(rightSide.x, rightSide.y, 0), Quaternion.identity, walls.transform);
                }
            }
        }

    }

    private int GetDistance(Vector2Int from, Vector2Int to)
    {
        Vector2Int direction = to - from;
        return Mathf.Abs(direction.x) + Mathf.Abs(direction.y);
    }
}
