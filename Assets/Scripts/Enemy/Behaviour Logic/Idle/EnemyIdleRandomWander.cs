using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Idle_Random Wander", menuName = "Enemy_Logic/Idle_Logic/Random_Wander")]
public class EnemyIdleRandomWander : EnemyIdleSOBase
{
    [SerializeField] public float RandomMovementRange = 5f;
    [SerializeField] public float RandomMovementSpeed = 1f;

    private Vector3 targetPosition;
    private Vector3 direction;

    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        targetPosition = GetRandomPointInCircle();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        if (enemy.agent.remainingDistance < 0.5f) // If reached the point, pick a new one
        {
            targetPosition = GetRandomPointInCircle();
            enemy.MoveEnemy(targetPosition);
        }
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

    private Vector3 GetRandomPointInCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle * RandomMovementRange;
        return new Vector3(
            enemy.transform.position.x + randomPoint.x,
            enemy.transform.position.y,
            enemy.transform.position.z + randomPoint.y
        );
    }
}
