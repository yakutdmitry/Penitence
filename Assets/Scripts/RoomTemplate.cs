
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate", menuName = "Procedural/Room Template")]
public class RoomTemplate : ScriptableObject
{
    public string roomName;
    public GameObject roomPrefab;
    public int maxConnections = 4;
    public RoomType type;
}

public enum RoomType
{
    Start,
    Combat,
    Treasure,
    Boss
}
