using UnityEngine;
using System.Collections.Generic;

public class IncrementalGenerationManager : MonoBehaviour
{
    public RoomTemplate startRoomTemplate;
    public RoomTemplate[] possibleRooms;
    public RoomTemplate upgradeRoomTemplate;
    public RoomTemplate bossRoomTemplate;

    public float roomSize = 50f;

    private Dictionary<Vector2Int, RoomNode> roomMap = new();
    private int totalRoomsSpawned = 0;
    private int combatRoomsSinceLastUpgrade = 0;
    private int upgradeRoomsSpawned = 0;
    private bool bossRoomSpawned = false;

    private RoomInstance currentRoomInstance;

    private void Start()
    {
        // Initial start room at 0,0
        Vector2Int startPos = Vector2Int.zero;
        RoomNode startNode = new RoomNode(startPos, startRoomTemplate);
        roomMap[startPos] = startNode;

        SpawnRoom(startNode);
        totalRoomsSpawned = 1;
    }

    public void OnPlayerEnterRoom(RoomInstance room)
    {
        Debug.Log($"Spawning neighbors for room at {room.nodeData.position}");

        currentRoomInstance = room;

        // Spawn new rooms if needed at each valid exit
        foreach (var direction in GetRoomDirections(room.nodeData.template))
        {
            Vector2Int newRoomPosition = room.nodeData.position + direction;

            if (!roomMap.ContainsKey(newRoomPosition))
            {
                RoomTemplate nextRoomTemplate = PickNextRoomTemplate();

                if (ShouldSpawnUpgradeRoom())
                {
                    nextRoomTemplate = upgradeRoomTemplate;
                    upgradeRoomsSpawned++;
                    combatRoomsSinceLastUpgrade = 0;
                    Debug.Log("Upgrade Room Spawned!");
                }
                else if (ShouldSpawnBossRoom())
                {
                    nextRoomTemplate = bossRoomTemplate;
                    bossRoomSpawned = true;
                    Debug.Log("Boss Room Spawned!");
                }
                else
                {
                    combatRoomsSinceLastUpgrade++;
                }

                RoomNode newNode = new RoomNode(newRoomPosition, nextRoomTemplate);
                roomMap[newRoomPosition] = newNode;

                room.nodeData.AddNeighbor(newNode);
                newNode.AddNeighbor(room.nodeData);

                SpawnRoom(newNode);

                totalRoomsSpawned++;
            }
        }
    }

    private bool ShouldSpawnUpgradeRoom()
    {
        return combatRoomsSinceLastUpgrade >= 3 && upgradeRoomsSpawned < 3;
    }

    private bool ShouldSpawnBossRoom()
    {
        return upgradeRoomsSpawned >= 3 && !bossRoomSpawned;
    }

    private void SpawnRoom(RoomNode node)
    {
        Vector3 worldPos = new Vector3(node.position.x * roomSize, 0, node.position.y * roomSize);
        GameObject roomInstance = Instantiate(node.template.prefab, worldPos, Quaternion.identity);

        RoomInstance roomInstanceComponent = roomInstance.GetComponent<RoomInstance>();
        if (roomInstanceComponent != null)
        {
            roomInstanceComponent.Initialize(node);

            // Auto-link generationManager to all entrance triggers in the room
            RoomEntranceTrigger[] triggers = roomInstance.GetComponentsInChildren<RoomEntranceTrigger>();
            foreach (var trigger in triggers)
            {
                trigger.SetGenerationManager(this);  // New method to set the manager directly
            }
        }
        else
        {
            Debug.LogWarning($"RoomInstance missing on {node.template.name}");
        }

        node.instance = roomInstance;
    }


    private RoomTemplate PickNextRoomTemplate()
    {
        return possibleRooms[Random.Range(0, possibleRooms.Length)];
    }

    private List<Vector2Int> GetRoomDirections(RoomTemplate template)
    {
        List<Vector2Int> directions = new List<Vector2Int>();

        if (template.hasNorthDoor) directions.Add(Vector2Int.up);
        if (template.hasSouthDoor) directions.Add(Vector2Int.down);
        if (template.hasEastDoor) directions.Add(Vector2Int.right);
        if (template.hasWestDoor) directions.Add(Vector2Int.left);

        return directions;
    }

    public RoomNode GetRoomAtPosition(Vector2Int position)
    {
        roomMap.TryGetValue(position, out RoomNode node);
        return node;
    }
}
