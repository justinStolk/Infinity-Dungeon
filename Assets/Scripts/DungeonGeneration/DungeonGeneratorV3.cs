using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorV3 : MonoBehaviour
{
    public DungeonSettings settings;
    public enum OverlapPolicy { Ignore, Retry, Merge }
    public OverlapPolicy Policy;
    public int policyRetries;

    private Vector2Int exitRoomTarget;
    private DungeonData dungeonData;
    private Chamber entryChamber;
    private Chamber exitChamber;



    private int roomStepLimit { get { return GetDistance(entryChamber.RelativeRoomPosition, exitRoomTarget) + settings.MaxSubChamberDetours; } }

    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeon();
        GetComponent<DungeonBuilder>().BuildDungeon(dungeonData);
    }

    public void GenerateDungeon()
    {
        dungeonData = new DungeonData();
        CreateCoreRooms(dungeonData);
        ChainCoreRooms(dungeonData);
        CreateSubRooms(dungeonData);
        //BuildChamberTiles(dungeonData);
        CreateCorridors(dungeonData);
    }

    private void CreateCoreRooms(DungeonData dungeonData)
    {
        Vector2Int entryDimensions = new Vector2Int(Random.Range(settings.MinimumRoomWidth, settings.MaximumRoomWidth), Random.Range(settings.MinimumRoomHeight, settings.MaximumRoomHeight));
        entryChamber = new Chamber(entryDimensions, Vector2Int.zero, Vector2Int.zero);
        CreateNewChamber(entryChamber, null);
        //dungeonData.chambers.Add(Vector2Int.zero, entryChamber);

        //Vector2Int exitDimensions = new Vector2Int(Random.Range(settings.MinimumRoomWidth, settings.MaximumRoomWidth), Random.Range(settings.MinimumRoomHeight, settings.MaximumRoomHeight));
        exitRoomTarget = new Vector2Int(Random.Range(-settings.MaxHorizontalRoomOffset, settings.MaxHorizontalRoomOffset), settings.MaxVerticalRoomOffset);
        //exitChamber = new Chamber(exitDimensions, Vector2Int.zero, exitRoomTarget);
        //dungeonData.chambers.Add(exitRoomTarget, exitChamber);

    }

    private void ChainCoreRooms(DungeonData dungeonData)
    {
        int step = 0;
        Chamber current = entryChamber;
        while (current != exitChamber)
        {
            List<Vector2Int> directions = new List<Vector2Int>();
            directions.Add(Vector2Int.right);
            directions.Add(Vector2Int.left);
            directions.Add(Vector2Int.up);
            directions.Add(Vector2Int.down);

            while (directions.Count > 0)
            {
                Vector2Int roomTarget = directions[Random.Range(0, directions.Count - 1)];
                Vector2Int newRelativePosition = current.RelativeRoomPosition + roomTarget;
                if (GetDistance(newRelativePosition, exitRoomTarget) > roomStepLimit - step || newRelativePosition.x < -settings.MaxHorizontalRoomOffset || newRelativePosition.x > settings.MaxHorizontalRoomOffset || newRelativePosition.y < 0 || newRelativePosition.y > settings.MaxVerticalRoomOffset || dungeonData.chambers.ContainsKey(newRelativePosition))
                {
                    directions.Remove(roomTarget);
                    if (directions.Count == 0)
                    {
                        Debug.Log("Couldn't find a path from available directions!");
                        return;
                    }
                }
                else
                {
                    step++;
                    if (newRelativePosition == exitRoomTarget)
                    {
                        Vector2Int exitDimensions = new Vector2Int(Random.Range(settings.MinimumRoomWidth, settings.MaximumRoomWidth), Random.Range(settings.MinimumRoomHeight, settings.MaximumRoomHeight));
                        Debug.Log("Found the exit chamber!");
                        if (roomTarget.x == 1)
                        {
                            Vector2Int exitRoomPosition = new Vector2Int(current.Position.x + current.RoomDimensions.x + settings.RoomDistance, current.Position.y);
                            exitChamber = new Chamber(exitDimensions, exitRoomPosition, exitRoomTarget);
                            CreateNewChamber(exitChamber, current);
                        }
                        if (roomTarget.x == -1)
                        {
                            Vector2Int exitRoomPosition = new Vector2Int(current.Position.x - exitDimensions.x - settings.RoomDistance, current.Position.y);
                            exitChamber = new Chamber(exitDimensions, exitRoomPosition, exitRoomTarget);
                            CreateNewChamber(exitChamber, current);
                        }
                        if (roomTarget.y == 1)
                        {
                            Vector2Int exitRoomPosition = new Vector2Int(current.Position.x, current.Position.y + current.RoomDimensions.y + settings.RoomDistance);
                            exitChamber = new Chamber(exitDimensions, exitRoomPosition, exitRoomTarget);
                            CreateNewChamber(exitChamber, current);
                        }
                        if (roomTarget.y == -1)
                        {
                            Vector2Int exitRoomPosition = new Vector2Int(current.Position.x, current.Position.y - exitDimensions.y - settings.RoomDistance);
                            exitChamber = new Chamber(exitDimensions, exitRoomPosition, exitRoomTarget);
                            CreateNewChamber(exitChamber, current);
                        }
                        return;
                    }
                    else
                    {
                        Vector2Int newDimensions = new Vector2Int(Random.Range(settings.MinimumRoomWidth, settings.MaximumRoomWidth), Random.Range(settings.MinimumRoomHeight, settings.MaximumRoomHeight));
                        Vector2Int newChamberPosition = Vector2Int.zero;
                        if(roomTarget.x == 1)
                        {
                            newChamberPosition = new Vector2Int(current.Position.x + current.RoomDimensions.x + settings.RoomDistance, current.Position.y);
                        }
                        if (roomTarget.x == -1)
                        {
                            newChamberPosition = new Vector2Int(current.Position.x - newDimensions.x - settings.RoomDistance, current.Position.y);
                        }
                        if (roomTarget.y == 1)
                        {
                            newChamberPosition = new Vector2Int(current.Position.x, current.Position.y + current.RoomDimensions.y + settings.RoomDistance);
                        }
                        if (roomTarget.y == -1)
                        {
                            newChamberPosition = new Vector2Int(current.Position.x, current.Position.y - newDimensions.y - settings.RoomDistance);
                        }
                        Chamber newChamber = new Chamber(newDimensions, newChamberPosition, newRelativePosition);
                        CreateNewChamber(newChamber, current);
                    }
                }
            }
        }
    }
    private void CreateSubRooms(DungeonData dungeonData)
    {
        
    }
    private void CreateCorridors(DungeonData dungeonData)
    {
        foreach (KeyValuePair<Vector2Int, Chamber> pair in dungeonData.chambers)
        {
            Chamber chamber = pair.Value;
            foreach (Chamber neighbour in chamber.connectedChambers)
            {
                //Vector2Int perceivedCenter = new Vector2Int(Mathf.RoundToInt(chamber.RelativeRoomPosition.x * (settings.RoomWidth + settings.RoomDistance) * 0.5f), Mathf.RoundToInt(chamber.RelativeRoomPosition.y * (settings.RoomHeight + settings.RoomDistance) * 0.5f));
                for (int rw = 1; rw < settings.RoomDistance + 1; rw++)
                {
                    Vector2Int position = Vector2Int.zero;
                    if (chamber.RelativeRoomPosition.x > neighbour.RelativeRoomPosition.x)
                    {
                        position = new Vector2Int(chamber.Position.x - rw, chamber.Position.y);
                    }
                    if (chamber.RelativeRoomPosition.x < neighbour.RelativeRoomPosition.x)
                    {
                        position = new Vector2Int(neighbour.Position.x - rw, chamber.Position.y);
                    }
                    if (chamber.RelativeRoomPosition.y > neighbour.RelativeRoomPosition.y)
                    {
                        position = new Vector2Int(chamber.Position.x, chamber.Position.y - rw);
                    }
                    if (chamber.RelativeRoomPosition.y < neighbour.RelativeRoomPosition.y)
                    {
                        position = new Vector2Int(chamber.Position.x, neighbour.Position.y - rw);
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
    private bool CheckIfChamberFits(Chamber chamber)
    {
        for(int x = chamber.Position.x; x < chamber.Position.x + chamber.RoomDimensions.x; x++)
        {
            for(int y = chamber.Position.y; y < chamber.Position.y + chamber.RoomDimensions.y; y++)
            {
                Vector2Int chamberTilePosition = new Vector2Int(x, y);
                if (dungeonData.nodes.ContainsKey(chamberTilePosition))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void CreateNewChamber(Chamber chamber, Chamber connectedTo)
    {
        List<Vector2Int> chamberTilePositions = new();

        for (int x = chamber.Position.x; x < chamber.Position.x + chamber.RoomDimensions.x; x++)
        {
            for (int y = chamber.Position.y; y < chamber.Position.y + chamber.RoomDimensions.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                chamberTilePositions.Add(pos);
            }
        }
        if (CheckIfChamberFits(chamber))
        {
            foreach(Vector2Int v in chamberTilePositions)
            {
                dungeonData.tiles.Add(v);
                dungeonData.nodes.Add(v, new Node(v, null, 0, 0));
            }
            connectedTo?.connectedChambers.Add(chamber);
            dungeonData.chambers.Add(chamber.RelativeRoomPosition, chamber);
        }
        else
        {
            switch (Policy)
            {
                case OverlapPolicy.Ignore:
                    //Don't care, skip this room (dangerous, as it can block rooms from being accessible)
                    break;
                case OverlapPolicy.Retry:
                    foreach (Vector2Int v in chamberTilePositions)
                    {
                        if (dungeonData.nodes.ContainsKey(v))
                        {
                            //Must reconstruct the room, which is quite terrible...
                            continue;
                        }
                    }
                    //Try again, try a different position? A different size?
                    break;
                case OverlapPolicy.Merge:
                    foreach (Vector2Int v in chamberTilePositions)
                    {
                        if (!dungeonData.nodes.ContainsKey(v))
                        {
                            dungeonData.tiles.Add(v);
                            dungeonData.nodes.Add(v, new Node(v, null, 0, 0));
                        }
                        connectedTo.connectedChambers.Add(chamber);
                        dungeonData.chambers.Add(chamber.RelativeRoomPosition, chamber);
                    }
                    //Ignore overlapping tiles, but place all others. May create eldritch horror rooms.
                    break;
            }
        }
    }


    private int GetDistance(Vector2Int from, Vector2Int to)
    {
        Vector2Int direction = to - from;
        return Mathf.Abs(direction.x) + Mathf.Abs(direction.y);
    }

}
