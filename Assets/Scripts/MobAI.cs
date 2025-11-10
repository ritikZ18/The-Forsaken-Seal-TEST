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
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private bool dealDamageOnAnimationEvent = false;
    [SerializeField] private float attackWindup = 0.15f;

    private float nextAttackTime = 0f;
    private bool attacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.stoppingDistance = 0f;

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

        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        if (distance > stopDistance && !attacking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (!attacking && distance <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        attacking = true;
        animator.ResetTrigger(attackTrigger);
        animator.SetTrigger(attackTrigger);

        nextAttackTime = Time.time + attackCooldown;

        if (!dealDamageOnAnimationEvent)
        {
            yield return new WaitForSeconds(attackWindup);
            DealDamageIfValid();
        }

        yield return new WaitForSeconds(attackCooldown * 0.8f);
        attacking = false;
    }

    private void DealDamageIfValid()
    {
        if (!target) return;

        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= attackRange + 0.3f)
            {
                playerHealth.TakeDamage(damagePerHit);
                Debug.Log($"💥 Player took {damagePerHit} damage.");
            }
        }
    }
}
