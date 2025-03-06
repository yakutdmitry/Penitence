using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private DoorController doorController;

    private void Start()
    {
        doorController = GetComponentInParent<DoorController>();
        if (doorController == null)
        {
            Debug.LogError("DoorTrigger could not find parent DoorController!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorController?.TryOpen();
        }
    }
}
