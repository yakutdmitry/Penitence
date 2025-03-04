using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen = false;

    private Animator animator;
    private bool isLocked = true;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator found on door or its children!");
            return;
        }

        ForceClose();  // Start with door closed
    }

    public void OnPlayerApproach()
    {
        if (!isLocked)
        {
            ToggleDoor();
        }
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        UpdateVisuals();
    }

    public void ToggleDoor()
    {
        if (isLocked) return;

        isOpen = !isOpen;
        UpdateVisuals();
    }

    public void ForceClose()
    {
        isOpen = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", isOpen);
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = isLocked ? Color.red : (isOpen ? Color.green : Color.white);
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = !isLocked;
        }
    }
}
