using UnityEngine;

public class CameraLogicBlock : MonoBehaviour
{
    [SerializeField] Transform tget;
    [SerializeField] public Vector3 offset = new Vector3(0, 1.6f, -4f);
    [SerializeField] float sensitivity = 120f;
    [SerializeField] float mP = -20f, maxP = 70f;
    [SerializeField] bool cursLock = true;

    float yaw, pitch;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y; pitch = Mathf.Clamp(angles.x, mP, maxP);
        if (cursLock) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, mP, maxP);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = tget.position + rot * offset;
        transform.position = desiredPos;
        transform.LookAt(tget.position + Vector3.up * 1.2f);
    }

}