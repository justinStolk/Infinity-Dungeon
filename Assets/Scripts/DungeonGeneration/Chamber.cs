using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber
{
    public List<Chamber> connectedChambers = new();
    public Vector2Int RelativeRoomPosition { get; private set; }
    public Vector2Int Position { get; private set; }
    public Vector2Int RoomDimensions { get; private set; }

    public Chamber(Vector2Int roomDimensions, Vector2Int position, Vector2Int relativeRoomPosition)
    {
        RoomDimensions = roomDimensions;
        Position = position;
        RelativeRoomPosition = relativeRoomPosition;
    }

}
