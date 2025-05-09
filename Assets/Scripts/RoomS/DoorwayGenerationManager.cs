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

    // Instance the DoorwayGenerationManager
    public static DoorwayGenerationManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    
    private void Start()
    {
        Vector2Int startPos = Vector2Int.zero;

        if (startRoomTemplate == null)
        {
            Debug.LogError("Start room template is missing!");
            return;
        }

        RoomNode startNode = new RoomNode(startPos, startRoomTemplate);
        roomMap[startPos] = startNode;
        startRoomInstance = SpawnRoom(startNode, Vector2Int.zero, isStartRoom: true);

        if (startRoomInstance != null)
        {
            roomMap[startPos] = startNode;
            roomPositions[startPos] = startRoomInstance;

            startRoomInstance.SetAsStartRoom();
            startRoomInstance.SpawnDoors(Vector2Int.zero);

        }
    }

    public void OnPlayerEnterDoorway(Vector2Int doorwayPosition, Vector2Int direction)
    {
        Vector2Int newRoomPos = doorwayPosition + direction;

        if (roomMap.ContainsKey(newRoomPos))
        {
            Debug.Log($"Room already exists at {newRoomPos}, skipping spawn.");
            return;
        }

        RoomTemplate nextRoomTemplate = PickNextRoomTemplateWithDoorFacing(direction, newRoomPos);
        if (nextRoomTemplate == null)
        {
            Debug.LogError($"[ERROR] No valid room found for direction {direction} at {newRoomPos}");
            return;
        }

        RoomNode newNode = new RoomNode(newRoomPos, nextRoomTemplate);
        roomMap[newRoomPos] = newNode;

        RoomInstance newRoom = SpawnRoom(newNode, -direction); // player is entering FROM -direction
        newRoom.DisableEntryTrigger(-direction);

        if (newRoom == null)
        {
            Debug.LogError($"[ERROR] Failed to spawn room at {newRoomPos}");
            return;
        }

        roomPositions[newRoomPos] = newRoom;

        //CreateDoorBetween(doorwayPosition, newRoom, direction);
        TrackRoomSpawn();

        RoomInstance existingRoom = roomPositions[doorwayPosition];
        DoorController door = existingRoom.GetDoorAnchor(direction).GetComponentInChildren<DoorController>();
        if (door != null)
        {
            StartCoroutine(CloseDoorWithDelay(2f, door));
        }
    }

    public RoomInstance SpawnRoom(RoomNode node, Vector2Int entryDirection, bool isStartRoom = false)
    {
        Vector3 pos = GridToWorldPosition(node.position);
        RoomInstance instance = Instantiate(node.template.prefab, pos, Quaternion.identity).GetComponent<RoomInstance>();

        if (instance == null)
        {
            Debug.LogError($"[ERROR] Failed to spawn room at {node.position}");
            return null;
        }

        instance.enemySpawner = instance.GetComponent<RoomEnemySpawner>();
        instance.nodeData = node;
        instance.Initialize(node.position, entryDirection);

        if (isStartRoom)
        {
            instance.SetAsStartRoom();
        }

        // Pass the entry direction to avoid spawning a door in the entry doorway
        instance.SpawnDoors(entryDirection);

        if (!isStartRoom)
        {
            instance.SetRoomTriggersActive(false);
        }


        return instance;
    }

    private IEnumerator CloseDoorWithDelay(float delay, DoorController door)
    {
        yield return new WaitForSeconds(delay);
        door.ForceClose();
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
        return worldPos;
    }

    private void CreateDoorBetween(Vector2Int doorwayPosition, RoomInstance newRoom, Vector2Int direction)
    {
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

        // Check if a door already exists in this direction
        if (existingRoom.GetDoorAnchor(direction).Equals(anchorB) || newRoom.GetDoorAnchor(-direction).Equals(anchorA))
        {
            return; // Door already exists, skip creating a new one
        }

        Vector3 doorPosition = (anchorA.position + anchorB.position) / 2;
        doorPosition.y += doorYOffset;

        Quaternion doorRotation = GetDoorRotation(direction);

        GameObject doorObj = Instantiate(doorPrefab, doorPosition, doorRotation);
        if (doorObj == null)
        {
            Debug.LogError("[CreateDoorBetween] Door instantiation FAILED!");
        }

        DoorController door = doorObj.GetComponent<DoorController>();

        existingRoom.RegisterDoor(direction, door);
        newRoom.RegisterDoor(-direction, door);
        door.SetLocked(!existingRoom.isStartRoom && !newRoom.isStartRoom);
    }


    private RoomTemplate PickNextRoomTemplateWithDoorFacing(Vector2Int direction, Vector2Int newRoomPos)
    {
        List<RoomTemplate> validTemplates = new();

        foreach (RoomTemplate template in possibleRooms)
        {
            if (!DoesRoomHaveDoorFacing(template, direction))
                continue;

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
        bool hasDoor = direction == Vector2Int.up ? template.hasNorthDoor :
                       direction == Vector2Int.down ? template.hasSouthDoor :
                       direction == Vector2Int.right ? template.hasEastDoor :
                       direction == Vector2Int.left ? template.hasWestDoor : false;
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

    public bool TryGetRoomAt(Vector2Int position, out RoomInstance room)
    {
        return roomPositions.TryGetValue(position, out room);
    }


    private void OnDrawGizmos()
    {
        if (roomMap == null) return;

        Gizmos.color = Color.green;
        foreach (var room in roomMap)
        {
            Vector3 worldPos = GridToWorldPosition(room.Key);
            Gizmos.DrawWireCube(worldPos, new Vector3(roomSize, 1, roomSize));
        }
    }
}