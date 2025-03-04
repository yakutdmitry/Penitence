using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isAlive = true;

    public void Kill()
    {
        isAlive = false;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
