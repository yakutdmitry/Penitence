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
    private DoorwayGenerationManager doorwayGenerationManager;

    public Transform northDoorAnchor;
    public Transform southDoorAnchor;
    public Transform eastDoorAnchor;
    public Transform westDoorAnchor;

    public GameObject doorPrefab;

    public int enemyCount = 0;
    private bool objectiveCompleted = false;

    public Transform GetDoorAnchor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return northDoorAnchor;
        if (direction == Vector2Int.down) return southDoorAnchor;
        if (direction == Vector2Int.right) return eastDoorAnchor;
        if (direction == Vector2Int.left) return westDoorAnchor;
        return null;
    }

    private void Start()
    {
        navMeshSurface = GetComponentInChildren<NavMeshSurface>();
        enemySpawner = GetComponent<RoomEnemySpawner>(); // Use GetComponent to find the component
                                                         //Debug.Log("Enemy spawner: " + enemySpawner);
        objectiveController = GetComponent<RoomObjectiveController>();
        doorwayGenerationManager = FindObjectOfType<DoorwayGenerationManager>();

        if (navMeshSurface != null)
        {
            Invoke(nameof(BuildNavMeshDelayed), 0.2f);
        }

        if (isStartRoom)
        {
            UnlockAllDoors();
        }

        // Move initialization logic here
        Initialize(position, Vector2Int.zero);
    }

    private void BuildNavMeshDelayed()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh built for room: " + gameObject.name);

        // Spawn enemies after the NavMesh is built
        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies(this);
        }
    }

    public void Initialize(Vector2Int pos, Vector2Int entryDirection)
    {
        position = pos;

        // Set world position based on roomSize
        transform.position = new Vector3(position.x * roomSize, 0, position.y * roomSize);

        // Initialize door states based on room template
        doors[Vector2Int.up] = nodeData.template.hasNorthDoor;
        doors[Vector2Int.down] = nodeData.template.hasSouthDoor;
        doors[Vector2Int.left] = nodeData.template.hasWestDoor;
        doors[Vector2Int.right] = nodeData.template.hasEastDoor;

        // Ensure the entry direction door is set to true
        doors[entryDirection] = true;

        // Debug log for door states
        //Debug.Log($"Room at {position} initialized with doors: North: {doors[Vector2Int.up]}, South: {doors[Vector2Int.down]}, East: {doors[Vector2Int.right]}, West: {doors[Vector2Int.left]}");

        // Spawn enemies in the room
        //if (enemySpawner != null)
        //{
        //    //Debug.Log("Spawning enemies in room: " + gameObject.name);
        //    enemySpawner.SpawnEnemies(this);
        //    //Debug.Log("Enemies spawned in room: " + gameObject.name);
        //}
        //else
        //{
        //    Debug.LogWarning("Enemy spawner is null in room: " + gameObject.name);
        //}
    }

    public void SetAsStartRoom()
    {
        isStartRoom = true;

        // Unlock all doors by default for the start room
        doors[Vector2Int.up] = true;
        doors[Vector2Int.down] = true;
        doors[Vector2Int.left] = true;
        doors[Vector2Int.right] = true;

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
            Transform anchor = kvp.Value;

            // Skip the entry direction to avoid spawning a door there
            if (dir == entryDirection) continue;

            if (doors.TryGetValue(dir, out bool shouldHaveDoor) && shouldHaveDoor && anchor != null)
            {
                Quaternion rotation = dir == Vector2Int.up ? Quaternion.Euler(0, 0, 0) :
                                      dir == Vector2Int.down ? Quaternion.Euler(0, 180, 0) :
                                      dir == Vector2Int.left ? Quaternion.Euler(0, -90, 0) :
                                      dir == Vector2Int.right ? Quaternion.Euler(0, 90, 0) :
                                      Quaternion.identity;

                GameObject door = Instantiate(doorPrefab, anchor.position, rotation, transform);
                DoorController controller = door.GetComponent<DoorController>();
                RegisterDoor(dir, controller);
            }
        }
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * roomSize, 0, gridPos.y * roomSize);
    }

    public void SetDoorState(bool north, bool south, bool east, bool west)
    {
        doors[Vector2Int.up] = north;
        doors[Vector2Int.down] = south;
        doors[Vector2Int.left] = west;
        doors[Vector2Int.right] = east;

        //Debug.Log($"Room at {position} door states - North: {north}, South: {south}, East: {east}, West: {west}");
    }
    public void SpawnDoors()
    {
        if (!isStartRoom) return; // Only spawn doors for the start room

        if (doors[Vector2Int.up] && northDoorAnchor != null)
        {
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), northDoorAnchor.position, Quaternion.identity, transform);
        }

        if (doors[Vector2Int.down] && southDoorAnchor != null)
        {
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), southDoorAnchor.position, Quaternion.Euler(0, 180, 0), transform);
        }

        if (doors[Vector2Int.right] && eastDoorAnchor != null)
        {
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), eastDoorAnchor.position, Quaternion.Euler(0, 90, 0), transform);
        }

        if (doors[Vector2Int.left] && westDoorAnchor != null)
        {
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), westDoorAnchor.position, Quaternion.Euler(0, -90, 0), transform);
        }
    }



    private void OpenExtraDoors()
    {
        int extraDoors = Random.Range(1, 3);
        List<Vector2Int> possibleDirections = new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        possibleDirections.RemoveAll(d => doors[d]);
        Shuffle(possibleDirections);

        for (int i = 0; i < extraDoors && i < possibleDirections.Count; i++)
        {
            doors[possibleDirections[i]] = true;
        }
    }

    public void MarkDoorAsEntered(Vector2Int direction)
    {
        if (doors.ContainsKey(direction))
        {
            doors[direction] = true;  // Mark door as entered
        }
    }

    // log the doordirection the player entered from
    public void LogDoorDirection(Vector2Int direction)
    {
        Debug.Log($"Player entered room from {direction}");
    }


    public bool IsDoorOpen(Vector2Int doorDirection)
    {
        return doors.ContainsKey(doorDirection) && doors[doorDirection];
    }

    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void RegisterDoor(Vector2Int direction, DoorController door)
    {
        if (!doorMap.ContainsKey(direction))
        {
            doorMap[direction] = door;
            door.SetLocked(!isStartRoom); // Unlock doors in start room
        }
    }

    public List<RoomInstance> GetAdjacentRooms()
    {
        List<RoomInstance> adjacentRooms = new List<RoomInstance>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacentPos = position + dir;
            RoomInstance adjacentRoom = RoomGenerationManager.Instance.GetRoomAtPosition(adjacentPos);

            if (adjacentRoom != null)
            {
                adjacentRooms.Add(adjacentRoom);
            }
        }

        return adjacentRooms;
    }


    public void RegisterEnemy()
    {
        enemyCount++;
    }

    public void EnemyDefeated()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            FindObjectOfType<RoomTracker>()?.RoomObjectiveCompleted();
            objectiveController?.CheckObjective();
        }
    }

    public void UpgradeCompleted()
    {
        FindObjectOfType<RoomTracker>()?.UpgradeRoomCompleted();
    }


    public void PlayerEnteredRoom()
    {
        RoomGenerationManager.Instance.TrySpawnAdjacentRooms(this);

        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies(this);
        }

        if (objectiveController != null && !objectiveCompleted)
        {
            InvokeRepeating(nameof(CheckObjective), 1f, 1f);
        }
    }


    private void CheckObjective()
    {
        if (objectiveController != null)
        {
            objectiveController.CheckObjective();

            if (objectiveCompleted)
            {
                CancelInvoke(nameof(CheckObjective));
            }
        }
    }

    public void SetRoomTriggersActive(bool isActive)
    {
        // Ensure start room door triggers are never deactivated
        if (isStartRoom && !isActive) return;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("DoorTrigger"))
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }


    private bool DoesRoomHaveDoorFacing(RoomTemplate template, Vector2Int direction)
    {
        return template.openDoors.Contains(direction);
    }

    public void SetObjectiveCompleted()
    {
        objectiveCompleted = true;
    }


    public void UnlockAllDoors()
    {
        //Debug.Log("Unlocking all doors in room: " + gameObject.name);
        foreach (var door in doorMap.Values)
        {
            door.SetLocked(false);
        }
    }

    public void DisableEntryTrigger(Vector2Int direction)
    {
        Transform anchor = GetDoorAnchor(direction);
        if (anchor != null)
        {
            Transform triggerObj = anchor.Find("DoorTrigger");
            if (triggerObj != null)
            {
                Collider col = triggerObj.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (northDoorAnchor != null) Gizmos.DrawSphere(northDoorAnchor.position, 0.5f);
        if (southDoorAnchor != null) Gizmos.DrawSphere(southDoorAnchor.position, 0.5f);
        if (eastDoorAnchor != null) Gizmos.DrawSphere(eastDoorAnchor.position, 0.5f);
        if (westDoorAnchor != null) Gizmos.DrawSphere(westDoorAnchor.position, 0.5f);
    }
}
