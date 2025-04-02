using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate", menuName = "Procedural/Room Template")]
public class RoomTemplate : ScriptableObject
{
    public string roomName;
    public GameObject prefab;
    public RoomType type;
    public List<Vector2Int> openDoors;

    public bool hasNorthDoor;
    public bool hasSouthDoor;
    public bool hasEastDoor;
    public bool hasWestDoor;
}
