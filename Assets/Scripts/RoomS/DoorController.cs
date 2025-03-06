using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform visualDoorMesh;  // Reference to the moving mesh (child)
    [SerializeField] private Animator animator;         // Animator on the visual mesh

    private bool isLocked = false;
    private bool isOpen = false;

    private void Start()
    {
        // In case animator is not set, auto find it (but better to link it directly in prefab)
        if (animator == null && visualDoorMesh != null)
        {
            animator = visualDoorMesh.GetComponent<Animator>();
        }

        ForceClose(); // Start closed
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        if (isLocked)
        {
            ForceClose();
        }
    }

    public void TryOpen()
    {
        if (!isLocked && !isOpen)
        {
            Open();
        }
    }

    public void ForceClose()
    {
        isOpen = false;
        UpdateDoorAnimation();
    }

    private void Open()
    {
        isOpen = true;
        UpdateDoorAnimation();
    }

    private void UpdateDoorAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", isOpen);
        }
        else
        {
            Debug.LogWarning($"{name} - Missing Animator on visual mesh!");
        }
    }

    public void ToggleDoor()
    {
        if (isOpen)
        {
            ForceClose();
        }
        else
        {
            TryOpen();
        }
    }
}
