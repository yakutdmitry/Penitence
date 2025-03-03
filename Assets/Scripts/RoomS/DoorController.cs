using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isLocked;

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = isLocked ? Color.red : Color.green;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = !isLocked;
        }
    }
}
