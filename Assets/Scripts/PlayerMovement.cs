using UnityEngine;

/// <summary>
/// Simple, optimized Rigidbody-based FPS/TPS Player Movement script.
/// Supports walking, running, jumping, and crouching without CharacterController.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float crouchSpeed = 3f;
    public float jumpForce = 7f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public float maxLookX = 80f;

    [Header("Crouch Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private float rotX;
    private bool isGrounded;
    private bool isCrouching;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true; // prevents physics tilt
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        CameraLook();
        HandleJump();
        HandleCrouch();
    }

    void Move()
    {
        float t_hMove = Input.GetAxisRaw("Horizontal");
        float t_vMove = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        Vector3 moveDir = (transform.forward * t_vMove + transform.right * t_hMove).normalized;
        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        Vector3 moveVelocity = moveDir * currentSpeed;
        Vector3 rbVelocity = rb.linearVelocity;

        // preserve vertical velocity (gravity/jump)
        rb.linearVelocity = new Vector3(moveVelocity.x, rbVelocity.y, moveVelocity.z);
    }

    void CameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -maxLookX, maxLookX);

        playerCamera.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            capsule.height = isCrouching ? crouchHeight : defaultHeight;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Simple grounded check
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 45f)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
