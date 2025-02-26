using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemybehaviour : MonoBehaviour
{
    public AudioClip found;  
    private AudioSource audioSource;  
    public bool Alerted;
    public bool Patrol;
    public Rigidbody EnemyRB;
    public Transform PlayerPosition;
    public GameObject KillboxEnemy;
    public bool audiocheck;

    public List<Transform> Waypoints;  // List of waypoints to patrol between
    public float patrolSpeed = 2.0f;  // next lines are speed variables and the checker for waypoints
    public float pursuitSpeed = 4.0f;  
    public float rotationSpeed = 5.0f;  
    private int currentWaypointIndex = 0; 

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        Patrol = true; 
        GetComponentInChildren<BoxCollider>(). enabled = false;

    }

    void Update()
    {
        Statehandler();
        VisionConeCheck();

    }


    private void Statehandler()
    {
        if (!Alerted && Patrol)
        {
            EnemyPatrol();
        }
        else if (Alerted)
        {
            EnemyPursue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {



            Debug.Log("Player Detected!");

            SceneManager.LoadScene("AI test"); // change scene *************
        }
    }

    private void EnemyPatrol()
    {
        if (Waypoints.Count > 0)
        {
            Transform targetWaypoint = Waypoints[currentWaypointIndex];

            //waypoint orientatiomn
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, patrolSpeed * Time.deltaTime);

            // Check if the enemy has reached the current waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                // Move to the next waypoint (loops back to the first one)
                currentWaypointIndex = (currentWaypointIndex + 1) % Waypoints.Count;
            }
        }
    }

    private void EnemyPursue()
    {
        if (PlayerPosition != null)
        {
            if (audioSource != null && found != null)
            {
                audioSource.PlayOneShot(found);

            }
            
            Vector3 direction = (PlayerPosition.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            transform.position = Vector3.MoveTowards(transform.position, PlayerPosition.position, pursuitSpeed * Time.deltaTime);

            GetComponentInChildren<BoxCollider>().enabled = true;
        }
    }

    private void VisionConeCheck()
    {
        Vector3 forwardDirection = transform.forward;

        Vector3 directionToPlayer = PlayerPosition.position - transform.position;
        float angleToPlayer = Vector3.Angle(forwardDirection, directionToPlayer);
        //raycast + angles
        if (angleToPlayer < 45.0f / 2 && directionToPlayer.magnitude <= 10.0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, 10.0f))
            {
                //
                //switch to pursue state if cauggt
                if (hit.collider.CompareTag("Player"))
                {
                    if (!Alerted)
                    {
                        Alerted = true;
                        Patrol = false;  // Stop patrolling and start pursuing
                        Debug.Log("Player spotted! Switching to pursuit.");
                    }
                }
            }
        }
    }
}
