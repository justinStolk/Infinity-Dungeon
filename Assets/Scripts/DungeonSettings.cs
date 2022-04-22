using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Dungeon Settings", menuName = "Infinity Dungeon:/Dungeon Settings")]
public class DungeonSettings : ScriptableObject
{
    [Range(0, 100)]
    public float RoomSpawnChance = 50f;
    [Min(1)]
    public int SubRoomIterations = 2;
    public int RoomWidth, RoomHeight;
    [Min(1)]
    public int MaxHorizontalRoomOffset, MaxVerticalRoomOffset = 1;
    public int RoomDistance = 3;
    //public int CorridorWidth = 3;
    [Min(0)]
    public int MaxSubChamberDetours = 2;
    public int MaxAttempts = 5;


}
