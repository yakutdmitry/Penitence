using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, iDamageable, iEnemyMoveable, iTriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public NavMeshAgent agent { get; set; }

    public bool isFacingRight { get; set; } = true;

    public bool IsAggroed { get; private set; }
    public bool IsWithinStrikingDistance { get; private set; }

    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float hearingRange = 15f;
    public float fieldOfView = 60f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;

    private Vector3 lastKnownPosition;
    private bool playerInSight;
    private bool playerHeard;

    private GameObject player;
    private HealthManager playerHealth;
    private RoomObjectiveController roomObjective;
    private RoomInstance roomInstance;

    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    #endregion

    #region ScriptableObject Variables
    [SerializeField] private EnemyIdleSOBase EnemyIdleBase;
    [SerializeField] private EnemyChaseSOBase EnemyChaseBase;
    [SerializeField] private EnemyAttackSOBase EnemyAttackBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
    bool iTriggerCheckable.IsAggroed { get; set; }
    bool iTriggerCheckable.IsWithinStrikingDistance { get; set; }
    #endregion

    #region Attack State Variables
    public Transform projectileSpawnPoint;
    #endregion

    private void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
        EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
        EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);

        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
    }

    public void Start()
    {
        CurrentHealth = MaxHealth;
        agent = GetComponent<NavMeshAgent>();

        EnemyAttackBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyIdleBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);

        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthManager>();
        }

        // Find RoomObjectiveController in the parent (assuming enemy is a child of the room)
        roomObjective = GetComponentInParent<RoomObjectiveController>();
    }

    public void AssignRoom(RoomInstance room)
    {
        roomInstance = room;
    }

    public void Update()
    {
        SensePlayer();
        HandleStates();
    }

    #region Player Detection System

    private void SensePlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool canSeePlayer = false;
        bool canHearPlayer = distanceToPlayer <= hearingRange;

        // **VISION DETECTION**
        if (distanceToPlayer <= detectionRange && angleToPlayer <= fieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, detectionRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    canSeePlayer = true;
                }
            }
        }

        playerInSight = canSeePlayer;
        playerHeard = canHearPlayer;

        // **AGGRO HANDLING**
        if (playerInSight || playerHeard)
        {
            lastKnownPosition = player.transform.position;
            SetAggroStatus(true);
        }
        else
        {
            SetAggroStatus(false);
        }

        // **STRIKING DISTANCE HANDLING**
        SetStrikingDistanceBool(distanceToPlayer <= attackRange);
    }

    private void HandleStates()
    {
        if (IsAggroed)
        {
            if (IsWithinStrikingDistance)
            {
                StateMachine.ChangeState(AttackState);
                AttackPlayer();
            }
            else
            {
                StateMachine.ChangeState(ChaseState);
                agent.SetDestination(lastKnownPosition);
            }
        }
        else
        {
            StateMachine.ChangeState(IdleState);
        }
    }

    #endregion

    #region Combat System

    public void TakeDamage(float damageAmount)
    {
       // Debug.Log($"{gameObject.name} took {damageAmount} damage! Current health: {CurrentHealth - damageAmount}");

        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            //Debug.Log($"{gameObject.name} died!");
            Die();
        }
    }

    public void AttackPlayer()
    {
        if (Time.time >= attackCooldown)
        {
            playerHealth.TakeDamage(attackDamage);
            attackCooldown = Time.time + attackCooldown;
        }
    }

    public void Die()
    {
        //Debug.Log($"{gameObject.name} is being destroyed!");

        // Notify the room that an enemy has been defeated
        if (roomInstance != null)
        {
            roomInstance.EnemyDefeated();
        }

        Destroy(gameObject);
    }

    #endregion

    #region Interface Methods

    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }

    public void MoveEnemy(Vector3 destination)
    {
        agent.SetDestination(destination);
        CheckForLeftOrRightFacing(destination - transform.position);
    }

    public void CheckForLeftOrRightFacing(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Vector3 lookPosition = transform.position + direction;
            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition);
        }
    }

    #endregion


    #region Animation Triggers

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        // TO DO
        // This is where we will call the animation trigger events
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayFootstepSound,
    }

    private void UpdateAnimations()
    {
        // TO DO
        // This is where we will update the animations
    }

    #endregion


    #region Debug Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vision Cone
        Gizmos.color = Color.green;
        Vector3 fovLine1 = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * detectionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }

    public void SetAggroedBool(bool value)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
