using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private DoorController door;

    private void Start()
    {
        door = GetComponentInParent<DoorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.OnPlayerApproach();
        }
    }
}
