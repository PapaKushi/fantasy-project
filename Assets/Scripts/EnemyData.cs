using UnityEngine;

/// <summary>
/// Stats for one enemy type (health, movement, combat). Create one
/// asset per enemy type (e.g. "Boar_Data", "Wolf_Data") and assign it
/// to that enemy's Enemy_AI and Enemy_Health components, so a new
/// enemy is just a new data asset rather than a script/prefab copy.
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Tooltip("Stable internal ID, used to match KillEnemy quests. Not shown to the player - use enemyName for that.")]
    public string enemyID;
    public string enemyName;

    [Header("Health")]
    public int maxHealth = 30;

    [Header("Movement")]
    public float wanderSpeed = 1.5f;
    public float chaseSpeed = 3.5f;
    public float stoppingDistance = 1.5f;
    public float wanderRadius = 6f;
    public float leashRadius = 12f;
    public float minPauseTime = 1.5f;
    public float maxPauseTime = 4f;
    public float destinationUpdateInterval = 0.2f;

    [Header("Combat")]
    public float attackRange = 2f;
    public int attackDamage = 5;
    public float attackCooldown = 1.5f;
}