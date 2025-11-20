//using UnityEngine;
//using UnityEngine.AI;
//using System.Collections;

//public class HazmatChaseAI : MonoBehaviour
//{
//    [Header("References")]
//    [Tooltip("Assign your Player root here. If empty, tries tag 'Player', then Camera.main.root.")]
//    public Transform player;

//    [Tooltip("Optional: Camera reference, only used as a fallback to find player root.")]
//    public Transform playerCamera;

//    // Auto-resolved at runtime
//    private Transform playerRoot;
//    private IHealth playerHealth;
//    private Animator anim;
//    private NavMeshAgent agent;

//    [Header("Detection")]
//    [Tooltip("Boss wakes up and starts chasing when the player enters this radius.")]
//    public float detectionRadius = 8f;

//    [Tooltip("Boss goes back to idle when the player is farther than this. Keep slightly bigger than detection to avoid flicker.")]
//    public float disengageRadius = 10f;

//    private bool engaged = false;   // false = idle, true = chasing/attacking

//    [Header("Movement Settings")]
//    public float runSpeed = 4.5f;     // applied to NavMeshAgent
//    public float rotationSpeed = 5f;  // slerp speed when facing player

//    [Header("Attack Settings")]
//    public float attackRange = 2.2f;    // distance from player ROOT to attack
//    public float attackCooldown = 2f;   // seconds between attacks
//    public float attackHitTime = 0.6f;  // when the hit lands if no animation event

//    private bool isAttacking = false;
//    private float lastAttackTime = -999f;

//    void Awake()
//    {
//        anim = GetComponentInChildren<Animator>();
//        agent = GetComponent<NavMeshAgent>();
//    }

//    void Start()
//    {
//        // Camera fallback
//        if (!playerCamera && Camera.main) playerCamera = Camera.main.transform;

//        // Resolve player root
//        if (!player)
//        {
//            var tagged = GameObject.FindWithTag("Player");
//            if (tagged) player = tagged.transform;
//            else if (playerCamera) player = playerCamera.root;
//        }
//        playerRoot = player;

//        // Health lookup
//        if (playerRoot) playerHealth = playerRoot.GetComponentInChildren<IHealth>();

//        if (!playerRoot)
//            Debug.LogError("HazmatChaseAI: No player root found. Assign 'player' or tag your player as 'Player'.");
//        if (playerHealth == null)
//            Debug.LogWarning("HazmatChaseAI: No IHealth found on player.");

//        if (agent)
//        {
//            agent.speed = runSpeed;
//            agent.stoppingDistance = Mathf.Max(0.1f, attackRange * 0.9f);
//            agent.updateRotation = false; // we'll rotate manually
//            agent.updatePosition = true;  // agent drives position
//            agent.isStopped = true;       // start truly idle
//        }

//        if (anim)
//        {
//            anim.applyRootMotion = false; // let agent move the character
//            anim.SetFloat("Speed", 0f);
//            anim.ResetTrigger("Attack");
//        }

//        // Start idle
//        SetIdle();
//    }

//    void OnDisable()
//    {
//        // Ensure idle if script gets disabled
//        HardStopAgent();
//        if (anim) anim.SetFloat("Speed", 0f);
//        engaged = false;
//        isAttacking = false;
//    }

//    void Update()
//    {
//        if (!playerRoot) return;

//        float distance = Vector3.Distance(transform.position, playerRoot.position);

//        // Engage/disengage based on proximity
//        if (!engaged)
//        {
//            if (distance <= detectionRadius)
//            {
//                engaged = true;
//            }
//            else
//            {
//                // stay idle
//                SetIdle();
//                return;
//            }
//        }
//        else // engaged
//        {
//            if (distance > disengageRadius)
//            {
//                engaged = false;
//                SetIdle();
//                return;
//            }
//        }

//        // If attacking, stay put but face the player
//        if (isAttacking)
//        {
//            IdleFacePlayer();
//            return;
//        }

