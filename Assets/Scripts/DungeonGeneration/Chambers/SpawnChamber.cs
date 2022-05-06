using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChamber : Chamber
{
    public Vector2Int SpawnerPosition { get; private set; }
    public SpawnChamber(Vector2Int roomDimensions, Vector2Int position, Vector2Int relativeRoomPosition) : base(roomDimensions, position, relativeRoomPosition)
    {
        Vector2Int spawnInRoomPosition = new Vector2Int(Random.Range(1, roomDimensions.x - 1), Random.Range(1, roomDimensions.y - 1));
        SpawnerPosition = spawnInRoomPosition + position;
    }



}
