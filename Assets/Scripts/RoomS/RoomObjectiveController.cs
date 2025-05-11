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
        if (objectiveCompleted) return;

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
            roomInstance.SetRoomTriggersActive(true, roomInstance.GetPlayerEntryDirection()); // <- EXCLUDE entry trigger
            EnableAdjacentRoomTriggers();
        }

        if (roomInstance.nodeData.template.type == RoomType.Boss)
        {
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
        return PlayerInventory.Instance != null && PlayerInventory.Instance.HasKeyForRoom(gameObject);
    }

}
