using UnityEngine;
using System.Collections.Generic;

public class ProceduralMapGenerator : MonoBehaviour
{
    public RoomTemplate startRoom;
    public RoomTemplate[] possibleRooms;
    public RoomTemplate bossRoom;

    public int maxRooms = 10;

    // This is the new addition — set this to match your room's size (50x50 in your case)
    public int roomSize = 50;  // Distance between room centers

    private Dictionary<Vector2Int, RoomNode> roomMap = new Dictionary<Vector2Int, RoomNode>();

    public Dictionary<Vector2Int, RoomNode> GenerateLayout()
    {
        roomMap.Clear();
        Vector2Int startPos = Vector2Int.zero;
        RoomNode startNode = new RoomNode(startPos, startRoom);
        roomMap[startPos] = startNode;

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
            RoomNode newRoom = new RoomNode(newPos, nextTemplate);

            roomMap[newPos] = newRoom;
            currentRoom.AddNeighbor(newRoom);
            newRoom.AddNeighbor(currentRoom);

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

    //  Add this helper method to convert grid coordinates to world space
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * roomSize, 0, gridPosition.y * roomSize);
    }
}
