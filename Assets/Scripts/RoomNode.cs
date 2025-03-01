using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public Vector2Int position;
    public RoomTemplate template;
    public GameObject instance;

    // Expose read-only access to neighbors
    private Dictionary<Vector2Int, RoomNode> neighbors = new Dictionary<Vector2Int, RoomNode>();
    public IReadOnlyDictionary<Vector2Int, RoomNode> Neighbors => neighbors;

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

    public void OpenDoors()
    {
        foreach (var kvp in neighbors)
        {
            Vector2Int direction = kvp.Key;
            OpenDoor(direction);
        }
    }

    private void OpenDoor(Vector2Int direction)
    {
        if (instance == null) return;

        RoomDoorController doorController = instance.GetComponent<RoomDoorController>();
        if (doorController == null) return;

        if (direction == Vector2Int.up)
        {
            doorController.OpenNorthDoor();
        }
        else if (direction == Vector2Int.down)
        {
            doorController.OpenSouthDoor();
        }
        else if (direction == Vector2Int.right)
        {
            doorController.OpenEastDoor();
        }
        else if (direction == Vector2Int.left)
        {
            doorController.OpenWestDoor();
        }
    }
}
