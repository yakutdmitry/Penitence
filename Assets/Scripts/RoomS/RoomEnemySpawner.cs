using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class RoomEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int minEnemiesPerRoom = 1;
    [SerializeField] private int maxEnemiesPerRoom = 3;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void SpawnEnemies(RoomInstance room)
    {
        int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 spawnPosition = GetRandomSpawnPoint(room);

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemy);
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
