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

    public int enemyCount = 0; // Visible in the editor for debugging

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
            Invoke(nameof(BuildNavMeshDelayed), 0.2f); // Delay NavMesh build slightly
        }

        enemySpawner?.SpawnEnemies(this);
        objectiveController = GetComponent<RoomObjectiveController>();

        if (objectiveController != null)
        {
            objectiveController.OnObjectiveCompleted += UnlockAllDoors;
        }

        if (isStartRoom)
        {
            UnlockAllDoors();
        }
    }

    private void BuildNavMeshDelayed()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void RegisterDoor(Vector2Int direction, DoorController door)
    {
        if (!doors.ContainsKey(direction))
        {
            doors[direction] = door;
            door.SetLocked(!isStartRoom);
            Debug.Log($"Registered door at {nodeData.position} facing {direction}. Locked: {!isStartRoom}");
        }
        else
        {
            Debug.LogWarning($"Door at {nodeData.position} in direction {direction} already exists!");
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
        Debug.Log("All doors unlocked!");
        objectiveCompleted = true;
        foreach (var door in doors.Values)
        {
            door.SetLocked(false);
            door.ToggleDoor(); // Ensures doors visually open
        }
    }

    public void RegisterEnemy()
    {
        enemyCount++;
        Debug.Log($"Enemy Registered in {gameObject.name}, Total Enemies: {enemyCount}");
    }

    public void EnemyDefeated()
    {
        enemyCount--;
        Debug.Log($"Enemy Defeated in {gameObject.name}, Remaining: {enemyCount}");

        if (enemyCount <= 0)
        {
            Debug.Log("Room Cleared! Checking objectives...");
            objectiveController?.CheckObjective();
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

            if (objectiveCompleted) // No need for separate method
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
