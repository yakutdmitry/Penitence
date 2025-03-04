using UnityEngine;
using System.Collections.Generic;

public class IncrementalGenerationManager : MonoBehaviour
{
    public RoomTemplate startRoomTemplate;
    public RoomTemplate[] possibleRooms;
    public RoomTemplate upgradeRoomTemplate;
    public RoomTemplate bossRoomTemplate;
    public GameObject doorPrefab;

    public float roomSize = 50f;
    public float doorOffset = 5f;

    private Dictionary<Vector2Int, RoomNode> roomMap = new();
    private int totalRoomsSpawned = 0;
    private int combatRoomsSinceLastUpgrade = 0;
    private int upgradeRoomsSpawned = 0;
    private bool bossRoomSpawned = false;

    private RoomInstance currentRoomInstance;
    private RoomInstance startRoomInstance;

    // Configurable guaranteed rooms (you can easily modify this to change the guaranteed number and directions)
    private List<Vector2Int> guaranteedRooms = new List<Vector2Int>
    {
        Vector2Int.up,    // North
        Vector2Int.right, // East
        Vector2Int.left   // West
    };

    private void Start()
    {
        Debug.Log("Incremental Generation Manager started");

        Vector2Int startPos = Vector2Int.zero;
        RoomNode startNode = new RoomNode(startPos, startRoomTemplate);
        roomMap[startPos] = startNode;

        startRoomInstance = SpawnRoom(startNode, true);  // mark it as the start room
        totalRoomsSpawned = 1;

        Debug.Log($"Start room spawned at {startPos}");

        // Automatically spawn guaranteed adjacent rooms
        SpawnGuaranteedRooms(startNode);

        RoomEntranceTrigger startTrigger = startRoomInstance.GetComponentInChildren<RoomEntranceTrigger>();
        if (startTrigger != null)
        {
            startTrigger.ForceTriggerEntry();
        }
    }


    private void SpawnGuaranteedRooms(RoomNode startNode)
    {
        foreach (Vector2Int direction in guaranteedRooms)
        {
            Vector2Int newRoomPosition = startNode.position + direction;

            if (roomMap.ContainsKey(newRoomPosition))
            {
                Debug.Log($"Skipping guaranteed room at {newRoomPosition}, already occupied.");
                continue;
            }

            RoomTemplate nextRoomTemplate = PickNextRoomTemplate();
            RoomNode newNode = new RoomNode(newRoomPosition, nextRoomTemplate);

            roomMap[newRoomPosition] = newNode;
            startNode.AddNeighbor(newNode);
            newNode.AddNeighbor(startNode);

            RoomInstance newRoomInstance = SpawnRoom(newNode);
            CreateDoorBetween(startRoomInstance, newRoomInstance, direction);  // FIXED HERE

            totalRoomsSpawned++;
            Debug.Log($"Guaranteed room spawned at {newRoomPosition} facing {direction}");
        }
    }


    public void OnPlayerEnterRoom(RoomInstance room)
    {
        Debug.Log("OnPlayerEnterRoom has been called");

        if (room == null)
        {
            Debug.LogError("OnPlayerEnterRoom received a null room reference!");
            return;
        }

        if (room.nodeData == null)
        {
            Debug.LogError("OnPlayerEnterRoom received a RoomInstance with null nodeData!");
            return;
        }

        Debug.Log($"OnPlayerEnterRoom processing room at {room.nodeData.position}");

        currentRoomInstance = room;

        var directions = GetRoomDirections(room.nodeData.template);
        Debug.Log($"Room at {room.nodeData.position} has {directions.Count} possible directions");

        foreach (var direction in directions)
        {
            Vector2Int newRoomPosition = room.nodeData.position + direction;
            Debug.Log($"Checking position {newRoomPosition} for new room");

            if (!roomMap.ContainsKey(newRoomPosition))
            {
                RoomTemplate nextRoomTemplate = PickNextRoomTemplate();
                Debug.Log($"Base room template selected for {newRoomPosition}");

                if (ShouldSpawnUpgradeRoom())
                {
                    nextRoomTemplate = upgradeRoomTemplate;
                    upgradeRoomsSpawned++;
                    combatRoomsSinceLastUpgrade = 0;
                    Debug.Log($"Spawning upgrade room at {newRoomPosition}");
                }
                else if (ShouldSpawnBossRoom())
                {
                    nextRoomTemplate = bossRoomTemplate;
                    bossRoomSpawned = true;
                    Debug.Log($"Spawning boss room at {newRoomPosition}");
                }
                else
                {
                    combatRoomsSinceLastUpgrade++;
                    Debug.Log($"Spawning normal room at {newRoomPosition}");
                }

                RoomNode newNode = new RoomNode(newRoomPosition, nextRoomTemplate);
                roomMap[newRoomPosition] = newNode;

                room.nodeData.AddNeighbor(newNode);
                newNode.AddNeighbor(room.nodeData);

                RoomInstance newRoomInstance = SpawnRoom(newNode);  // This ensures newNode.instance is populated
                CreateDoorBetween(room, newRoomInstance, direction);  // Use the actual RoomInstance, not the node

                totalRoomsSpawned++;
                Debug.Log($"Total rooms spawned so far: {totalRoomsSpawned}");
            }
            else
            {
                Debug.Log($"Room already exists at {newRoomPosition}, skipping.");
            }
        }

        room.PlayerEnteredRoom();  // This triggers closing doors and starting the objective check.

        Debug.Log("OnPlayerEnterRoom processing complete");
    }

