using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [HideInInspector] public RoomPool roomPool; // Uses scriptable object
    [SerializeField] public int maxRooms = 15;
    [SerializeField] public int minRooms = 7;

    [HideInInspector] public Dictionary<Vector2Int, RoomNode> roomNodes = new(); // Tracks all placed rooms

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GenerateRooms()
    {
        Vector2Int startPos = Vector2Int.zero;
        RoomTemplate startTemplate = roomPool.GetRandomRoom(); // Get random starting room

        RoomNode startNode = new RoomNode(startPos, startTemplate);
        roomNodes[startPos] = startNode;

        SpawnRoom(startNode);
    }

    public bool HasRoomAt(Vector2Int pos)
    {
        return roomNodes.ContainsKey(pos);
    }

    public void RegisterRoom(Vector2Int pos, RoomNode node)
    {
        if (!HasRoomAt(pos))
            roomNodes[pos] = node;
    }

    public RoomNode GetRoomAt(Vector2Int pos)
    {
        return roomNodes.ContainsKey(pos) ? roomNodes[pos] : null;
    }

    public void SpawnRoom(RoomNode node)
    {
        Vector3 worldPos = new Vector3(node.position.x * 50, 0, node.position.y * 50);
        node.instance = Instantiate(node.template.prefab, worldPos, Quaternion.identity);

        RoomSpawner spawner = node.instance.GetComponent<RoomSpawner>();
        if (spawner)
        {
            spawner.Initialize(node);
        }
    }
}
