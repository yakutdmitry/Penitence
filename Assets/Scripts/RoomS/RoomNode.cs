using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public Vector2Int position;
    public RoomTemplate template;
    public GameObject instance;
    public Dictionary<Vector2Int, RoomNode> neighbors = new();

    public RoomNode(Vector2Int pos, RoomTemplate template)
    {
        position = pos;
        this.template = template;
    }

    public void AddNeighbor(RoomNode neighbor)
    {
        Vector2Int direction = neighbor.position - position;
        neighbors[direction] = neighbor;
    }
}