    private void CreateDoorBetween(RoomInstance roomA, RoomInstance roomB, Vector2Int direction)
    {
        Vector3 doorPosition = (roomA.transform.position + roomB.transform.position) / 2f;
        doorPosition.y = doorOffset;

        Quaternion doorRotation = Quaternion.identity;
        if (direction == Vector2Int.up)
            doorRotation = Quaternion.Euler(0, 0, 0);
        else if (direction == Vector2Int.down)
            doorRotation = Quaternion.Euler(0, 180, 0);
        else if (direction == Vector2Int.right)
            doorRotation = Quaternion.Euler(0, 90, 0);
        else if (direction == Vector2Int.left)
            doorRotation = Quaternion.Euler(0, -90, 0);

        GameObject doorInstance = Instantiate(doorPrefab, doorPosition, doorRotation);

        DoorController door = doorInstance.GetComponent<DoorController>();
        roomA.RegisterDoor(direction, door);
        roomB.RegisterDoor(-direction, door);

        bool isStartRoom = roomA.isStartRoom || roomB.isStartRoom;
        door.SetLocked(!isStartRoom);
    }





    private bool ShouldSpawnUpgradeRoom()
    {
        bool result = combatRoomsSinceLastUpgrade >= 3 && upgradeRoomsSpawned < 3;
        Debug.Log($"ShouldSpawnUpgradeRoom: {result}");
        return result;
    }

    private bool ShouldSpawnBossRoom()
    {
        bool result = upgradeRoomsSpawned >= 3 && !bossRoomSpawned;
        Debug.Log($"ShouldSpawnBossRoom: {result}");
        return result;
    }

    private RoomTemplate PickNextRoomTemplate()
    {
        RoomTemplate picked = possibleRooms[Random.Range(0, possibleRooms.Length)];
        Debug.Log($"Randomly picked room template: {picked.name}");
        return picked;
    }

    private List<Vector2Int> GetRoomDirections(RoomTemplate template)
    {
        List<Vector2Int> directions = new();
        if (template.hasNorthDoor) directions.Add(Vector2Int.up);
        if (template.hasSouthDoor) directions.Add(Vector2Int.down);
        if (template.hasEastDoor) directions.Add(Vector2Int.right);
        if (template.hasWestDoor) directions.Add(Vector2Int.left);
        Debug.Log($"Template {template.name} has {directions.Count} doors");
        return directions;
    }

    private RoomInstance SpawnRoom(RoomNode node, bool isStartRoom = false)
    {
        Debug.Log($"Spawning room at grid position {node.position}");

        Vector3 position = GridToWorldPosition(node.position);
        GameObject roomInstanceObj = Instantiate(node.template.prefab, position, Quaternion.identity);
        RoomInstance instance = roomInstanceObj.GetComponent<RoomInstance>();

        instance.Initialize(node, isStartRoom);  // Pass if it's the start room

        node.instance = roomInstanceObj;

        Debug.Log($"Room successfully spawned at {node.position} (Is Start Room: {isStartRoom})");

        return instance;
    }


    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
    }
}
