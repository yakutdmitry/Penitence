using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, iDamageable, iEnemyMoveable, iTriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public NavMeshAgent agent { get; set; }  // Replace Rigidbody with NavMeshAgent
    public bool isFacingRight { get; set; } = true;

    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }


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

        agent = GetComponent<NavMeshAgent>();  // Initialize NavMeshAgent

        EnemyAttackBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyIdleBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    public void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    public void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Movement Functions
    public void MoveEnemy(Vector3 destination)
    {
        agent.SetDestination(destination);
        CheckForLeftOrRightFacing(destination - transform.position);
    }

    public void CheckForLeftOrRightFacing(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f) // Prevents jittering
        {
            Vector3 lookPosition = transform.position + direction;
            lookPosition.y = transform.position.y; // Keep enemy upright
            transform.LookAt(lookPosition);
        }
    }

    #endregion

    #region Distance Check

    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
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




    #endregion


    #region Damage System
    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            Die();

        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion
}
