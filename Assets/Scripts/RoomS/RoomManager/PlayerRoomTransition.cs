//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerRoomTransition : MonoBehaviour
//{
//    private RoomGenerationManager roomManager;
//    private Vector2Int currentRoomPosition = Vector2Int.zero;
//    private RoomInstance currentRoom;

//    private HashSet<Vector2Int> spawnedRooms = new HashSet<Vector2Int>();

//    private bool hasSpawnedRoom = false;

//    private void Start()
//    {
//        Debug.Log("PlayerRoomTransition initialized");
//        roomManager = FindObjectOfType<RoomGenerationManager>();
//        currentRoom = roomManager.GetRoomAtPosition(currentRoomPosition);
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;
//        Debug.Log("Player entered room trigger");

//        if (hasSpawnedRoom) return;

//        if (currentRoom == null)
//        {
//            Debug.LogError("currentRoom is NULL! Fetching again...");
//            currentRoom = roomManager.GetRoomAtPosition(currentRoomPosition);
//            if (currentRoom == null)
//            {
//                Debug.LogError("Still NULL! Room tracking is broken.");
//                return;
//            }
//        }

//        Vector2Int doorDirection = GetDoorDirection(transform);
//        Vector2Int nextRoomPosition = roomManager.GetNextRoomPosition(doorDirection);

//        if (spawnedRooms.Contains(nextRoomPosition)) return;

//        RoomInstance nextRoom = roomManager.GetRoomAtPosition(nextRoomPosition) ??
//                                roomManager.SpawnRoom(nextRoomPosition, doorDirection);

//        if (nextRoom != null)
//        {
//            spawnedRooms.Add(nextRoomPosition);
//            currentRoom = nextRoom;
//            currentRoomPosition = nextRoomPosition;
//            hasSpawnedRoom = true;
//            Debug.Log($"Moved to room at {currentRoomPosition}");
//        }
//        else
//        {
//            Debug.LogError("Failed to spawn room!");
//        }
//    }




//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            //hasSpawnedRoom = false;  // Reset flag when player leaves
//        }
//    }

//    private Vector2Int GetDoorDirection(Transform doorTransform)
//    {
//        Vector3 direction = doorTransform.forward;
//        Debug.Log($"Door forward vector: {direction}");

//        if (Vector3.Dot(direction, Vector3.forward) > 0.9f) return Vector2Int.up;
//        if (Vector3.Dot(direction, Vector3.back) > 0.9f) return Vector2Int.down;
//        if (Vector3.Dot(direction, Vector3.right) > 0.9f) return Vector2Int.right;
//        if (Vector3.Dot(direction, Vector3.left) > 0.9f) return Vector2Int.left;

//        Debug.LogError("Could not determine door direction!");
//        return Vector2Int.zero;
//    }


//}
