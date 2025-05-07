using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform visualDoorMesh;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AudioSource doorCreakSFX;

    private bool isLocked = false;
    private bool isOpen = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
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
        if (doorCreakSFX != null)
        {
            doorCreakSFX.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Only open if not locked
        if (!isLocked && !isOpen)
        {
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CloseDoorWithDelay(2f));
        }
    }

    private void UpdateDoorAnimation()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsOpen", isOpen);
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
