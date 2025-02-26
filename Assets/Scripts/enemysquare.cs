using UnityEngine;

public class MoveInSquare : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveDistance = 5f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private int direction = 0;

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    void Update()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If the object reaches the target position, change the direction
        if (transform.position == targetPosition)
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        // Change the direction to create a square movement pattern
        switch (direction)
        {
            case 0: // Move forward
                targetPosition = startPosition + transform.forward * moveDistance;
                break;
            case 1: // Move right
                targetPosition = startPosition + transform.right * moveDistance;
                break;
            case 2: // Move backward
                targetPosition = startPosition - transform.forward * moveDistance;
                break;
            case 3: // Move left
                targetPosition = startPosition - transform.right * moveDistance;
                break;
        }

        // Update the direction for the next move
        direction = (direction + 1) % 4;
    }
}