//        // Attack if in range, otherwise chase
//        if (distance <= attackRange)
//        {
//            if (Time.time >= lastAttackTime + attackCooldown)
//                StartCoroutine(AttackSequence());
//            else
//                IdleFacePlayer();
//        }
//        else
//        {
//            ChasePlayer();
//        }
//    }

//    void SetIdle()
//    {
//        // Fully idle: stop agent & idle anim
//        HardStopAgent();
//        if (anim) anim.SetFloat("Speed", 0f);
//    }

//    void HardStopAgent()
//    {
//        if (agent && agent.isOnNavMesh)
//        {
//            agent.isStopped = true;
//            agent.ResetPath();
//            agent.velocity = Vector3.zero;
//        }
//    }

//    void AllowAgentMove()
//    {
//        if (agent && agent.isOnNavMesh)
//        {
//            agent.isStopped = false;
//        }
//    }

//    void ChasePlayer()
//    {
//        if (!playerRoot) return;

//        AllowAgentMove();

//        if (agent && agent.isOnNavMesh)
//        {
//            agent.SetDestination(playerRoot.position);

//            // Drive animation by actual movement amount (prevents run-in-place)
//            float moveSpeed = agent.velocity.magnitude; // 0..runSpeed
//            if (anim) anim.SetFloat("Speed", moveSpeed);
//        }
//        else
//        {
//            // Fallback simple movement if no navmesh/agent
//            Vector3 dir = (playerRoot.position - transform.position);
//            dir.y = 0f;
//            if (dir.sqrMagnitude > 0.0001f)
//            {
//                dir.Normalize();
//                transform.position += dir * runSpeed * Time.deltaTime;
//                if (anim) anim.SetFloat("Speed", runSpeed);
//            }
//            else
//            {
//                if (anim) anim.SetFloat("Speed", 0f);
//            }
//        }

//        FaceTowards(playerRoot.position);
//    }

//    IEnumerator AttackSequence()
//    {
//        isAttacking = true;
//        lastAttackTime = Time.time;

//        HardStopAgent(); // stop in place to strike

//        if (anim)
//        {
//            anim.SetFloat("Speed", 0f);
//            anim.SetTrigger("Attack");
//        }

//        // Face player before striking
//        FaceTowards(playerRoot.position, instant: true);

//        // If you use an Animation Event calling AnimEvent_PerformAttack(), you can remove this wait.
//        yield return new WaitForSeconds(attackHitTime);
//        PerformAttack();

//        // Finish cooldown window
//        float remaining = Mathf.Max(0f, (lastAttackTime + attackCooldown) - Time.time);
//        if (remaining > 0f) yield return new WaitForSeconds(remaining);

//        isAttacking = false;
//    }

//    // (Optional) Hook this with an Animation Event on your attack clip.
//    public void AnimEvent_PerformAttack()
//    {
//        PerformAttack();
//    }

//    void PerformAttack()
//    {
//        if (!playerRoot) return;

//        float dist = Vector3.Distance(transform.position, playerRoot.position);
//        if (dist <= attackRange + 0.5f)
//        {
//            if (playerHealth != null)
//                playerHealth.ApplyDamage(10);
//        }
//    }

//    void IdleFacePlayer()
//    {
//        if (!playerRoot) return;
//        FaceTowards(playerRoot.position);
//        if (anim) anim.SetFloat("Speed", 0f);
//    }

//    void FaceTowards(Vector3 worldTarget, bool instant = false)
//    {
//        Vector3 face = worldTarget - transform.position;
//        face.y = 0f;
//        if (face.sqrMagnitude < 0.0001f) return;

//        Quaternion targetRot = Quaternion.LookRotation(face);
//        if (instant) transform.rotation = targetRot;
//        else transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
//    }

//    void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, detectionRadius);
//        Gizmos.color = new Color(1f, 0.5f, 0f);
//        Gizmos.DrawWireSphere(transform.position, disengageRadius);
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//    }
//}
