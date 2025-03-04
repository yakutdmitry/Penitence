using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 7;

    [SerializeField] private int roomWidth = 50;
    [SerializeField] private int roomLength = 50;

    [SerializeField] private int gridSizeX = 50;
    [SerializeField] private int gridSizeZ = 50;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;


    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeZ];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeZ / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridZ = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX + 1, gridZ));
            TryGenerateRoom(new Vector2Int(gridX - 1, gridZ));
            TryGenerateRoom(new Vector2Int(gridX, gridZ + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridZ - 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("Generation failed, regenerating rooms.");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            generationComplete = true;
            Debug.Log("Generation complete, {roomCount} rooms created.");
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex) 
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int z = roomIndex.y;
        roomGrid[x, z] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room_{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);

    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int z = roomIndex.y;

        if (roomCount >= maxRooms)
        {
            return false;
        }
        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            return false;
        }
        if (CountAdjacentRooms(roomIndex) >1)
        {
            return false;
        }

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, z] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room_{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, z);

        return true;
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(room => Destroy(room));
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeZ];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeZ / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void OpenDoors(GameObject room, int x, int z)
    {
        Room newRoomScript = room.GetComponent<Room>();
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, z));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, z));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, z + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, z - 1));
        if (x > 0 && roomGrid[x - 1, z] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, z] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (z > 0 && roomGrid[x, z - 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
        if (z < gridSizeZ - 1 && roomGrid[x, z + 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    Room GetRoomScriptAt(Vector2Int index) 
    {
        GameObject roomObject = roomObjects.Find(room => room.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }


    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int z = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, z] != 0) count++; // Left Neighbor
        if (x < gridSizeX - 1 && roomGrid[x + 1, z] != 0) count++; // Right Neighbor
        if (z > 0 && roomGrid[x, z - 1] != 0) count++; // Bottom Neighbor
        if (z < gridSizeZ - 1 && roomGrid[x, z + 1] != 0) count++; // Top Neighbor

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridZ = gridIndex.y;

        return new Vector3(
            roomWidth * (gridX - gridSizeX / 2), // X stays the same (left/right)
            0,                                   // Y stays at 0 unless you have multiple floors
            roomLength * (gridZ - gridSizeZ / 2) // Z handles depth (forward/back)
        );
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, z));

                // Fix: Use Z for depth (forward/back) instead of Y
                Gizmos.DrawWireCube(new Vector3(position.x, 0, position.z), new Vector3(roomWidth, 1, roomLength));
            }
        }
    }

}

