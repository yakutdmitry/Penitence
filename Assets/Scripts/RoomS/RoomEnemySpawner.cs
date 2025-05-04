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

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;

                // Check if the spawn position is buried under a platform
                Vector3 rayOrigin = spawnPosition + Vector3.up * 5f;
                float rayDistance = 10f;

                bool isBuriedUnderPlatform = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo, rayDistance, LayerMask.GetMask("IsGround")) &&
                                             (hitInfo.point.y - spawnPosition.y) < -0.5f;

                if (isBuriedUnderPlatform)
                {
                    Debug.LogWarning("Spawn position is buried under platform — skipping");
                    continue;
                }


                // Check for surface collisions (walls, props, clutter)
                if (!Physics.CheckSphere(spawnPosition + Vector3.up * 3f, 3f, LayerMask.GetMask("IsGround")))
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    spawnedEnemies.Add(enemy);

                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.AssignRoom(room);
                        room.RegisterEnemy();
                    }
                }
                else
                {
                    Debug.LogWarning($"Spawn blocked at {spawnPosition} - collided with IsGround layer");
                }
            }
            else
            {
                Debug.LogWarning("Failed to find NavMesh near: " + spawnPosition);
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
