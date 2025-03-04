using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;

    private Dictionary<Vector2Int, DoorController> doors = new();
    private RoomObjectiveController objectiveController;
    private bool objectiveCompleted = false;

    public bool isStartRoom { get; private set; } = false;  // Add this property

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
}
