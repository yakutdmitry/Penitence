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

    public void OpenNorthDoor()
    {
        northWall.SetActive(false);
        if (northDoorway != null) northDoorway.SetActive(true);
    }

    public void OpenSouthDoor()
    {
        southWall.SetActive(false);
        if (southDoorway != null) southDoorway.SetActive(true);
    }

    public void OpenEastDoor()
    {
        eastWall.SetActive(false);
        if (eastDoorway != null) eastDoorway.SetActive(true);
    }

    public void OpenWestDoor()
    {
        westWall.SetActive(false);
        if (westDoorway != null) westDoorway.SetActive(true);
    }
}
