using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Attack_Melee", menuName = "Enemy_Logic/Attack_Logic/Melee")]
public class EnemyAttackMelee : EnemyAttackSOBase
{
    [Header("Attack Settings")]
    public float timeBetweenAttacks = 1f;
    public float attackRange = 1f;
    public float attackDamage = 10f;

    private iDamageable playerHealth;
    private float timer;
    private float exitTimer;

    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        timer += Time.deltaTime;

        if (timer >= timeBetweenAttacks)
        {
            if (Vector3.Distance(playerTransform.position, enemy.transform.position) <= attackRange)
            {
                //enemy.Animator.SetTrigger("Attack");
                timer = 0;
                AttackPlayer();

            }
        }
    }

    private void AttackPlayer()
    {
        playerHealth = playerTransform.GetComponent<iDamageable>();
        playerHealth.TakeDamage(attackDamage);
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
