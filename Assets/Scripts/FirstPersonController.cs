using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    [Tooltip("Desired jump height in meters")]
    public float jumpHeight = 1.2f;
    [Tooltip("Gravity should be negative; e.g., -9.81")]
    public float gravity = -9.81f;
    [Range(0f, 1f)] public float airControl = 0.5f;
    [Tooltip("How strongly we push the player down when grounded to stick to slopes")]
    public float groundStick = 2f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 1.5f;
    public Transform playerCamera;
    [Range(0f, 0.2f)] public float lookSmoothing = 0.03f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 currentLookVelocity;
    private float xRotation;
    private float verticalVelocity;
    private bool isSprinting;
    private bool jumpPressed;

    void OnEnable()
    {
        controller = GetComponent<CharacterController>();

        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();

        jumpAction.action.performed += _ => jumpPressed = true;
        jumpAction.action.canceled  += _ => jumpPressed = false;
        sprintAction.action.performed += _ => isSprinting = true;
        sprintAction.action.canceled  += _ => isSprinting = false;
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!playerCamera)
        {
            Camera cam = Camera.main;
            if (cam) playerCamera = cam.transform;
        }
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        lookInput = lookAction.action.ReadValue<Vector2>();

        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // Smooth the look input slightly to reduce jitter
        Vector2 target = lookInput * mouseSensitivity;
        Vector2 smoothed = Vector2.SmoothDamp(Vector2.zero, target, ref currentLookVelocity, lookSmoothing);

        float mouseX = smoothed.x;
        float mouseY = smoothed.y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (playerCamera) playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Build local input direction
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        input = input.sqrMagnitude > 1f ? input.normalized : input;

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Horizontal velocity (world space)
        Vector3 move = (transform.right * input.x + transform.forward * input.z);

        // Apply ground or air control
        if (!controller.isGrounded) move *= targetSpeed * airControl;
        else move *= targetSpeed;

        // Gravity & Jump
        if (controller.isGrounded)
        {
            // keep player grounded to slopes
            if (verticalVelocity < 0f) verticalVelocity = -groundStick;

            if (jumpPressed)
            {
                // v = sqrt(2 * h * -g)
                verticalVelocity = Mathf.Sqrt(2f * jumpHeight * -gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply vertical separately (DON’T scale by speed!)
        Vector3 velocity = move;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
