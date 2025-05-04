using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform visualDoorMesh;  // Reference to the moving mesh (child)
    [SerializeField] private Animator leftDoorAnimator;         // Animator on the visual mesh
    [SerializeField] private Animator rightDoorAnimator;

    private bool isLocked = false;
    private bool isOpen = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true; // Ensure trigger is active
        }

        // Set the door direction based on the transform's forward vector
        Vector2Int doorDirection = GetDoorDirection();

        if (doorDirection == Vector2Int.left)
        {
            leftDoorAnimator = rightDoorAnimator;
            rightDoorAnimator = leftDoorAnimator;
            // Fix for CS1717: Ensure the correct assignment logic is applied
            if (doorDirection == Vector2Int.up)
            {
                // No change needed here as the assignment is correct
            }
            else if (doorDirection == Vector2Int.down)
            {
                leftDoorAnimator = rightDoorAnimator;
                rightDoorAnimator = leftDoorAnimator;
            }
            else if (doorDirection == Vector2Int.right)
            {
                // No change needed here as the assignment is correct
            }
            else if (doorDirection == Vector2Int.left)
            {
                leftDoorAnimator = rightDoorAnimator;
                rightDoorAnimator = leftDoorAnimator;
            }
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

    private Vector2Int GetDoorDirection()
    {
        Vector3 direction = transform.forward; // Assumes doors face outward

        if (Vector3.Dot(direction, Vector3.forward) > 0.9f) return Vector2Int.up;
        if (Vector3.Dot(direction, Vector3.back) > 0.9f) return Vector2Int.down;
        if (Vector3.Dot(direction, Vector3.right) > 0.9f) return Vector2Int.right;
        if (Vector3.Dot(direction, Vector3.left) > 0.9f) return Vector2Int.left;

        return Vector2Int.zero; // Default case
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{name} - Player entered door trigger!"); // Confirm if this runs
        if (other.CompareTag("Player"))
        {
            TryOpen();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"{name} - Player exited door trigger!"); // Confirm if this runs
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CloseDoorWithDelay(2f)); // Close door after 2 seconds
        }
    }

    private void UpdateDoorAnimation()
    {
        if (isOpen)
        {
            leftDoorAnimator.SetBool("IsOpen", true);
            rightDoorAnimator.SetBool("IsOpen", true);
        }
        else
        {
            leftDoorAnimator.SetBool("IsOpen", false);
            rightDoorAnimator.SetBool("IsOpen", false);
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

    private IEnumerator CloseDoorWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ForceClose();
    }
}