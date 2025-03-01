
using UnityEngine;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    public float roomSize = 10f;

    public void SpawnRooms(Dictionary<Vector2Int, RoomNode> layout)
    {
        foreach (var kvp in layout)
        {
            Vector2Int gridPos = kvp.Key;
            RoomNode node = kvp.Value;

            Vector3 worldPos = new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
            GameObject roomInstance = Instantiate(node.template.roomPrefab, worldPos, Quaternion.identity);

            RoomInstance roomComponent = roomInstance.GetComponent<RoomInstance>();
            if (roomComponent != null)
            {
                roomComponent.Initialize(node);
            }
        }
    }
}
