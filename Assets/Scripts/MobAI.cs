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

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2.7f;
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private float attackWindup = 0.15f;

    private float nextAttackTime = 0f;
    private bool attacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.stoppingDistance = stopDistance;

        var playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning("⚠️ No GameObject tagged 'Player' found!");
    }

    void Update()
    {
        if (!target) return;

        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        // Rotate toward player
        if (distance > 0.1f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        // Move or stop
        if (distance > stopDistance && !attacking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
        }

        float normalizedSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", normalizedSpeed);
        // Attack when in range
        if (!attacking && distance <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (target == null) yield break;

        attacking = true;
        agent.isStopped = true;

        transform.LookAt(target.position); // Face player
        animator.ResetTrigger(attackTrigger);
        animator.SetTrigger(attackTrigger);

        yield return new WaitForSeconds(attackWindup);

        DealDamageIfValid();

        nextAttackTime = Time.time + attackCooldown;
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }

    private void DealDamageIfValid()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange + 0.2f)
        {
            var playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerHit);
                Debug.Log("Player took " + damagePerHit + " damage.");
            }
        }
    }
}
