using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControllerPerson : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float speedMeasure = 5f;
    [SerializeField] float timeSmooth = 0.1f;

    [Header("Jump/Gravity")]
    [SerializeField] float heightOfJump = 1.6f;
    [SerializeField] float gravity = -20f;

    [Header("References")]
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public Animator playerAnim;  // <-- Reference to Wukong Animator

    // Lock-on strafing fields
    [HideInInspector] public bool useLockOnStrafing = false;
    [HideInInspector] public Transform lockOnTarget = null;
    [SerializeField] float strafeSpeed = 3.8f;

    CharacterController controller;
    float turnSmoothVelocity;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v);
        float inputMagnitude = input.magnitude;

        bool isMoving = inputMagnitude >= 0.1f;

        Vector3 moveDir = Vector3.zero;
        if (isMoving)
        {
            if (useLockOnStrafing && lockOnTarget != null)
            {
                // In lock-on we strafe relative to player orientation (player faces target)
                Vector3 localInput = new Vector3(h, 0f, v).normalized;

                // Convert local input to world space using player's local axes
                Vector3 worldMove = transform.right * localInput.x + transform.forward * localInput.z;
                controller.Move(worldMove * strafeSpeed * Time.deltaTime);

                // Update animator params for strafing if you use them
                playerAnim.SetFloat("MoveX", h);
                playerAnim.SetFloat("MoveZ", v);
            }
            else
            {
                float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, timeSmooth);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speedMeasure * Time.deltaTime);

                // Reset strafing animator params
                playerAnim.SetFloat("MoveX", 0f);
                playerAnim.SetFloat("MoveZ", 0f);
            }
        }
        else
        {
            // not moving
            playerAnim.SetFloat("MoveX", 0f);
            playerAnim.SetFloat("MoveZ", 0f);
        }

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(heightOfJump * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        // ---- ANIMATION ----
        // Keep your existing booleans for basic directions (optional with lock-on)
        playerAnim.SetBool("isWalking", v > 0);
        playerAnim.SetBool("isWalkingBack", v < 0);
        playerAnim.SetBool("isWalkingLeft", h < 0);
        playerAnim.SetBool("isWalkingRight", h > 0);
    }
}
