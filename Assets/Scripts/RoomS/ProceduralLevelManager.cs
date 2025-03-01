
using UnityEngine;

public class ProceduralLevelManager : MonoBehaviour
{
    public ProceduralMapGenerator mapGenerator;
    public RoomSpawner roomSpawner;

    void Start()
    {
        var layout = mapGenerator.GenerateLayout();
        roomSpawner.SpawnRooms(layout);
    }
}
