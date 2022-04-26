using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floorPrefab;

    [Header("Settings")]
    public float RoomSpawnChance = 50f;
    public int RoomWidth, RoomHeight;
    public int MaxHorizontalRoomCount, MaxVerticalRoomCount;
    public int RoomDistance = 3;
    public int CorridorWidth = 3;
    [Range(0, 5)]
    public int MaxSubChamberDetours = 2;

    private Dictionary<Vector2Int, Node> nodes = new();
    private Dictionary<Vector2Int, Chamber> chambers = new();
    private List<Chamber> unevaluatedChambers = new();
    private List<Chamber> evaluatedChambers = new();

    // Start is called before the first frame update
    void Start()
    {
        //GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        CreateCoreRooms();
        CreateRooms();
        BuildChamberTiles();
        CreateCorridors();
        BuildTiles();
    }


    private void CreateCoreRooms()
    {
        //Chamber entryChamber = new Chamber(new(0, 0));
        //chambers.Add(entryChamber.RelativeRoomPosition, entryChamber);
        //Chamber centralChamber = new Chamber(new(0, 2));
        //chambers.Add(centralChamber.RelativeRoomPosition, centralChamber);
       // Chamber exitChamber = new Chamber(new(0, MaxVerticalRoomCount));     
       // chambers.Add(exitChamber.RelativeRoomPosition, exitChamber);
        //unevaluatedChambers.Add(entryChamber);
       //unevaluatedChambers.Add(centralChamber);
       // unevaluatedChambers.Add(exitChamber);
    }

    private void CreateRooms()
    {
        //Chamber currentChamber = chambers[new Vector2Int(0, 0)];
        while(unevaluatedChambers.Count > 0)
        {
            Chamber current = unevaluatedChambers[0];
            List<Chamber> neighbours = CreatedNeighbouringChambers(current);
            foreach (Chamber neighbour in neighbours)
            {
                if (!chambers.ContainsKey(neighbour.RelativeRoomPosition))
                {
                    unevaluatedChambers.Add(neighbour);
                    current.connectedChambers.Add(neighbour);
                    chambers.Add(neighbour.RelativeRoomPosition, neighbour);
                }
            }
            unevaluatedChambers.Remove(current);
        }
    }

    private void BuildChamberTiles()
    {
        foreach(KeyValuePair<Vector2Int, Chamber> pair in chambers)
        {
            for(int x = 0; x < RoomWidth; x++)
            {
                for(int y = 0; y < RoomHeight; y++)
                {
                    Vector2Int tilePosition = new Vector2Int((pair.Value.RelativeRoomPosition.x * (RoomWidth + RoomDistance)) + x, (pair.Value.RelativeRoomPosition.y * (RoomHeight + RoomDistance)) + y);
                    nodes.Add(tilePosition, new Node(tilePosition, null, 0, 0));

                }
            }
        }
    }

    private List<Chamber> CreatedNeighbouringChambers(Chamber startChamber)
    {
        List<Chamber> result = new();
        Vector2Int relativeStart = startChamber.RelativeRoomPosition;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Debug.Log("New relative position" + (relativeStart.x + x) + "," + y);
                if (y + relativeStart.y < 0 || y + relativeStart.y > MaxVerticalRoomCount || x + relativeStart.x < -MaxHorizontalRoomCount * 0.5f || x + relativeStart.x > MaxHorizontalRoomCount * 0.5f)
                {
                    continue;
                }
                if (Random.Range(0, 100) <= RoomSpawnChance && !chambers.ContainsKey(new Vector2Int(relativeStart.x + x, relativeStart.y + y)))
                {
                    //Chamber newNeighbour = new Chamber(new Vector2Int(startChamber.RelativeRoomPosition.x + x, startChamber.RelativeRoomPosition.y + y));
                   // result.Add(newNeighbour);
                    //Debug.Log("Created new Chamber at Relative Pos: " + newNeighbour.RelativeRoomPosition);
                }
            }
        }
        return result;
    }
    private void CreateCorridors()
    {
        foreach(KeyValuePair<Vector2Int, Chamber> pair in chambers)
        {
            Chamber main = pair.Value;
            foreach (Chamber connectedChamber in main.connectedChambers)
            {
                for(int i = 0; i < RoomDistance; i++)
                {
                    if (Mathf.Abs(connectedChamber.RelativeRoomPosition.x) > main.RelativeRoomPosition.x)
                    {
                        Vector2Int nodePos = new Vector2Int(main.RelativeRoomPosition.x * RoomWidth + RoomWidth + i, main.RelativeRoomPosition.y * RoomHeight + main.RelativeRoomPosition.y * RoomDistance);
                        if (!nodes.ContainsKey(nodePos))
                        {
                            nodes.Add(nodePos, new Node(nodePos, null, 0, 0));
                        }
                    }
                    if (Mathf.Abs(connectedChamber.RelativeRoomPosition.x) < main.RelativeRoomPosition.x)
                    {
                        Vector2Int nodePos = new Vector2Int(main.RelativeRoomPosition.x * RoomWidth - i, main.RelativeRoomPosition.y * RoomHeight + main.RelativeRoomPosition.y * RoomDistance);
                        if (!nodes.ContainsKey(nodePos))
                        {
                            nodes.Add(nodePos, new Node(nodePos, null, 0, 0));
                        }
                    }
                    if (Mathf.Abs(connectedChamber.RelativeRoomPosition.y) > main.RelativeRoomPosition.y)
                    {
                        Vector2Int nodePos = new Vector2Int(main.RelativeRoomPosition.x * RoomWidth + main.RelativeRoomPosition.x * RoomDistance, main.RelativeRoomPosition.y * RoomHeight + RoomHeight + i);
                        if (!nodes.ContainsKey(nodePos))
                        {
                            nodes.Add(nodePos, new Node(nodePos, null, 0, 0));
                        }
                    }
                    if (Mathf.Abs(connectedChamber.RelativeRoomPosition.y) < main.RelativeRoomPosition.y)
                    {
                        Vector2Int nodePos = new Vector2Int(main.RelativeRoomPosition.x * RoomWidth + main.RelativeRoomPosition.x * RoomDistance, main.RelativeRoomPosition.y * RoomHeight + RoomHeight - i);
                        if (!nodes.ContainsKey(nodePos))
                        {
                            nodes.Add(nodePos, new Node(nodePos, null, 0, 0));
                        }
                    }
                }
                //for (int i = 0; i < CorridorWidth; i++)
                //{


                //}
            }
        }
    }
    private bool PathToEndExists()
    {
        List<Chamber> unevaluatedChambers = new();
        List<Chamber> evaluatedChambers = new();
        Chamber start = chambers[new Vector2Int(0, 0)];
        Chamber end = chambers[new Vector2Int(0, MaxVerticalRoomCount)];
        unevaluatedChambers.Add(start);
        while (unevaluatedChambers.Count > 0)
        {
            Chamber current = unevaluatedChambers[0];
            foreach(Chamber neighbour in current.connectedChambers)
            {
                if(neighbour == end)
                {
                    return true;
                }
                if (!evaluatedChambers.Contains(neighbour))
                {
                    unevaluatedChambers.Add(neighbour);
                }
            }
            evaluatedChambers.Add(current);
            unevaluatedChambers.Remove(current);
        }
        return false;

    }
    private void BuildTiles()
    {
        foreach(KeyValuePair<Vector2Int, Node> pair in nodes)
        {
            Instantiate(floorPrefab, new Vector3(pair.Key.x, pair.Key.y, 0), Quaternion.identity);
        }
    }

}


