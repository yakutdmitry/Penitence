using UnityEngine;

public class DoorwayTrigger : MonoBehaviour
{
    public Vector2Int doorDirection;
    private DoorwayGenerationManager generationManager;

    private void Start()
    {
        generationManager = FindObjectOfType<DoorwayGenerationManager>();
        //Debug.Log($"[DoorwayTrigger] Assigned doorDirection: {doorDirection}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector2Int doorwayPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / generationManager.roomSize),
            Mathf.RoundToInt(transform.position.z / generationManager.roomSize)
        );

        //Debug.Log($"[DoorwayTrigger] Doorway entered. Position: {doorwayPosition}, Door Direction: {doorDirection}");

        generationManager.OnPlayerEnterDoorway(doorwayPosition, doorDirection);
    }
}