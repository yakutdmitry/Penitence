using System.Collections;
using UnityEngine;

public class PenanceStare : MonoBehaviour
{
    public Camera playerCamera;
    public float coneAngle = 45f;
    public float coneRange = 10f;
    public float StareDuration = 0.5f;  // this is how long the actual ability lasts, so like, how long the penance stare is active
    public float stunDuration = 5f;     // this is how long the enemies are stunned for if they’re caught in it
    public KeyCode Keybind = KeyCode.F;

    public float cooldownDuration = 3f;  // in seconds
    private float cooldownTimer = 0f;    //i worded these weird, but this is checking what number the timer has to be at to enable the ability again

    private bool isActive = false;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; //ensures the camera is actually selected, as the scene loader kinda just seems to skip over it?
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(Keybind) && cooldownTimer <= 0f)
        {
            CastPenanceStareCone();
            cooldownTimer = cooldownDuration;  
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;  
        }
    }

    void CastPenanceStareCone()
    {
        isActive = true;

        int rayCount = 10; // might need altering for consistency
        float angleStep = coneAngle / rayCount;
        for (int i = -rayCount / 2; i < rayCount / 2; i++)
        {
            float currentAngle = i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * playerCamera.transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, rayDirection, out hit, coneRange))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    StartCoroutine(DisableEnemyAI(enemy));
                }
            }
        }

        StartCoroutine(DeactivatePenanceStare());
    }

    IEnumerator DeactivatePenanceStare()
    {
        yield return new WaitForSeconds(StareDuration);
        isActive = false;
    }

    // this all references peters Enemy baseclass and disables the navmesh temporarily
    IEnumerator DisableEnemyAI(Enemy enemy)
    {
        // disables navmesh and turns velocity to 0 to prevent weird sliding effect
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;  // Ensure velocity is zero to stop sliding

        enemy.SetAggroStatus(false);
        enemy.StateMachine.ChangeState(enemy.IdleState);

        yield return new WaitForSeconds(stunDuration);

        // re activates movement
        enemy.agent.isStopped = false;
        enemy.agent.velocity = Vector3.zero;
        enemy.SetAggroStatus(true);
        enemy.StateMachine.ChangeState(enemy.ChaseState);
    }

    private void OnDrawGizmos()
    {
        if (playerCamera == null || !isActive) return;

        int rayCount = 15; // might need altering for consistency
        float angleStep = coneAngle / rayCount;

        for (int i = -rayCount / 2; i < rayCount / 2; i++)
        {
            float currentAngle = i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * playerCamera.transform.forward;

            // Visualize rays in scene view for debug, it SHOULDNT BE VISIBLE IN GAME but sometimes it just decides to be so idk
            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerCamera.transform.position, playerCamera.transform.position + rayDirection * coneRange);
        }
    }
}
