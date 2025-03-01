using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;

    public GameObject northDoor;
    public GameObject southDoor;
    public GameObject eastDoor;
    public GameObject westDoor;

    public GameObject northWall;
    public GameObject southWall;
    public GameObject eastWall;
    public GameObject westWall;

    public void Initialize(RoomNode node)
    {
        nodeData = node;

        ToggleDoorAndWall(Vector2Int.up, northDoor, northWall, Vector2Int.down);
        ToggleDoorAndWall(Vector2Int.down, southDoor, southWall, Vector2Int.up);
        ToggleDoorAndWall(Vector2Int.right, eastDoor, eastWall, Vector2Int.left);
        ToggleDoorAndWall(Vector2Int.left, westDoor, westWall, Vector2Int.right);
    }

    private void ToggleDoorAndWall(Vector2Int direction, GameObject door, GameObject wall, Vector2Int oppositeDirection)
    {
        bool hasNeighbor = nodeData.neighbors.TryGetValue(direction, out RoomNode neighbor);
        bool roomHasDoor = nodeData.HasDoorInDirection(direction);

        bool neighborHasMatchingDoor = false;
        if (hasNeighbor && neighbor != null)
        {
            neighborHasMatchingDoor = neighbor.HasDoorInDirection(oppositeDirection);
        }

        if (hasNeighbor && roomHasDoor && neighborHasMatchingDoor)
        {
            // Open door if both rooms have matching doors
            if (door != null) door.SetActive(true);
            if (wall != null) wall.SetActive(false);
        }
        else
        {
            // No valid connection, close door
            if (door != null) door.SetActive(false);
            if (wall != null) wall.SetActive(true);
        }
    }
}
