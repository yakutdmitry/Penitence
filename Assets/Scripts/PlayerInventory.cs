using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    private HashSet<GameObject> keys = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddKey(GameObject room)
    {
        keys.Add(room);
    }

    public bool HasKeyForRoom(GameObject room)
    {
        return keys.Contains(room);
    }
}
