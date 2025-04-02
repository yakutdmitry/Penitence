using System.Collections.Generic;
using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    private Dictionary<Vector2Int, RoomInstance> spawnedRooms = new();
    private int combatRoomsCompleted = 0;
    private int upgradeRoomsCompleted = 0;
    private RoomInstance currentRoom;

    public static RoomTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterRoom(Vector2Int position, RoomInstance room)
    {
        if (!spawnedRooms.ContainsKey(position))
        {
            spawnedRooms[position] = room;
            Debug.Log($"Registered room at {position}");
        }
    }

    public Vector2Int GetNextAvailablePosition(Vector2Int currentRoomPosition)
    {
        Vector2Int newPosition = GetEmptyAdjacentPosition(currentRoomPosition);
        if (newPosition == currentRoomPosition)
        {
            Debug.LogWarning("No available adjacent positions found. Expanding search...");
            newPosition = FindLeastConnectedRoom();
        }
        return newPosition;
    }

    private Vector2Int GetEmptyAdjacentPosition(Vector2Int currentRoomPosition)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direction in directions)
        {
            Vector2Int newPosition = currentRoomPosition + direction;
            if (!spawnedRooms.ContainsKey(newPosition))
            {
                return newPosition;
            }
        }
        return currentRoomPosition;
    }

    private Vector2Int FindLeastConnectedRoom()
    {
        foreach (var kvp in spawnedRooms)
        {
            int openNeighbors = 0;
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int dir in directions)
            {
                if (!spawnedRooms.ContainsKey(kvp.Key + dir))
                {
                    openNeighbors++;
                }
            }

            if (openNeighbors > 0)
            {
                return kvp.Key;
            }
        }

        Debug.LogError("No available spaces left for new rooms!");
        return Vector2Int.zero;
    }

    public void SetCurrentRoom(RoomInstance room)
    {
        currentRoom = room;
        Debug.Log($"Current room set to {room.position}");
    }

    public void RoomObjectiveCompleted()
    {
        combatRoomsCompleted++;
        Debug.Log($"Combat rooms completed: {combatRoomsCompleted}");
    }

    public bool ShouldSpawnUpgradeRoom()
    {
        return combatRoomsCompleted > 0 && combatRoomsCompleted % 3 == 0 && upgradeRoomsCompleted < 3;
    }

    public bool ShouldSpawnBossRoom()
    {
        return upgradeRoomsCompleted >= 3;
    }

    public void UpgradeRoomCompleted()
    {
        upgradeRoomsCompleted++;
        Debug.Log($"Upgrade rooms completed: {upgradeRoomsCompleted}");
    }

    public bool RoomExists(Vector2Int position)
    {
        return spawnedRooms.ContainsKey(position);
    }

    public RoomInstance GetRoom(Vector2Int position)
    {
        return spawnedRooms.TryGetValue(position, out RoomInstance room) ? room : null;
    }
}
