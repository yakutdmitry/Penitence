using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface iEnemyMoveable
{
    public NavMeshAgent agent { get; set; }  // Replace Rigidbody with NavMeshAgent

    bool isFacingRight { get; set; }

    void MoveEnemy(Vector3 velocity);

    void CheckForLeftOrRightFacing(Vector3 velocity);
}
