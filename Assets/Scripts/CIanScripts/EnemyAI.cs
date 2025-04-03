using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float chaseRange = 15f;
    public float attackRange = 2f;
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float attackDamage = 10f;

    private NavMeshAgent agent;
    private Transform player;
    private PlayerMovement playerMovement;
    public float CurrentHealth { get; set; }
    public AudioClip MonsterDie;
    public AudioSource MonsterAudioSource;

    private bool isAIEnabled = true; // Track if AI is active

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAIEnabled)
            return; // Skip AI logic if disabled

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // Stops enemies attacking through walls
        if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        MonsterAudioSource.PlayOneShot(MonsterDie);
    }

    // this is for the penance stare ability, it disables/enables the ai script as needed
    public void DisableAI()
    {
        isAIEnabled = false;
        Debug.Log("Enemy sunned");
        agent.isStopped = true; 
        // will add visual indicator later
    }

    public void EnableAI()
    {
        isAIEnabled = true;
        agent.isStopped = false; 
        
    }
}
