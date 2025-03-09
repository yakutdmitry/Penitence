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

    private Dictionary<Vector2Int, RoomNode> roomMap = new();
    private int totalRoomsSpawned = 0;
    private int combatRoomsSinceLastUpgrade = 0;
    private int upgradeRoomsSpawned = 0;
    private bool bossRoomSpawned = false;

    private RoomInstance currentRoomInstance;
    private RoomInstance startRoomInstance;

    public bool showDebugGizmos = true;  // Toggle in Inspector
    public float debugLabelOffsetY = 3f;  // Controls label height above room center

    public float doorYOffset = 0f;  // Inspector tweak for height adjustment

    private List<Vector2Int> guaranteedRooms = new() { Vector2Int.up, Vector2Int.right, Vector2Int.left };

    private void Start()
    {
        Vector2Int startPos = Vector2Int.zero;
        RoomNode startNode = new RoomNode(startPos, startRoomTemplate);
        roomMap[startPos] = startNode;

        startRoomInstance = SpawnRoom(startNode, true);
        SpawnGuaranteedRooms(startNode);

        RoomEntranceTrigger trigger = startRoomInstance.GetComponentInChildren<RoomEntranceTrigger>();
        trigger?.ForceTriggerEntry();
    }

    private void SpawnGuaranteedRooms(RoomNode startNode)
    {
        foreach (var direction in guaranteedRooms)
        {
            Vector2Int newPos = startNode.position + direction;
            if (roomMap.ContainsKey(newPos)) continue;

            RoomTemplate nextRoomTemplate = PickNextRoomTemplateWithDoorFacing(direction);
            RoomNode newNode = new RoomNode(newPos, nextRoomTemplate);
            roomMap[newPos] = newNode;

            startNode.AddNeighbor(newNode);
            newNode.AddNeighbor(startNode);

            RoomInstance newRoom = SpawnRoom(newNode);
            CreateDoorBetween(startRoomInstance, newRoom, direction);

            TrackRoomSpawn();  // Track guaranteed rooms too
        }
    }

    public void OnPlayerEnterRoom(RoomInstance room)
    {
        currentRoomInstance = room;

        foreach (var direction in GetRoomDirections(room.nodeData.template))
        {
            Vector2Int newPos = room.nodeData.position + direction;

            if (!roomMap.ContainsKey(newPos))
            {
                RoomTemplate nextRoomTemplate = PickNextRoomTemplateWithDoorFacing(direction);
                RoomNode newNode = new RoomNode(newPos, nextRoomTemplate);
                roomMap[newPos] = newNode;

                room.nodeData.AddNeighbor(newNode);
                newNode.AddNeighbor(room.nodeData);

                RoomInstance newRoom = SpawnRoom(newNode);
                CreateDoorBetween(room, newRoom, direction);

                TrackRoomSpawn();
            }
        }

        room.PlayerEnteredRoom();
    }

    private void CreateDoorBetween(RoomInstance roomA, RoomInstance roomB, Vector2Int direction)
    {
        Transform anchorA = roomA.GetDoorAnchor(direction);
        Transform anchorB = roomB.GetDoorAnchor(-direction);

        if (anchorA == null || anchorB == null)
        {
            Debug.LogError($"Missing door anchors between rooms at {roomA.nodeData.position} and {roomB.nodeData.position}");
            return;
        }

        Vector3 doorPosition = (anchorA.position + anchorB.position) / 2;
        doorPosition.y += doorYOffset;  // Fine-tuning height placement from Inspector

        Quaternion doorRotation = GetDoorRotation(direction);

        GameObject doorObj = Instantiate(doorPrefab, doorPosition, doorRotation);
        DoorController door = doorObj.GetComponent<DoorController>();

        roomA.RegisterDoor(direction, door);
        roomB.RegisterDoor(-direction, door);

        door.SetLocked(!roomA.isStartRoom && !roomB.isStartRoom);
    }

    private RoomInstance SpawnRoom(RoomNode node, bool isStartRoom = false)
    {
        Vector3 pos = GridToWorldPosition(node.position);
        RoomInstance instance = Instantiate(node.template.prefab, pos, Quaternion.identity).GetComponent<RoomInstance>();

        instance.Initialize(node, isStartRoom);
        instance.roomSize = roomSize; // Pass the roomSize from IncrementalGenerationManager

        node.instance = instance.gameObject;
        return instance;
    }


    private RoomTemplate PickNextRoomTemplateWithDoorFacing(Vector2Int direction)
    {
        List<RoomTemplate> validTemplates = new();

        foreach (RoomTemplate template in possibleRooms)
        {
            if (DoesRoomHaveDoorFacing(template, direction))
                validTemplates.Add(template);
        }

        if (validTemplates.Count == 0)
        {
            Debug.LogError($"No valid room template found with a door facing {direction}");
            return possibleRooms[Random.Range(0, possibleRooms.Length)];
        }

        return validTemplates[Random.Range(0, validTemplates.Count)];
    }

    private bool DoesRoomHaveDoorFacing(RoomTemplate template, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return template.hasSouthDoor;
        if (direction == Vector2Int.down) return template.hasNorthDoor;
        if (direction == Vector2Int.right) return template.hasWestDoor;
        if (direction == Vector2Int.left) return template.hasEastDoor;
        return false;
    }

    private List<Vector2Int> GetRoomDirections(RoomTemplate template)
    {
        List<Vector2Int> directions = new();
        if (template.hasNorthDoor) directions.Add(Vector2Int.up);
        if (template.hasSouthDoor) directions.Add(Vector2Int.down);
        if (template.hasEastDoor) directions.Add(Vector2Int.right);
        if (template.hasWestDoor) directions.Add(Vector2Int.left);
        return directions;
    }

    private Quaternion GetDoorRotation(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return Quaternion.Euler(0, 0, 0);        // North
        if (direction == Vector2Int.down) return Quaternion.Euler(0, 180, 0);     // South
        if (direction == Vector2Int.right) return Quaternion.Euler(0, 90, 0);     // East
        if (direction == Vector2Int.left) return Quaternion.Euler(0, -90, 0);     // West
        return Quaternion.identity;
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
    }

    //  New Tracking + Upgrade/Boss Logic

    private void TrackRoomSpawn()
    {
        totalRoomsSpawned++;
        combatRoomsSinceLastUpgrade++;

        CheckUpgradeRoomConditions();
    }

    private void CheckUpgradeRoomConditions()
    {
        if (combatRoomsSinceLastUpgrade >= 3 && upgradeRoomsSpawned < 3)
        {
            upgradeRoomsSpawned++;
            combatRoomsSinceLastUpgrade = 0;
            Debug.Log(" Upgrade room logic would trigger here!");
        }
        else if (upgradeRoomsSpawned >= 3 && !bossRoomSpawned)
        {
            bossRoomSpawned = true;
            Debug.Log(" Boss room logic would trigger here!");
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        foreach (var roomEntry in roomMap)
        {
            Vector2Int gridPos = roomEntry.Key;
            RoomNode node = roomEntry.Value;
            RoomTemplate template = node.template;

            Color color = Color.blue;  // Default to combat room

            if (template == startRoomTemplate)
            {
                color = Color.green;  // Start room
            }
            else if (template == upgradeRoomTemplate)
            {
                color = Color.yellow;  // Upgrade room
            }
            else if (template == bossRoomTemplate)
            {
                color = Color.red;  // Boss room
            }

            DrawRoomGizmo(gridPos, color, template.roomName);
        }
    }

    private void DrawRoomGizmo(Vector2Int gridPos, Color color, string roomName)
    {
        Vector3 worldPos = GridToWorldPosition(gridPos) + Vector3.up * 0.5f;
        Vector3 size = new Vector3(roomSize, 1, roomSize);

        Gizmos.color = color;
        Gizmos.DrawWireCube(worldPos, size);

        // Draw Label
        if (showDebugGizmos)
        {
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.Label(worldPos + Vector3.up * debugLabelOffsetY, $"{roomName}\n({gridPos.x}, {gridPos.y})");
        }
    }
}
