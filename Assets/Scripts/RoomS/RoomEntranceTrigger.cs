using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntranceTrigger : MonoBehaviour
{
    private IncrementalGenerationManager generationManager;

    public void SetGenerationManager(IncrementalGenerationManager manager)
    {
        generationManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggered room entrance!");

            RoomInstance roomInstance = GetComponentInParent<RoomInstance>();
            if (roomInstance != null)
            {
                generationManager?.OnPlayerEnterRoom(roomInstance);
            }
            else
            {
                Debug.LogError("RoomInstance not found on parent!");
            }
        }
    }
}



