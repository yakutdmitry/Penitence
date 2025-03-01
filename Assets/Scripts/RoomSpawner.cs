using UnityEngine;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    public float roomSize = 10f;

    // Track already-spawned rooms to avoid double spawning
    private HashSet<Vector2Int> spawnedPositions = new HashSet<Vector2Int>();

    public void SpawnRooms(Dictionary<Vector2Int, RoomNode> layout)
    {
        foreach (var kvp in layout)
        {
            Vector2Int gridPos = kvp.Key;
            RoomNode node = kvp.Value;

            // Skip if we've already spawned a room at this position
            if (spawnedPositions.Contains(gridPos))
            {
                Debug.LogWarning($"Skipping duplicate spawn at {gridPos}");
                continue;
            }

            Vector3 worldPos = new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
            GameObject roomInstance = Instantiate(node.template.prefab, worldPos, Quaternion.identity);

            RoomInstance roomComponent = roomInstance.GetComponent<RoomInstance>();
            if (roomComponent != null)
            {
                roomComponent.Initialize(node);
            }

            // Mark position as occupied
            spawnedPositions.Add(gridPos);
        }
    }
}
