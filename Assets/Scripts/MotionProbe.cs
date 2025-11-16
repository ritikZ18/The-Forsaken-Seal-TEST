using UnityEngine;
using UnityEngine.AI;

public class MotionProbe : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.destination = GameObject.FindWithTag("Player").transform.position;
        Debug.Log($"Agent: speed={agent.speed}, accel={agent.acceleration}, updatePos={agent.updatePosition}");
    }

    void Update()
    {
        float v = agent.velocity.magnitude;
        anim.SetFloat("Speed", v);
        Debug.Log($"Frame {Time.frameCount} → Vel:{v:F2}, Pos:{transform.position:F2}");
    }
}
