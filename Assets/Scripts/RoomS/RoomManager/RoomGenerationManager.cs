using System.Collections.Generic;
using UnityEngine;

public class RoomGenerationManager : MonoBehaviour
{
    public static RoomGenerationManager Instance;

    public RoomPool startRoomPool;
    public RoomPool combatRoomPool;
    public RoomPool upgradeRoomPool;
    public RoomPool bossRoomPool;

    public bool isStartRoom = false;

    private Dictionary<Vector2Int, RoomInstance> spawnedRooms = new();
    private int combatRoomsPlaced = 0;
    private int upgradeRoomsPlaced = 0;
    private bool bossRoomSpawned = false;

    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        Vector2Int startPos = Vector2Int.zero;
        SpawnInitialRoom(startPos);
    }

    public RoomInstance GetRoomAtPosition(Vector2Int position)
    {
        return spawnedRooms.ContainsKey(position) ? spawnedRooms[position] : null;
    }

    public void SpawnInitialRoom(Vector2Int startPos)
    {
        RoomTemplate startRoom = startRoomPool.GetRandomRoom();
        RoomInstance startInstance = Instantiate(startRoom.prefab, transform).GetComponent<RoomInstance>();
        startInstance.Initialize(startPos, Vector2Int.zero);
        startInstance.SetAsStartRoom();
        spawnedRooms[startPos] = startInstance;
    }

    public void TrySpawnAdjacentRooms(RoomInstance currentRoom)
    {
        foreach (Vector2Int direction in directions)
        {
            Vector2Int newPosition = currentRoom.position + direction;
            if (!spawnedRooms.ContainsKey(newPosition) && currentRoom.GetDoorAnchor(direction) != null)
            {
                SpawnRoom(newPosition, direction);
            }
        }
    }

    public RoomInstance SpawnStartRoom(Vector2Int position)
    {
        RoomTemplate startRoom = startRoomPool.GetRandomRoom();
        RoomInstance startInstance = Instantiate(startRoom.prefab, transform).GetComponent<RoomInstance>();
        startInstance.Initialize(position, Vector2Int.zero);
        startInstance.SetAsStartRoom();
        spawnedRooms[position] = startInstance;
        return startInstance;
    }

    public RoomInstance SpawnRoom(Vector2Int position, Vector2Int entryDirection)
    {
        RoomTemplate roomTemplate = SelectNextRoomType();

        if (roomTemplate == null)
        {
            Debug.LogError("No valid room template found! Cannot spawn room.");
            return null;
        }

        RoomInstance newRoom = Instantiate(roomTemplate.prefab, transform).GetComponent<RoomInstance>();

        if (newRoom == null)
        {
            Debug.LogError("Room instantiation failed!");
            return null;
        }

        newRoom.Initialize(position, -entryDirection);
        spawnedRooms[position] = newRoom;

        return newRoom; //Now always returns a RoomInstance or null
    }


    private RoomTemplate SelectNextRoomType()
    {
        if (bossRoomSpawned) return null;

        if (combatRoomsPlaced < 3)
        {
            combatRoomsPlaced++;
            return combatRoomPool.GetRandomRoom();
        }

        if (upgradeRoomsPlaced < 3)
        {
            upgradeRoomsPlaced++;
            return upgradeRoomPool.GetRandomRoom();
        }

        bossRoomSpawned = true;
        return bossRoomPool.GetRandomRoom();
    }

    public Vector2Int GetNextRoomPosition(Vector2Int doorDirection)
    {
        Vector2Int nextPosition = doorDirection; // Move in the given direction

        if (!spawnedRooms.ContainsKey(nextPosition))
        {
            return nextPosition;
        }

        // Fallback: Find the nearest empty space
        foreach (Vector2Int dir in directions)
        {
            Vector2Int testPosition = nextPosition + dir;
            if (!spawnedRooms.ContainsKey(testPosition))
            {
                return testPosition;
            }
        }

        Debug.LogError("No available next position found!");
        return Vector2Int.zero;
    }

}
