using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    private bool isLocked = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ForceClose();  // Start doors closed unless overridden.
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }

    public void OnPlayerApproach()
    {
        if (!isLocked)
        {
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        bool isOpen = animator.GetBool("isOpen");
        animator.SetBool("isOpen", !isOpen);
    }

    public void ForceClose()
    {
        animator.SetBool("isOpen", false);
    }
}
