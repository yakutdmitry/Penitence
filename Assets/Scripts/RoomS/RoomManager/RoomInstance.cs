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

    public void SpawnDoors()
    {
        //Debug.Log("Spawning doors for room: " + gameObject.name);

        if (doors[Vector2Int.up] && northDoorAnchor != null)
        {
            //Debug.Log($"Spawning north door at: {northDoorAnchor.position}");
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), northDoorAnchor.position, Quaternion.identity, transform);
        }
        else
        {
            //Debug.LogWarning("North door missing due to conditions.");
        }

        if (doors[Vector2Int.down] && southDoorAnchor != null)
        {
            //Debug.Log("Spawning south door at: " + southDoorAnchor.position);
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), southDoorAnchor.position, Quaternion.Euler(0, 180, 0), transform);
        }
        else
        {
            //Debug.LogWarning("South door missing due to conditions.");
        }

        if (doors[Vector2Int.right] && eastDoorAnchor != null)
        {
            //Debug.Log("Spawning east door at: " + eastDoorAnchor.position);
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), eastDoorAnchor.position, Quaternion.Euler(0, 90, 0), transform);
        }
        else
        {
            //Debug.LogWarning("East door missing due to conditions.");
        }

        if (doors[Vector2Int.left] && westDoorAnchor != null)
        {
            //Debug.Log("Spawning west door at: " + westDoorAnchor.position);
            Instantiate(Resources.Load<GameObject>("DoorPrefab"), westDoorAnchor.position, Quaternion.Euler(0, -90, 0), transform);
        }
        else
        {
            //Debug.LogWarning("West door missing due to conditions.");
        }
    }

    public void SetDoorState(bool north, bool south, bool east, bool west)
    {
        doors[Vector2Int.up] = north;
        doors[Vector2Int.down] = south;
        doors[Vector2Int.left] = west;
        doors[Vector2Int.right] = east;

        //Debug.Log($"Room at {position} door states - North: {north}, South: {south}, East: {east}, West: {west}");
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (northDoorAnchor != null) Gizmos.DrawSphere(northDoorAnchor.position, 0.5f);
        if (southDoorAnchor != null) Gizmos.DrawSphere(southDoorAnchor.position, 0.5f);
        if (eastDoorAnchor != null) Gizmos.DrawSphere(eastDoorAnchor.position, 0.5f);
        if (westDoorAnchor != null) Gizmos.DrawSphere(westDoorAnchor.position, 0.5f);
    }
}
