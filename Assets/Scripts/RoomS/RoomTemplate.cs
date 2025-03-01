
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate", menuName = "Procedural/Room Template")]
public class RoomTemplate : ScriptableObject
{
    public string roomName;
    public GameObject prefab;
    public int maxConnections = 4;
    public RoomType type;
    public bool hasNorthDoor;
    public bool hasSouthDoor;
    public bool hasEastDoor;
    public bool hasWestDoor;
}

public enum RoomType
{
    Start,
    Combat,
    Treasure,
    Boss
}
