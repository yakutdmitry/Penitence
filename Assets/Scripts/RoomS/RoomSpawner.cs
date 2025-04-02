using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    private RoomNode roomNode;

    public void Initialize(RoomNode node)
    {
        roomNode = node;
        StartCoroutine(SpawnNeighbors());
    }

    private IEnumerator SpawnNeighbors()
    {
        yield return new WaitForSeconds(0.1f); // Prevents overlap

        foreach (Vector2Int direction in roomNode.template.openDoors) // Use the fixed definition
        {
            Vector2Int neighborPos = roomNode.position + direction;

            if (RoomManager.Instance.HasRoomAt(neighborPos))
            {
                RoomNode neighbor = RoomManager.Instance.GetRoomAt(neighborPos);
                roomNode.AddNeighbor(neighbor);
                neighbor.AddNeighbor(roomNode);
            }
            else if (RoomManager.Instance.roomNodes.Count < RoomManager.Instance.maxRooms)
            {
                RoomTemplate newTemplate = RoomManager.Instance.roomPool.GetRandomRoom();
                RoomNode newNode = new RoomNode(neighborPos, newTemplate);

                RoomManager.Instance.RegisterRoom(neighborPos, newNode);
                RoomManager.Instance.SpawnRoom(newNode);

                roomNode.AddNeighbor(newNode);
                newNode.AddNeighbor(roomNode);
            }
        }
    }

}
