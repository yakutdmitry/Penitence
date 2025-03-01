using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;

    // Doors (visual + trigger parts)
    public GameObject northDoor;
    public GameObject southDoor;
    public GameObject eastDoor;
    public GameObject westDoor;

    // Central "cut-out-able" wall sections (where doors would appear)
    public GameObject northWall;
    public GameObject southWall;
    public GameObject eastWall;
    public GameObject westWall;

    // Outer static walls - these always exist
    public GameObject outerNorthLeft;
    public GameObject outerNorthRight;
    public GameObject outerSouthLeft;
    public GameObject outerSouthRight;
    public GameObject outerWestLeft;
    public GameObject outerWestRight;
    public GameObject outerEastLeft;
    public GameObject outerEastRight;

    public void Initialize(RoomNode node)
    {
        nodeData = node;

        // Check each side and dynamically toggle doors and central wall sections.
        ToggleDoorAndWall(Vector2Int.up, northDoor, northWall, Vector2Int.down);
        ToggleDoorAndWall(Vector2Int.down, southDoor, southWall, Vector2Int.up);
        ToggleDoorAndWall(Vector2Int.right, eastDoor, eastWall, Vector2Int.left);
        ToggleDoorAndWall(Vector2Int.left, westDoor, westWall, Vector2Int.right);
    }

    private void ToggleDoorAndWall(Vector2Int direction, GameObject door, GameObject wallSection, Vector2Int oppositeDirection)
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
            if (wallSection != null) wallSection.SetActive(false); // Hide central wall where door appears
        }
        else
        {
            // No valid connection, close door and restore the wall section
            if (door != null) door.SetActive(false);
            if (wallSection != null) wallSection.SetActive(true);  // Show central wall if no door
        }
    }

    // New methods to lock/unlock all doors
    public void LockAllDoors()
    {
        SetDoorLocked(northDoor, true);
        SetDoorLocked(southDoor, true);
        SetDoorLocked(eastDoor, true);
        SetDoorLocked(westDoor, true);
    }

    public void UnlockAllDoors()
    {
        SetDoorLocked(northDoor, false);
        SetDoorLocked(southDoor, false);
        SetDoorLocked(eastDoor, false);
        SetDoorLocked(westDoor, false);
    }

    private void SetDoorLocked(GameObject door, bool locked)
    {
        if (door != null && door.activeSelf)  // Only lock/unlock visible doors
        {
            RoomDoorController doorController = door.GetComponent<RoomDoorController>();
            if (doorController != null)
            {
                doorController.SetLocked(locked);
            }
        }
    }
}
