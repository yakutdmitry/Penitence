using UnityEngine;
using System.Collections.Generic;

public class ProceduralMapGenerator : MonoBehaviour
{
    public RoomTemplate startRoom;
    public RoomTemplate[] possibleRooms;
    public RoomTemplate bossRoom;

    public int maxRooms = 10;

    public int roomSize = 50;  // Distance between room centers

    private Dictionary<Vector2Int, RoomNode> roomMap = new();

    public Dictionary<Vector2Int, RoomNode> GenerateLayout()
    {
        roomMap.Clear();

        // Place the start room at (0,0) and spawn it in world space
        Vector2Int startPos = Vector2Int.zero;
        RoomNode startNode = new RoomNode(startPos, startRoom);
        roomMap[startPos] = startNode;

        SpawnRoom(startNode);

        GenerateRoomsRecursive(startNode, 1);
        PlaceBossRoom();

        return roomMap;
    }

    void GenerateRoomsRecursive(RoomNode currentRoom, int currentCount)
    {
        if (currentCount >= maxRooms) return;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        ShuffleArray(directions);

        foreach (var dir in directions)
        {
            Vector2Int newPos = currentRoom.position + dir;
            if (roomMap.ContainsKey(newPos)) continue;

            RoomTemplate nextTemplate = ChooseRandomRoomTemplate();

            // Ensure the new room matches door requirements with the current room
            if (!CanPlaceRoom(currentRoom, nextTemplate, dir))
            {
                continue; // Skip this direction if the doors don't match
            }

            RoomNode newRoom = new RoomNode(newPos, nextTemplate);

            roomMap[newPos] = newRoom;
            currentRoom.AddNeighbor(newRoom);
            newRoom.AddNeighbor(currentRoom);

            SpawnRoom(newRoom);

            GenerateRoomsRecursive(newRoom, currentCount + 1);

            if (roomMap.Count >= maxRooms) return;
        }
    }

    void PlaceBossRoom()
    {
        RoomNode furthestRoom = null;
        int maxDistance = 0;

        foreach (var room in roomMap.Values)
        {
            int dist = Mathf.Abs(room.position.x) + Mathf.Abs(room.position.y);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestRoom = room;
            }
        }

        if (furthestRoom != null)
        {
            furthestRoom.template = bossRoom;

            // Re-spawn the boss room at the correct location (overriding the old one)
            if (furthestRoom.instance != null)
            {
                Destroy(furthestRoom.instance);
            }

            SpawnRoom(furthestRoom);
        }
    }

    RoomTemplate ChooseRandomRoomTemplate()
    {
        return possibleRooms[Random.Range(0, possibleRooms.Length)];
    }

    void ShuffleArray(Vector2Int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    // This ensures world positioning is correct
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * roomSize, 0, gridPosition.y * roomSize);
    }

    //  This spawns the room and initializes it
    void SpawnRoom(RoomNode node)
    {
        Vector3 worldPos = GridToWorldPosition(node.position);
        node.instance = Instantiate(node.template.prefab, worldPos, Quaternion.identity);

        RoomInstance roomInstance = node.instance.GetComponent<RoomInstance>();
        if (roomInstance != null)
        {
            roomInstance.Initialize(node);
        }
        else
        {
            Debug.LogWarning($"RoomInstance missing on {node.template.name}");
        }
    }

    //  This ensures neighboring doors match up
    bool CanPlaceRoom(RoomNode currentRoom, RoomTemplate candidateTemplate, Vector2Int offset)
    {
        Vector2Int neighborPosition = currentRoom.position + offset;

        if (roomMap.ContainsKey(neighborPosition))
        {
            // There is already a neighbor here, check door compatibility
            RoomNode neighbor = roomMap[neighborPosition];

            bool currentRoomHasDoor = currentRoom.HasDoorInDirection(offset);
            bool neighborHasDoor = neighbor.HasDoorInDirection(-offset);

            return currentRoomHasDoor && neighborHasDoor;
        }

        // No neighbor — we can place any room
        return true;
    }
}
