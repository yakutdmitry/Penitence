using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;

    private Dictionary<Vector2Int, DoorController> doors = new();

    public void Initialize(RoomNode node)
    {
        nodeData = node;
        Debug.Log($"RoomInstance initialized for room at {node.position}");
    }


    public void RegisterDoor(Vector2Int direction, DoorController door)
    {
        doors[direction] = door;
    }

    public void LockAllDoors()
    {
        foreach (var door in doors.Values)
        {
            door.SetLocked(true);
        }
    }

    public void UnlockAllDoors()
    {
        foreach (var door in doors.Values)
        {
            door.SetLocked(false);
        }
    }
}
