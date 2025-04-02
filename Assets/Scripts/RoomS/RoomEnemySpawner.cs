using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class RoomEnemySpawner : MonoBehaviour
{

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int minEnemiesPerRoom = 1;
    [SerializeField] private int maxEnemiesPerRoom = 5;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void SpawnEnemies(RoomInstance room)
    {
        int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
        Debug.Log($"Spawning {enemyCount} enemies in {room.gameObject.name}");

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 spawnPosition = GetRandomSpawnPoint(room);

            // Check if the spawn position is on the NavMesh
            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                spawnedEnemies.Add(enemy);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.AssignRoom(room);
                    room.RegisterEnemy(); // Track enemy count in the room
                }
            }
            else
            {
                Debug.LogWarning("Failed to spawn enemy at position: " + spawnPosition + " - Not on NavMesh");
            }
        }
    }

    private Vector3 GetRandomSpawnPoint(RoomInstance room)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-room.roomSize / 3, room.roomSize / 3),
            0,
            Random.Range(-room.roomSize / 3, room.roomSize / 3)
        );

        return room.transform.position + randomOffset;
    }


    public void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }
}
