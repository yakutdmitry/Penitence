using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public RoomNode nodeData;
    public float roomSize = 50f;
    public bool isStartRoom { get; private set; } = false;

    public Vector2Int position;
    private Dictionary<Vector2Int, bool> doors = new();
    private Dictionary<Vector2Int, DoorController> doorMap = new();

    private NavMeshSurface navMeshSurface;
    [HideInInspector] public RoomEnemySpawner enemySpawner;
    private RoomObjectiveController objectiveController;

    public Transform northDoorAnchor;
    public Transform southDoorAnchor;
    public Transform eastDoorAnchor;
    public Transform westDoorAnchor;

    public GameObject doorPrefab;

    public int enemyCount = 0;
    private bool objectiveCompleted = false;

    private void Start()
    {
        navMeshSurface = GetComponentInChildren<NavMeshSurface>();
        enemySpawner = GetComponent<RoomEnemySpawner>();
        objectiveController = GetComponent<RoomObjectiveController>();

        if (navMeshSurface != null)
        {
            Invoke(nameof(BuildNavMeshDelayed), 0.2f);
        }

        if (isStartRoom)
        {
            UnlockAllDoors();
        }

        Initialize(position, Vector2Int.zero);
    }

    private void BuildNavMeshDelayed()
    {
        navMeshSurface.BuildNavMesh();
        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies(this);
        }
    }

    public void Initialize(Vector2Int pos, Vector2Int entryDirection)
    {
        position = pos;
        transform.position = new Vector3(position.x * roomSize, 0, position.y * roomSize);

        doors[Vector2Int.up] = nodeData.template.hasNorthDoor;
        doors[Vector2Int.down] = nodeData.template.hasSouthDoor;
        doors[Vector2Int.left] = nodeData.template.hasWestDoor;
        doors[Vector2Int.right] = nodeData.template.hasEastDoor;

        doors[entryDirection] = true;
    }

    public void SetAsStartRoom()
    {
        isStartRoom = true;
        doors[Vector2Int.up] = doors[Vector2Int.down] = doors[Vector2Int.left] = doors[Vector2Int.right] = true;
        UnlockAllDoors();
    }

    public void SpawnDoors(Vector2Int entryDirection)
    {
        Dictionary<Vector2Int, Transform> anchors = new()
        {
            { Vector2Int.up, northDoorAnchor },
            { Vector2Int.down, southDoorAnchor },
            { Vector2Int.left, westDoorAnchor },
            { Vector2Int.right, eastDoorAnchor }
        };

        foreach (var kvp in anchors)
        {
            Vector2Int dir = kvp.Key;
            if (dir == entryDirection) continue;

            if (doors.TryGetValue(dir, out bool hasDoor) && hasDoor && kvp.Value != null)
            {
                Quaternion rotation = dir == Vector2Int.up ? Quaternion.Euler(0, 0, 0) :
                                      dir == Vector2Int.down ? Quaternion.Euler(0, 180, 0) :
                                      dir == Vector2Int.left ? Quaternion.Euler(0, -90, 0) :
                                      dir == Vector2Int.right ? Quaternion.Euler(0, 90, 0) :
                                      Quaternion.identity;

                GameObject door = Instantiate(doorPrefab, kvp.Value.position, rotation, transform);
                RegisterDoor(dir, door.GetComponent<DoorController>());
            }
        }
    }

    public void RegisterDoor(Vector2Int direction, DoorController door)
    {
        if (!doorMap.ContainsKey(direction))
        {
            doorMap[direction] = door;
            door.SetLocked(!isStartRoom);
        }
    }

    public void DisableEntryTrigger(Vector2Int direction)
    {
        Transform anchor = GetDoorAnchor(direction);
        if (anchor != null)
        {
            Transform trigger = anchor.Find("RoomSpawnTrigger");
            if (trigger != null && trigger.TryGetComponent(out Collider col))
            {
                col.enabled = false;
            }
        }
    }

    public void UnlockAllDoors()
    {
        foreach (var door in doorMap.Values)
        {
            door.SetLocked(false);
        }
    }

    public void SetRoomTriggersActive(bool isActive)
    {
        if (isStartRoom && !isActive) return;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("RoomSpawnTrigger"))
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }

    public Transform GetDoorAnchor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return northDoorAnchor;
        if (direction == Vector2Int.down) return southDoorAnchor;
        if (direction == Vector2Int.left) return westDoorAnchor;
        if (direction == Vector2Int.right) return eastDoorAnchor;
        return null;
    }

    public void RegisterEnemy() => enemyCount++;
    public void EnemyDefeated()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            FindObjectOfType<RoomTracker>()?.RoomObjectiveCompleted();
            objectiveController?.CheckObjective();
        }
    }

    public void PlayerEnteredRoom()
    {
        if (enemySpawner != null)
            enemySpawner.SpawnEnemies(this);

        if (objectiveController != null && !objectiveCompleted)
            InvokeRepeating(nameof(CheckObjective), 1f, 1f);
    }

    private void CheckObjective()
    {
        if (objectiveController != null)
        {
            objectiveController.CheckObjective();
            if (objectiveCompleted)
                CancelInvoke(nameof(CheckObjective));
        }
    }

    public void SetObjectiveCompleted() => objectiveCompleted = true;

    public List<RoomInstance> GetAdjacentRooms()
    {
        List<RoomInstance> rooms = new();
        foreach (Vector2Int dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int pos = position + dir;
            if (DoorwayGenerationManager.instance != null &&
                DoorwayGenerationManager.instance.TryGetRoomAt(pos, out RoomInstance room))
            {
                rooms.Add(room);
            }
        }
        return rooms;
    }
}
