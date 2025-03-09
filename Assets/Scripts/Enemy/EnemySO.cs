using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


[CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy/EnemySO")]
public class EnemySO : ScriptableObject
{

    [Header("Enemy Sprite")]
    public Sprite enemySprite;

    [Header("Enemy Attributes")]
    public string enemyName;
    public int health;
    public int damage;
    public float speed;
    public float attackRange;
    public float attackCooldown;
    public float visionRange;
    public float wanderRadius;
    public float wanderCooldown;
    public float chaseSpeedMultiplier;

    [Header("Enemy Data")]
    public EnemyType enemyType;
    //public GameObject projectilePrefab;
    //public GameObject playerPrefab;
    //public EnemySO enemyData;

}

public enum EnemyType { Melee, Ranged, Boss }

public enum EnemyState { Idle, Wander, Chase, Attack }
