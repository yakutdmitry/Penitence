using UnityEngine;

public class RoomEntranceTrigger : MonoBehaviour
{
    private IncrementalGenerationManager generationManager;

    private void Start()
    {
        RoomGenerationManager manager = FindObjectOfType<RoomGenerationManager>();
        if (manager == null)
        {
            Debug.LogError("RoomGenerationManager could not be found!"); // Update the error message
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name}");

        if (other.CompareTag("Player"))
        {
            RoomInstance room = GetComponentInParent<RoomInstance>();
            if (room == null)
            {
                Debug.LogError($"RoomEntranceTrigger could not find RoomInstance in parent!");
                return;
            }

            Debug.Log($" Player entered room at {room.nodeData?.position ?? new Vector2Int(-999, -999)}");
            generationManager?.OnPlayerEnterRoom(room);
        }
    }

    public void ForceTriggerEntry()
    {
        RoomInstance room = GetComponentInParent<RoomInstance>();
        if (generationManager != null)
        {
            generationManager.OnPlayerEnterRoom(room);
        }
    }

}
