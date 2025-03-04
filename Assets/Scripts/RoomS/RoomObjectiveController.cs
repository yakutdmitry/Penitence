using UnityEngine;
using System;
using static UnityEngine.EventSystems.EventTrigger;

public class RoomObjectiveController : MonoBehaviour
{
    public enum ObjectiveType { KillAllEnemies, FindKey }

    public ObjectiveType objectiveType;
    private bool objectiveCompleted = false;

    public event Action OnObjectiveCompleted;

    public void CheckObjective()
    {
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
        if (objectiveCompleted) return;
        objectiveCompleted = true;
        OnObjectiveCompleted?.Invoke();
    }

    private bool AllEnemiesDefeated()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>(); // Example - assumes enemies are children of room
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive()) return false;
        }
        return true;
    }

    private bool PlayerHasKey()
    {
        // Example - depends on your inventory system
        return PlayerInventory.Instance.HasKeyForRoom(gameObject);
    }
}
