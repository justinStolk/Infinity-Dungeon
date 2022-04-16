using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber
{
    public List<Chamber> connectedChambers = new();
    public Vector2Int RelativeRoomPosition { get; private set; }
    //public Vector2Int Position { get; private set; }

    public Chamber(Vector2Int relativeRoomPosition)
    {
        RelativeRoomPosition = relativeRoomPosition;
        //Position = position;
    }
}
