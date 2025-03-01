using UnityEngine;

public class RoomDoorController : MonoBehaviour
{
    public GameObject northWall;
    public GameObject southWall;
    public GameObject eastWall;
    public GameObject westWall;

    public GameObject northDoorway;
    public GameObject southDoorway;
    public GameObject eastDoorway;
    public GameObject westDoorway;

    private bool isLocked = false;

    public void OpenNorthDoor()
    {
        northWall.SetActive(false);
        if (northDoorway != null) northDoorway.SetActive(true);
        UpdateDoorLockState(northDoorway);
    }

    public void OpenSouthDoor()
    {
        southWall.SetActive(false);
        if (southDoorway != null) southDoorway.SetActive(true);
        UpdateDoorLockState(southDoorway);
    }

    public void OpenEastDoor()
    {
        eastWall.SetActive(false);
        if (eastDoorway != null) eastDoorway.SetActive(true);
        UpdateDoorLockState(eastDoorway);
    }

    public void OpenWestDoor()
    {
        westWall.SetActive(false);
        if (westDoorway != null) westDoorway.SetActive(true);
        UpdateDoorLockState(westDoorway);
    }

    // New method to lock/unlock all doors this controller manages
    public void SetLocked(bool locked)
    {
        isLocked = locked;

        UpdateDoorLockState(northDoorway);
        UpdateDoorLockState(southDoorway);
        UpdateDoorLockState(eastDoorway);
        UpdateDoorLockState(westDoorway);
    }

    private void UpdateDoorLockState(GameObject doorway)
    {
        if (doorway == null) return;

        // Example: Disable the collider when locked
        Collider col = doorway.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = !isLocked;
        }

        // Example: Change color to red if locked, green if unlocked
        Renderer renderer = doorway.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = isLocked ? Color.red : Color.green;
        }
    }
}
