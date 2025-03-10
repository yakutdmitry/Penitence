using UnityEngine;
using UnityEngine.AI; // For pathfinding

public class EnemyAI : MonoBehaviour
{
    public float chaseRange = 15f; // Range to start chasing the player
    public float attackRange = 2f; // Range to start attacking
    [field: SerializeField] public float MaxHealth { get; set; } = 100f; // Enemy health
    public float attackDamage = 10f; // Damage dealt when attacking

    private NavMeshAgent agent; // Navigation agent for movement
    private Transform player; // Reference to the player
    //private bool isChasing = false;
    private PlayerMovement playerMovement; // Reference to player's movement script
    private HealthManager playerHealth;
    public float CurrentHealth { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>(); // Get the PlayerMovement component from the player
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within the chase range, start chasing
        if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }

        // If the enemy is close enough to the player, attack
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void ChasePlayer()
    {
        // Start chasing the player
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // If the player is within attack range, deal damage to the player
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(attackDamage); // Call TakeDamage on the PlayerMovement component
            Debug.Log("Enemy Attacks Player!");
        }
    }

    // Function to handle taking damage
    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"{gameObject.name} took {damageAmount} damage! Current health: {CurrentHealth - damageAmount}");

        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} died! Calling Die().");
            Die();  // This should remove the enemy
        }
    }

    private void Die()
    {
        // Play death animation or effects
        Debug.Log("Enemy Died!");
        Destroy(gameObject); // Destroy enemy GameObject for now
    }
}
