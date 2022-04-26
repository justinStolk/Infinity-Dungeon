using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorV2 : MonoBehaviour
{
    [Header("Settings")]
    public DungeonSettings settings;
    private int roomStepLimit { get { return GetDistance(entryChamber.RelativeRoomPosition, exitRoomRelativePosition) + settings.MaxSubChamberDetours; } }
    private Chamber entryChamber;
    private Chamber exitChamber;
    private Vector2Int roomSize;
    private Vector2Int exitRoomRelativePosition;

    private DungeonData dungeonData;

    // Start is called before the first frame update
    void Start()
    {
        if(settings.seed != 0)
        {
            Random.InitState(settings.seed);
        }
        roomSize = new Vector2Int(settings.MaximumRoomWidth, settings.MaximumRoomHeight);
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
        entryChamber = new Chamber(roomSize, new Vector2Int(0,0), new Vector2Int(0, 0));
        dungeonData.chambers.Add(entryChamber.RelativeRoomPosition, entryChamber);

        exitRoomRelativePosition = new Vector2Int(Random.Range(-settings.MaxHorizontalRoomOffset, settings.MaxHorizontalRoomOffset), settings.MaxVerticalRoomOffset);
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
                Vector2Int relativeRoomPos = current.RelativeRoomPosition + roomTarget;
                if (GetDistance(relativeRoomPos, exitRoomRelativePosition) > roomStepLimit - step || relativeRoomPos.x < -settings.MaxHorizontalRoomOffset || relativeRoomPos.x > settings.MaxHorizontalRoomOffset || relativeRoomPos.y < 0 || relativeRoomPos.y > settings.MaxVerticalRoomOffset || dungeonData.chambers.ContainsKey(relativeRoomPos))
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
                    if (relativeRoomPos == exitRoomRelativePosition)
                    {
                        Vector2Int roomPosition = new Vector2Int(current.Position.x + roomTarget.x * (roomSize.x + settings.RoomDistance), current.Position.y + roomTarget.y * (roomSize.y + settings.RoomDistance));
                        Vector2Int newDimensions = new Vector2Int(settings.MaximumRoomWidth, settings.MaximumRoomHeight);
                        exitChamber = new Chamber(newDimensions, roomPosition, exitRoomRelativePosition);
                        Debug.Log("Found the exit chamber!");
                        dungeonData.chambers.Add(exitChamber.RelativeRoomPosition, exitChamber);
                        current.connectedChambers.Add(exitChamber);
                        return true;
                    }
                    else
                    {
                        Vector2Int roomPosition = new Vector2Int(current.Position.x + roomTarget.x * (roomSize.x + settings.RoomDistance), current.Position.y + roomTarget.y * (roomSize.y + settings.RoomDistance));
                        Vector2Int newDimensions = new Vector2Int(settings.MaximumRoomWidth, settings.MaximumRoomHeight);
                        Chamber newChamber = new Chamber(roomSize, roomPosition, relativeRoomPos);
                        newChamber.connectedChambers.Add(current);
                        dungeonData.chambers.Add(relativeRoomPos, newChamber);
                        current = newChamber;
                        break;
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
            for (int x = 0; x < pair.Value.RoomDimensions.x; x++)
            {
                for (int y = 0; y < pair.Value.RoomDimensions.y; y++)
                {
                    Vector2Int tilePosition = new Vector2Int(pair.Value.Position.x + x, pair.Value.Position.y + y);
                    dungeonData.tiles.Add(tilePosition);
                    dungeonData.nodes.Add(tilePosition, new Node(tilePosition, null, 0, 0));
                }
            }
            if(pair.Value == entryChamber)
            {
                Vector2Int position = new Vector2Int(entryChamber.Position.x + Mathf.RoundToInt(entryChamber.RoomDimensions.x / 2),entryChamber.Position.y - 1);
                dungeonData.doors.Add(position);
                dungeonData.nodes.Add(position, new Node(position, null, 0, 0));
            }
            if (pair.Value == exitChamber)
            {
                Vector2Int position = new Vector2Int(exitChamber.Position.x + Mathf.RoundToInt(exitChamber.RoomDimensions.x / 2), exitChamber.Position.y + exitChamber.RoomDimensions.y);
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
                            Vector2Int roomPosition = new Vector2Int(pair.Value.Position.x + (roomSize.x + settings.RoomDistance) * direction.x, pair.Value.Position.y + (roomSize.y + settings.RoomDistance) * direction.y);
                            Vector2Int newDimensions = new Vector2Int(settings.MaximumRoomWidth, settings.MaximumRoomHeight);
                            Chamber subRoom = new Chamber(newDimensions, roomPosition, targetPosition);
                            parent.connectedChambers.Add(subRoom);
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
                //Vector2Int perceivedCenter = new Vector2Int(Mathf.RoundToInt(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) * 0.5f), Mathf.RoundToInt(chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) * 0.5f));
                for (int rw = 1; rw < settings.RoomDistance + 1; rw++)
                {
                    for(int w = 1; w <= settings.CorridorWidth; w++)
                    {
                        Vector2Int position = Vector2Int.zero;
                        if (chamber.RelativeRoomPosition.x > neighbour.RelativeRoomPosition.x)
                        {
                            position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.MaximumRoomWidth + settings.RoomDistance) - rw, chamber.RelativeRoomPosition.y * (settings.MaximumRoomHeight + settings.RoomDistance) + Mathf.RoundToInt(settings.MaximumRoomHeight / 2) - Mathf.FloorToInt(settings.CorridorWidth/2) + (w - 1));
                        }
                        if (chamber.RelativeRoomPosition.x < neighbour.RelativeRoomPosition.x)
                        {
                            position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.MaximumRoomWidth + settings.RoomDistance) + settings.MaximumRoomWidth + (rw - 1), chamber.RelativeRoomPosition.y * (settings.MaximumRoomHeight + settings.RoomDistance) + Mathf.RoundToInt(settings.MaximumRoomHeight / 2) - Mathf.FloorToInt(settings.CorridorWidth / 2) + (w - 1));
                        }
                        if (chamber.RelativeRoomPosition.y > neighbour.RelativeRoomPosition.y)
                        {
                            position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.MaximumRoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.MaximumRoomWidth / 2) - Mathf.FloorToInt(settings.CorridorWidth/2) + (w - 1), chamber.RelativeRoomPosition.y * (settings.MaximumRoomHeight + settings.RoomDistance) - rw);
                        }
                        if (chamber.RelativeRoomPosition.y < neighbour.RelativeRoomPosition.y)
                        {
                            position = new Vector2Int(chamber.RelativeRoomPosition.x * (settings.MaximumRoomWidth + settings.RoomDistance) + Mathf.RoundToInt(settings.MaximumRoomWidth / 2) - Mathf.FloorToInt(settings.CorridorWidth / 2) + (w - 1), chamber.RelativeRoomPosition.y * (settings.MaximumRoomHeight + settings.RoomDistance) + settings.MaximumRoomHeight + (rw - 1));
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
    }



    private int GetDistance(Vector2Int from, Vector2Int to)
    {
        Vector2Int direction = to - from;
        return Mathf.Abs(direction.x) + Mathf.Abs(direction.y);
    }
}
