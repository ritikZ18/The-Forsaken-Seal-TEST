using UnityEngine;

[RequireComponent(typeof(ControllerPerson))]
public class PlayerLockOnController : MonoBehaviour
{
    public LockOnSystem lockOn;
    public float rotationSpeed = 10f;

    ControllerPerson controller;
    Animator animator;

    void Awake()
    {
        controller = GetComponent<ControllerPerson>();
        animator = GetComponentInChildren<Animator>();
        if (lockOn == null) lockOn = GetComponent<LockOnSystem>();
    }

    void Update()
    {
        if (lockOn == null) return;

        bool locked = lockOn.currentTarget != null;
        // tell animator the lock state
        if (animator != null) animator.SetBool("LockOn", locked);

        if (!locked)
        {
            // ensure controller resets flags
            controller.useLockOnStrafing = false;
            controller.lockOnTarget = null;
            return;
        }

        // rotate player to face the target smoothly (Y only)
        Vector3 dir = lockOn.currentTarget.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        // enable strafing behaviour in the movement controller
        controller.useLockOnStrafing = true;
        controller.lockOnTarget = lockOn.currentTarget;
    }
}
