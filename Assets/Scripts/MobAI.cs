using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MobAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;

    [Header("Target Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float stopDistance = 2.5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;  // How far mob can see player
    [SerializeField] private float patrolSpeed = 2f;      // Speed while patrolling
    [SerializeField] private float chaseSpeed = 5f;       // Speed while chasing

    [Header("Patrol Settings")]
    [SerializeField] private bool shouldPatrol = true;    // Enable/disable patrol
    [SerializeField] private float patrolDistance = 5f;   // How far to walk side to side
    [SerializeField] private float patrolWaitTime = 2f;   // Wait time at patrol points

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2.7f;
    [SerializeField] private int smallSwingDamage = 1;
    [SerializeField] private int bigSwingDamage = 3;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private string attackTrigger = "Attack";

    private float nextAttackTime = 0f;
    private bool attacking = false;

    // Patrol state
    private enum MobState { Patrolling, Chasing, Attacking }
    private MobState currentState = MobState.Patrolling;
    private Vector3 patrolPointA;
    private Vector3 patrolPointB;
    private Vector3 currentPatrolTarget;
    private float patrolWaitTimer = -1f; // Start negative so it begins moving immediately
    private Vector3 startPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.stoppingDistance = stopDistance;
        agent.speed = patrolSpeed;

        // Store starting position
        startPosition = transform.position;

        // Set up patrol points (left and right from start position)
        Vector3 right = transform.right;
        Vector3 desiredPointA = startPosition - right * patrolDistance;
        Vector3 desiredPointB = startPosition + right * patrolDistance;

        // Sample NavMesh to ensure points are valid
        UnityEngine.AI.NavMeshHit hitA, hitB;
        if (UnityEngine.AI.NavMesh.SamplePosition(desiredPointA, out hitA, patrolDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            patrolPointA = hitA.position;
        }
        else
        {
            patrolPointA = startPosition; // Fallback to start
            Debug.LogWarning("⚠️ Patrol Point A not on NavMesh! Using start position.");
        }

        if (UnityEngine.AI.NavMesh.SamplePosition(desiredPointB, out hitB, patrolDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            patrolPointB = hitB.position;
        }
        else
        {
            patrolPointB = startPosition; // Fallback to start
            Debug.LogWarning("⚠️ Patrol Point B not on NavMesh! Using start position.");
        }

        currentPatrolTarget = patrolPointB;


        var playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning("⚠️ No GameObject tagged 'Player' found!");
    }

    void Update()
    {
        if (!target) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // State machine
        switch (currentState)
        {
            case MobState.Patrolling:
                HandlePatrolling(distanceToPlayer);
                break;
            case MobState.Chasing:
                HandleChasing(distanceToPlayer);
                break;
            case MobState.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
        }

        // Update animator
        float normalizedSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", normalizedSpeed);
    }

    void HandlePatrolling(float distanceToPlayer)
    {
        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            currentState = MobState.Chasing;
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            return;
        }

        // Continue patrol
        if (!shouldPatrol)
        {
            agent.isStopped = true;
            return;
        }

        float distanceToPatrolPoint = Vector3.Distance(transform.position, currentPatrolTarget);

        if (distanceToPatrolPoint < 0.5f)
        {
            // Reached patrol point, wait then switch
            if (patrolWaitTimer > 0)
            {
                // Currently waiting
                agent.isStopped = true;
                patrolWaitTimer -= Time.deltaTime;
            }
            else
            {
                // Wait is over, switch direction
                currentPatrolTarget = (currentPatrolTarget == patrolPointA) ? patrolPointB : patrolPointA;
                patrolWaitTimer = patrolWaitTime; // Reset timer for next wait
                agent.isStopped = false;
                agent.SetDestination(currentPatrolTarget);
            }
        }
        else
        {
            // Move to patrol point
            agent.isStopped = false;
            agent.SetDestination(currentPatrolTarget);

            // Rotate toward patrol direction
            Vector3 direction = (currentPatrolTarget - transform.position).normalized;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
            }
        }
    }

    void HandleChasing(float distanceToPlayer)
    {
        // Once engaged, NEVER go back to patrol - chase endlessly
        
        // Rotate toward player
        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        // Move toward player
        if (distanceToPlayer > stopDistance && !attacking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
        }

        // Trigger attack when in range
        if (!attacking && distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    void HandleAttacking(float distanceToPlayer)
    {
        // Once engaged, NEVER go back to patrol - attack endlessly
        
        // Face player
        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        agent.isStopped = true;

        // Attack
        if (!attacking && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (target == null) yield break;

        attacking = true;

        // Face player
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        // Trigger attack animation
        animator.SetTrigger(attackTrigger);

        // Cooldown setup
        nextAttackTime = Time.time + attackCooldown;

        // Wait for cooldown before attacking again
        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }

    // Called by Animation Events at each swing impact
    public void OnSmallSwing1()
    {
        DealDamage(smallSwingDamage);
    }

    public void OnSmallSwing2()
    {
        DealDamage(smallSwingDamage);
    }

    public void OnBigSwing()
    {
        DealDamage(bigSwingDamage);
    }

    private void DealDamage(int damage)
    {
        if (!target) return;

        // Check if player is in range
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange + 0.5f)
        {
            return;
        }

        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = target.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // Visualize detection and patrol in editor
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Patrol points (green)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(patrolPointA, 0.5f);
            Gizmos.DrawSphere(patrolPointB, 0.5f);
            Gizmos.DrawLine(patrolPointA, patrolPointB);
            
            // Current target
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, currentPatrolTarget);
        }
        else
        {
            // Show patrol preview in edit mode
            Vector3 right = transform.right;
            Vector3 pointA = transform.position - right * patrolDistance;
            Vector3 pointB = transform.position + right * patrolDistance;
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA, 0.5f);
            Gizmos.DrawSphere(pointB, 0.5f);
            Gizmos.DrawLine(pointA, pointB);
        }
    }

}
