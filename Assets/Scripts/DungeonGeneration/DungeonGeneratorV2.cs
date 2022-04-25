using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorV2 : MonoBehaviour
{
    [Header("Settings")]
    public DungeonSettings settings;

    private GameObject dungeonParent;
    private int roomStepLimit { get { return GetDistance(entryChamber.RelativeRoomPosition, exitChamber.RelativeRoomPosition) + settings.MaxSubChamberDetours; } }
    private Chamber entryChamber;
    private Chamber exitChamber;

    private DungeonData dungeonData;

    // Start is called before the first frame update
    void Start()
    {
        dungeonParent = new GameObject();
        GenerateDungeon();
        GetComponent<DungeonBuilder>().BuildDungeon(dungeonData);
    }
    public void GenerateDungeon()
    {
        bool chained = false;
        int attempts = 0;
        while (!chained)
        {
            dungeonData = new DungeonData();
            CreateCoreRooms(dungeonData);
            chained = ChainCoreRooms(dungeonData);
            attempts++;
            if (attempts >= settings.MaxAttempts)
            {
                Debug.Log("Could not generate dungeon");
                return;
            }
        }
        CreateSubRooms(dungeonData);
        BuildChamberTiles(dungeonData);
        CreateCorridors(dungeonData);
    }


    private void CreateCoreRooms(DungeonData dungeonData)
    {
        entryChamber = new Chamber(new Vector2Int(0,0));
        dungeonData.chambers.Add(entryChamber.RelativeRoomPosition, entryChamber);

        exitChamber = new Chamber(new Vector2Int(0, settings.MaxVerticalRoomOffset));
        dungeonData.chambers.Add(exitChamber.RelativeRoomPosition, exitChamber);
    }
    private bool ChainCoreRooms(DungeonData dungeonData)
    {
        int step = 0;
        Chamber current = entryChamber;
        while(current != exitChamber)
        {
            List<Vector2Int> directions = new List<Vector2Int>();
            directions.Add(Vector2Int.right);
            directions.Add(Vector2Int.left);
            directions.Add(Vector2Int.up);
            directions.Add(Vector2Int.down);

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
                        if(!dungeonData.chambers.ContainsKey(roomPosition))
                        {
                            Chamber newChamber = new Chamber(roomPosition);
                            newChamber.connectedChambers.Add(current);
                            dungeonData.chambers.Add(roomPosition, newChamber);
                            current = newChamber;
                            break;
                        }
                    }
                }
            }
        }
        return false;
    }


    private void BuildChamberTiles(DungeonData dungeonData)
    {
        foreach (KeyValuePair<Vector2Int, Chamber> pair in dungeonData.chambers)
        {
            for (int x = 0; x < settings.RoomWidth; x++)
            {
                for (int y = 0; y < settings.RoomHeight; y++)
                {
                    Vector2Int tilePosition = new Vector2Int((pair.Value.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance)) + x, (pair.Value.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance)) + y);
                    dungeonData.tiles.Add(tilePosition);
                    dungeonData.nodes.Add(tilePosition, new Node(tilePosition, null, 0, 0));
                }
            }
            if(pair.Value == entryChamber)
            {
                Vector2Int position = new Vector2Int(entryChamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomWidth / 2), entryChamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) - 1);
                dungeonData.doors.Add(position);
                dungeonData.nodes.Add(position, new Node(position, null, 0, 0));
            }
            if (pair.Value == exitChamber)
            {
                Vector2Int position = new Vector2Int(exitChamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.RoomWidth / 2), exitChamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) + settings.RoomHeight);
                dungeonData.doors.Add(position);
                dungeonData.nodes.Add(position, new Node(position, null, 0, 0));
            }
        }
    }

 
    private void CreateSubRooms(DungeonData dungeonData)
    {
        for(int i = 0; i < settings.SubRoomIterations; i++)
        {
            List<Chamber> subRooms = new List<Chamber>();
            foreach (KeyValuePair<Vector2Int, Chamber> pair in dungeonData.chambers)
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
                        if (!dungeonData.chambers.ContainsKey(targetPosition))
                        {
                            Chamber subRoom = new Chamber(targetPosition);
                            subRoom.connectedChambers.Add(parent);
                            subRooms.Add(subRoom);
                        }
                        else
                        {
                            parent.connectedChambers.Add(dungeonData.chambers[targetPosition]);
                        }
                    }
                }
            }
            foreach (Chamber c in subRooms)
            {
                if (!dungeonData.chambers.ContainsKey(c.RelativeRoomPosition))
                {
                    dungeonData.chambers.Add(c.RelativeRoomPosition, c);
                }
            }
        }
    }
    private void CreateCorridors(DungeonData dungeonData)
    {
        foreach(KeyValuePair<Vector2Int, Chamber> pair in dungeonData.chambers)
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
                    if (!dungeonData.corridors.Contains(position))
                    {
                        dungeonData.corridors.Add(position);
                        dungeonData.nodes.Add(position, new Node(position, null, 0, 0));
                    }
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
