using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public Vector2Int position;
    public RoomTemplate template;
    public GameObject instance;  // Instantiated room prefab
    public Dictionary<Vector2Int, RoomNode> neighbors = new();

    public RoomNode(Vector2Int pos, RoomTemplate template)
    {
        this.position = pos;
        this.template = template;
    }

    public void AddNeighbor(RoomNode neighbor)
    {
        Vector2Int direction = neighbor.position - this.position;
        if (!neighbors.ContainsKey(direction))
        {
            neighbors[direction] = neighbor;
        }
    }

    public bool HasDoorInDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return template.hasNorthDoor;
        if (direction == Vector2Int.down) return template.hasSouthDoor;
        if (direction == Vector2Int.right) return template.hasEastDoor;
        if (direction == Vector2Int.left) return template.hasWestDoor;
        return false;
    }

    public void DebugNeighbors()
    {
        foreach (var neighbor in neighbors)
        {
            Debug.Log($"Room at {position} has neighbor at {neighbor.Value.position} in direction {neighbor.Key}");
        }
    }

}
