using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Generic melee enemy AI with three states:
///
/// 1. Wander  - walks to a random nearby point, pauses, repeats.
/// 2. Chase   - triggered by Enemy_Health's OnDamaged event; paths
///              toward the player, and attacks (on a cooldown) once
///              close enough, playing an attack animation.
/// 3. Return  - if the player runs too far from this enemy's spawn
///              point (leashRadius), it gives up chasing and walks
///              back home, then resumes wandering.
///
/// Also drives the rigged mesh's Animator "Speed" float from the
/// NavMeshAgent's current velocity, so walk/idle animations blend
/// automatically based on actual movement rather than AI state.
///
/// Attach this to any enemy that should share this same behavior -
/// not specific to any one creature.
/// </summary>
[RequireComponent(typeof(Enemy_Health))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_AI : MonoBehaviour
{
    private enum State { Wander, Chase, Return }

    [Header("References")]
    [SerializeField] private Transform player;
    private Player_Health playerHealth;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float stoppingDistance = 1.5f; // don't walk into the player
    [SerializeField] private float destinationUpdateInterval = 0.2f; // how often to re-aim at the player

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackCooldown = 1.5f;
    private float nextAttackTime;

    [Header("Leash Settings")]
    [Tooltip("If the enemy strays this far from its spawn point while chasing, it gives up and returns home.")]
    [SerializeField] private float leashRadius = 12f;

    [Header("Wander Settings")]
    [SerializeField] private float wanderSpeed = 1.5f;
    [SerializeField] private float wanderRadius = 6f; // how far from spawn it'll wander
    [SerializeField] private float minPauseTime = 1.5f;
    [SerializeField] private float maxPauseTime = 4f;

    private Enemy_Health health;
    private NavMeshAgent agent;
    private Animator animator; // lives on the rigged mesh, usually a child object
    private State state = State.Wander;
    private Vector3 spawnPosition;
    private float nextDestinationUpdateTime;
    private float pauseUntilTime;
    private bool isPausing;

    private void Awake()
    {
        health = GetComponent<Enemy_Health>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        spawnPosition = transform.position;

        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = true; // let the agent handle facing its movement direction

        if (player == null)
        {
            // Fallback if not assigned in the Inspector - finds the
            // object tagged "Player". Make sure your Player GameObject
            // has that tag set, or assign this field manually instead.
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<Player_Health>();
        }
    }

    private void OnEnable()
    {
        health.OnDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        health.OnDamaged -= OnDamaged;
    }

    /// <summary>
    /// Called via Enemy_Health's event whenever this enemy takes
    /// damage. Interrupts wandering/pausing and starts the chase.
    /// </summary>
    private void OnDamaged()
    {
        state = State.Chase;
        isPausing = false;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Wander:
                UpdateWander();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Return:
                UpdateReturn();
                break;
        }

        UpdateAnimator();
    }

    /// <summary>
    /// Feeds the NavMeshAgent's current speed into the Animator's
    /// "Speed" float, so the Idle/Walk transitions react to actual
    /// movement rather than needing any manual state syncing.
    /// </summary>
    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);
    }

    private void UpdateWander()
    {
        agent.speed = wanderSpeed;

        if (isPausing)
        {
            if (Time.time >= pauseUntilTime)
            {
                isPausing = false;
                PickNewWanderDestination();
            }
            return;
        }

        // Reached the current wander destination -> pause before picking a new one
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isPausing = true;
            pauseUntilTime = Time.time + Random.Range(minPauseTime, maxPauseTime);
        }
    }

    private void PickNewWanderDestination()
    {
        // Random point within wanderRadius of the spawn position, then
        // snapped onto the NavMesh so it's always a valid destination.
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomPoint = spawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void UpdateChase()
    {
        agent.speed = chaseSpeed;

        // Give up and head home if the player got too far away
        float distanceFromSpawn = Vector3.Distance(transform.position, spawnPosition);
        if (distanceFromSpawn > leashRadius)
        {
            state = State.Return;
            agent.SetDestination(spawnPosition);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Close enough to attack - stop moving and try to attack
            agent.ResetPath();
            TryAttack();
        }
        else if (Time.time >= nextDestinationUpdateTime)
        {
            agent.SetDestination(player.position);
            nextDestinationUpdateTime = Time.time + destinationUpdateInterval;
        }
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return; // still on cooldown
        }

        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tried to attack but found no Player_Health on the player.");
        }
    }

    private void UpdateReturn()
    {
        agent.speed = wanderSpeed;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            state = State.Wander;
            isPausing = true;
            pauseUntilTime = Time.time + Random.Range(minPauseTime, maxPauseTime);
        }
    }
}