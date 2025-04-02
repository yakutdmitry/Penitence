using UnityEngine;
using System;

public class RoomObjectiveController : MonoBehaviour
{
    public enum ObjectiveType { KillAllEnemies, FindKey }

    public ObjectiveType objectiveType;
    private bool objectiveCompleted = false;

    public event Action OnObjectiveCompleted;

    private RoomInstance roomInstance;

    private void Start()
    {
        roomInstance = GetComponent<RoomInstance>();
    }

    public void CheckObjective()
    {
        if (objectiveCompleted) return; // Prevents multiple triggers

        switch (objectiveType)
        {
            case ObjectiveType.KillAllEnemies:
                if (AllEnemiesDefeated())
                {
                    CompleteObjective();
                }
                break;

            case ObjectiveType.FindKey:
                if (PlayerHasKey())
                {
                    CompleteObjective();
                }
                break;
        }
    }

    private void CompleteObjective()
    {
        objectiveCompleted = true;
        OnObjectiveCompleted?.Invoke();

        if (roomInstance != null)
        {
            roomInstance.UnlockAllDoors();

            // Enable triggers in all adjacent rooms
            roomInstance.SetRoomTriggersActive(true); // Enable this room's triggers
            EnableAdjacentRoomTriggers(); // Also enable adjacent rooms

        }

        if (roomInstance.nodeData.template.type == RoomType.Boss)
        {
            Debug.Log("Boss defeated! Loading next scene...");
            FindObjectOfType<SceneManagerCustom>()?.LoadNextScene();
        }
    }

    private void EnableAdjacentRoomTriggers()
    {
        foreach (RoomInstance adjacentRoom in roomInstance.GetAdjacentRooms())
        {
            if (adjacentRoom != null)
            {
                adjacentRoom.SetRoomTriggersActive(true);
            }
        }
    }



    private bool AllEnemiesDefeated()
    {
        return roomInstance != null && roomInstance.enemyCount <= 0;
    }

    private bool PlayerHasKey()
    {
        // Example - depends on your inventory system
        return PlayerInventory.Instance.HasKeyForRoom(gameObject);
    }
}
