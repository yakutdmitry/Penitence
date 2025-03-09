using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;
    public float roomSize = 50f; // Add this line to define room size

    private Dictionary<Vector2Int, DoorController> doors = new();
    private RoomObjectiveController objectiveController;
    private bool objectiveCompleted = false;

    public bool isStartRoom { get; private set; } = false;  // Add this property


    public Transform northDoorAnchor;
    public Transform southDoorAnchor;
    public Transform eastDoorAnchor;
    public Transform westDoorAnchor;

    private NavMeshSurface navMeshSurface;
    private RoomEnemySpawner enemySpawner;

    public Transform GetDoorAnchor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return northDoorAnchor;
        if (direction == Vector2Int.down) return southDoorAnchor;
        if (direction == Vector2Int.right) return eastDoorAnchor;
        if (direction == Vector2Int.left) return westDoorAnchor;
        return null;
    }

    private void ValidateAnchors()
    {
        if (northDoorAnchor == null && nodeData.template.hasNorthDoor)
            Debug.LogWarning($"Room at {nodeData.position} is missing north door anchor!");

        if (southDoorAnchor == null && nodeData.template.hasSouthDoor)
            Debug.LogWarning($"Room at {nodeData.position} is missing south door anchor!");

        if (eastDoorAnchor == null && nodeData.template.hasEastDoor)
            Debug.LogWarning($"Room at {nodeData.position} is missing east door anchor!");

        if (westDoorAnchor == null && nodeData.template.hasWestDoor)
            Debug.LogWarning($"Room at {nodeData.position} is missing west door anchor!");
    }


    public void Initialize(RoomNode node, bool isStart = false)
    {
        nodeData = node;
        isStartRoom = isStart;

        Debug.Log($"RoomInstance initialized for room at {node.position}, Is Start Room: {isStartRoom}");

        if (isStartRoom)
        {
            UnlockAllDoors();  // Start room doors should be open
            OpenAllDoors();    // Optional — auto-slide doors open
        }
    }



    public void OpenAllDoors()
    {
        foreach (var door in doors.Values)
        {
            door.SetLocked(false);
            door.ToggleDoor();  // Directly opens them
        }
    }


    private void Start()
    {
        navMeshSurface = GetComponentInChildren<NavMeshSurface>();
        enemySpawner = GetComponent<RoomEnemySpawner>();

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();  // Builds nav mesh after room is created
        }

        enemySpawner?.SpawnEnemies(this);  // Spawns enemies in the room
        objectiveController = GetComponent<RoomObjectiveController>();
        if (objectiveController != null)
        {
            objectiveController.OnObjectiveCompleted += UnlockAllDoors;
        }

        if (isStartRoom)
        {
            UnlockAllDoors();  // Start room doors are unlocked right away.
        }
    }

    public void RegisterDoor(Vector2Int direction, DoorController door)
    {
        if (!doors.ContainsKey(direction))
        {
            doors[direction] = door;
            door.SetLocked(!isStartRoom); // Start room doors are unlocked immediately.
        }
    }

    public void CloseAllDoors()
    {
        foreach (var door in doors.Values)
        {
            door.SetLocked(true);
            door.ForceClose();  // Direct close on player entry.
        }
    }

    public void UnlockAllDoors()
    {
        objectiveCompleted = true;
        foreach (var door in doors.Values)
        {
            door.SetLocked(false);
        }
    }

    public void PlayerEnteredRoom()
    {
        CloseAllDoors();

        if (objectiveController != null && !objectiveCompleted)
        {
            InvokeRepeating(nameof(CheckObjective), 1f, 1f);
        }
    }


    private void CheckObjective()
    {
        if (objectiveController != null)
        {
            objectiveController.CheckObjective();

            if (objectiveControllerHasCompleted())
            {
                CancelInvoke(nameof(CheckObjective));
            }
        }
    }

    private bool objectiveControllerHasCompleted()
    {
        return objectiveCompleted;  // Now tracks directly here.
    }

    public void ClearRoom()
    {
        enemySpawner?.ClearEnemies();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (northDoorAnchor != null) Gizmos.DrawSphere(northDoorAnchor.position, 0.5f);
        if (southDoorAnchor != null) Gizmos.DrawSphere(southDoorAnchor.position, 0.5f);
        if (eastDoorAnchor != null) Gizmos.DrawSphere(eastDoorAnchor.position, 0.5f);
        if (westDoorAnchor != null) Gizmos.DrawSphere(westDoorAnchor.position, 0.5f);
    }

}
