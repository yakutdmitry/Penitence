using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack_Single Straight Projectile", menuName = "Enemy_Logic/Attack_Logic/Single_Straight_Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase
{
    [SerializeField] private Rigidbody projectilePrefab;
    [SerializeField] private float timeBetweenShots = 2f;
    [SerializeField] private float timeTillExit = 3f;
    [SerializeField] private float distanceToCountExit = 3f;
    [SerializeField] private float projectileSpeed = 10f;

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

        enemy.MoveEnemy(Vector3.zero); // Stop moving while attacking

        timer += Time.deltaTime;

        if (timer >= timeBetweenShots)
        {
            FireProjectile();
            timer = 0;
        }

        // Handle exit conditions separately
        if (Vector3.Distance(playerTransform.position, enemy.transform.position) > distanceToCountExit)
        {
            exitTimer += Time.deltaTime;

            if (exitTimer >= timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
        }
        else
        {
            exitTimer = 0;
        }
    }

    private void FireProjectile()
    {
        if (enemy.projectileSpawnPoint == null)
        {
            Debug.LogError("Projectile Spawn Point is missing on " + enemy.gameObject.name);
            return;
        }

        Debug.Log("Firing Projectile");

        Vector3 direction = (playerTransform.position - enemy.projectileSpawnPoint.position).normalized;
        direction.y = 0; // Keep projectile movement on XZ plane

        Rigidbody projectile = Instantiate(projectilePrefab, enemy.projectileSpawnPoint.position, Quaternion.identity);
        projectile.velocity = direction * projectileSpeed;
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
