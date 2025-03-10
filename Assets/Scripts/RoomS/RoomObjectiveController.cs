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

        // Unlock doors when objective is completed
        if (roomInstance != null)
        {
            roomInstance.UnlockAllDoors();
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
