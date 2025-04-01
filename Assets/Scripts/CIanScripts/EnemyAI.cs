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
    private HealthManager playerHealth;
    public float CurrentHealth { get; set; }
    public AudioClip MonsterDie;
    public AudioSource MonsterAudioSource;

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //stops enemies attacking thru walls
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
}