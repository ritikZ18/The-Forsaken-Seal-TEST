using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LockOnCameraAdjust : MonoBehaviour
{
    public LockOnSystem lockOn;
    public CameraLogicBlock camLogic;
    public Vector3 lockOffset = new Vector3(0f, 1.4f, -3f);
    public Vector3 normalOffset = new Vector3(0f, 1.6f, -4f);
    public float transitionSpeed = 6f;

    void LateUpdate()
    {
        if (camLogic == null || lockOn == null) return;

        if (lockOn.currentTarget != null)
            camLogic.offset = Vector3.Lerp(camLogic.offset, lockOffset, Time.deltaTime * transitionSpeed);
        else
            camLogic.offset = Vector3.Lerp(camLogic.offset, normalOffset, Time.deltaTime * transitionSpeed);
    }
}
