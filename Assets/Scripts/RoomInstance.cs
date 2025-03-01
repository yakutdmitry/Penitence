using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;

    // Assign these in the inspector for each room prefab
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

        // Check each side for neighboring rooms and toggle doors and walls appropriately
        ToggleDoorAndWall(Vector2Int.up, northDoor, northWall);
        ToggleDoorAndWall(Vector2Int.down, southDoor, southWall);
        ToggleDoorAndWall(Vector2Int.right, eastDoor, eastWall);
        ToggleDoorAndWall(Vector2Int.left, westDoor, westWall);
    }

    private void ToggleDoorAndWall(Vector2Int direction, GameObject door, GameObject wall)
    {
        if (nodeData.Neighbors.ContainsKey(direction))  // Use Neighbors instead of neighbors
        {
            if (door != null) door.SetActive(true);
            if (wall != null) wall.SetActive(false);
        }
        else
        {
            if (door != null) door.SetActive(false);
            if (wall != null) wall.SetActive(true);
        }
    }

}
