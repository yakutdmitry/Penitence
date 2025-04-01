using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RoomPool", menuName = "Rooms/Room Pool")]
public class RoomPool : ScriptableObject
{
    public List<RoomTemplate> possibleRooms;
    public RoomType roomType;

    public RoomTemplate GetRandomRoom()
    {
        return possibleRooms[Random.Range(0, possibleRooms.Count)];
    }
}

public enum RoomType { Start, Combat, Treasure, Boss, Upgrade }