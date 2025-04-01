using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DoorwayGenerationManager : MonoBehaviour
{
    public RoomTemplate startRoomTemplate;
    public RoomTemplate[] possibleRooms;
    public RoomTemplate upgradeRoomTemplate;
    public RoomTemplate bossRoomTemplate;
    public GameObject doorPrefab;

    public float roomSize = 75f;

    private Dictionary<Vector2Int, RoomNode> roomMap = new();
    private bool bossRoomSpawned = false;
    private int totalRoomsSpawned = 0;
    private int upgradeRoomsSpawned = 0;
    private int combatRoomsSinceLastUpgrade = 0;

    private RoomInstance startRoomInstance;
    private Dictionary<Vector2Int, RoomInstance> roomPositions = new();
    public float doorYOffset = 0f;

    private void Start()
    {
        Vector2Int startPos = Vector2Int.zero;
        //Debug.Log($"Spawning start room at {startPos}");

        // Ensure the start room template is set
        if (startRoomTemplate == null)
        {
            Debug.LogError("Start room template is missing!");
            return;
        }

        // Create the start room node
        RoomNode startNode = new RoomNode(startPos, startRoomTemplate);

        // Spawn the room before adding it to the dictionary to avoid skipping spawn
        startRoomInstance = SpawnRoom(startNode, true);

        if (startRoomInstance != null)
        {
            // Only add to map if spawn was successful
            roomMap[startPos] = startNode;
            roomPositions[startPos] = startRoomInstance;

            // Set the start room and spawn doors
            startRoomInstance.SetAsStartRoom();
            startRoomInstance.SpawnDoors();
        }
    }

    public void OnPlayerEnterDoorway(Vector2Int doorwayPosition, Vector2Int direction)
    {
        //Debug.Log($"[DoorwayGenerationManager] Doorway entered. Position: {doorwayPosition}, Direction: {direction}");

        Vector2Int newRoomPos = doorwayPosition + direction;
        //Debug.Log($"Attempting to spawn room at {newRoomPos}");

        // If the room already exists, don't spawn another one
        if (roomMap.ContainsKey(newRoomPos))
        {
            Debug.Log($"Room already exists at {newRoomPos}, skipping spawn.");
            return;
        }

        // Pick a room that has a door facing the correct direction
        RoomTemplate nextRoomTemplate = PickNextRoomTemplateWithDoorFacing(direction, newRoomPos);
        if (nextRoomTemplate == null)
        {
            Debug.LogError($"[ERROR] No valid room found for direction {direction} at {newRoomPos}");
            return;
        }

        // Create and track the new room
        RoomNode newNode = new RoomNode(newRoomPos, nextRoomTemplate);
        roomMap[newRoomPos] = newNode;

        RoomInstance newRoom = SpawnRoom(newNode);
        if (newRoom == null)
        {
            Debug.LogError($"[ERROR] Failed to spawn room at {newRoomPos}");
            return;
        }

        roomPositions[newRoomPos] = newRoom; // Update roomPositions dictionary

        CreateDoorBetween(doorwayPosition, newRoom, direction);
        TrackRoomSpawn();

        // Close the door behind the player with a delay
        RoomInstance existingRoom = roomPositions[doorwayPosition];
        DoorController door = existingRoom.GetDoorAnchor(direction).GetComponentInChildren<DoorController>();
        if (door != null)
        {
            StartCoroutine(CloseDoorWithDelay(2f, door)); // Close door after 2 seconds
        }
    }

    private IEnumerator CloseDoorWithDelay(float delay, DoorController door)
    {
        yield return new WaitForSeconds(delay);
        door.ForceClose();
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
        //Debug.Log($"[GridToWorldPosition] Grid position {gridPos} -> World position {worldPos} (roomSize: {roomSize})");
        return worldPos;
    }


    private void CreateDoorBetween(Vector2Int doorwayPosition, RoomInstance newRoom, Vector2Int direction)
    {
        //Debug.Log($"Creating door between rooms with direction: {direction}");
        if (direction == Vector2Int.zero)
        {
            Debug.LogError("Attempting to create a door with an invalid direction (0,0). Check room generation logic.");
            return;
        }

        if (newRoom == null)
        {
            Debug.LogError($"Failed to create a doorway at {doorwayPosition}. Room instance is null.");
            return;
        }

        if (!roomPositions.ContainsKey(doorwayPosition))
        {
            Debug.LogError($"No existing room found at {doorwayPosition}");
            return;
        }

        RoomInstance existingRoom = roomPositions[doorwayPosition];

        Transform anchorA = existingRoom.GetDoorAnchor(direction);
        Transform anchorB = newRoom.GetDoorAnchor(-direction);

        if (anchorA == null || anchorB == null)
        {
            Debug.LogError($"Missing door anchors between rooms at {doorwayPosition} and {newRoom.nodeData.position}");
            return;
        }

        Vector3 doorPosition = (anchorA.position + anchorB.position) / 2;
        doorPosition.y += doorYOffset;

        Quaternion doorRotation = GetDoorRotation(direction);

        //Debug.Log($"[CreateDoorBetween] Instantiating door at {doorwayPosition} with direction {direction}");

        GameObject doorObj = Instantiate(doorPrefab, doorPosition, doorRotation);
        if (doorObj == null)
        {
            Debug.LogError("[CreateDoorBetween] Door instantiation FAILED!");
        }
        else
        {
            //Debug.Log($"[CreateDoorBetween] Door successfully instantiated at {doorPosition}");
        }

        DoorController door = doorObj.GetComponent<DoorController>();

        existingRoom.RegisterDoor(direction, door);
        newRoom.RegisterDoor(-direction, door);
        door.SetLocked(!existingRoom.isStartRoom && !newRoom.isStartRoom);

        //Debug.Log($"Door spawned between {doorwayPosition} and {newRoom.nodeData.position} at {doorPosition}");
    }

    private RoomInstance SpawnRoom(RoomNode node, bool isStartRoom = false)
    {
        Vector3 pos = GridToWorldPosition(node.position);

        //Debug.Log($"[SpawnRoom] Spawning room at grid {node.position}, world position: {pos}");

        RoomInstance instance = Instantiate(node.template.prefab, pos, Quaternion.identity).GetComponent<RoomInstance>();

        if (instance == null)
        {
            Debug.LogError($"[ERROR] Failed to spawn room at {node.position}");
            return null;
        }

        // Ensure RoomEnemySpawner is set before calling Initialize
        instance.enemySpawner = instance.GetComponent<RoomEnemySpawner>();

        // Initialize the room instance
        instance.nodeData = node;
        instance.Initialize(node.position, Vector2Int.zero);

        if (isStartRoom)
        {
            instance.SetAsStartRoom();
        }

        // Spawn doors for the room
        instance.SpawnDoors();

        // Disable doorway triggers until the objective is complete
        instance.SetRoomTriggersActive(false);

        return instance;
    }

    private RoomTemplate PickNextRoomTemplateWithDoorFacing(Vector2Int direction, Vector2Int newRoomPos)
    {
        List<RoomTemplate> validTemplates = new();

        foreach (RoomTemplate template in possibleRooms)
        {
            // Room must have a door facing the intended direction
            if (!DoesRoomHaveDoorFacing(template, direction))
                continue;

            // If a neighboring room exists, it must have a matching doorway
            Vector2Int oppositeDirection = -direction;
            if (!DoesRoomHaveDoorFacing(template, oppositeDirection))
                continue;

            validTemplates.Add(template);
        }

        if (validTemplates.Count == 0)
        {
            Debug.LogError($"No valid room template found with a door facing {direction} at {newRoomPos}");
            return null;
        }

        return validTemplates[Random.Range(0, validTemplates.Count)];
    }

    private bool DoesRoomHaveDoorFacing(RoomTemplate template, Vector2Int direction)
    {
        //Debug.Log($"Checking if {template.roomName} has door facing {direction}");

        bool hasDoor = direction == Vector2Int.up ? template.hasNorthDoor :
                       direction == Vector2Int.down ? template.hasSouthDoor :
                       direction == Vector2Int.right ? template.hasEastDoor :
                       direction == Vector2Int.left ? template.hasWestDoor : false;

        //Debug.Log($"Checking if {template.roomName} has door facing {direction}: {hasDoor}");
        return hasDoor;
    }

    private Quaternion GetDoorRotation(Vector2Int direction)
    {
        return direction == Vector2Int.up ? Quaternion.Euler(0, 0, 0) :
               direction == Vector2Int.down ? Quaternion.Euler(0, 180, 0) :
               direction == Vector2Int.right ? Quaternion.Euler(0, 90, 0) :
               direction == Vector2Int.left ? Quaternion.Euler(0, -90, 0) : Quaternion.identity;
    }

    private void TrackRoomSpawn()
    {
        totalRoomsSpawned++;
        combatRoomsSinceLastUpgrade++;

        Debug.Log($"Total rooms spawned: {totalRoomsSpawned}, Combat rooms since last upgrade: {combatRoomsSinceLastUpgrade}");

        if (combatRoomsSinceLastUpgrade >= 3 && upgradeRoomsSpawned < 3)
        {
            upgradeRoomsSpawned++;
            combatRoomsSinceLastUpgrade = 0;
            Debug.Log("Upgrade room condition met. Incrementing upgrade room count.");
        }
        else if (upgradeRoomsSpawned >= 3 && !bossRoomSpawned)
        {
            bossRoomSpawned = true;
            Debug.Log("Boss room condition met. Boss room will now be spawned.");
        }
    }
}