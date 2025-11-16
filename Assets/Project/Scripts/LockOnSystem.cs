using UnityEngine;
using System.Collections.Generic;

public class LockOnSystem : MonoBehaviour

{
    bool debugKeyShown = false;

    [Header("Lock settings")]
    public KeyCode toggleKey = KeyCode.Mouse2;
    public float maxLockRange = 18f;
    public float maxAngleFromForward = 80f;
    public LayerMask occlusionMask = ~0; // which layers block line of sight

    [Header("References")]
    public Camera cam;
    public Transform currentTarget; // read from other scripts

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // Debug: show assigned key so we can see what's expected
        // (only once per play session)
        if (!debugKeyShown)
        {
            Debug.Log($"[LockOnSystem] toggleKey = {toggleKey}. Middle mouse test also enabled.");
            debugKeyShown = true;
        }

        bool pressed = false;

        // 1) Old Input API: check KeyCode (e.g. KeyCode.Mouse2) OR direct mouse button index 2
        if (Input.GetKeyDown(toggleKey))
        {
            pressed = true;
            Debug.Log("[LockOnSystem] Input.GetKeyDown(toggleKey) detected.");
        }
        else if (Input.GetMouseButtonDown(2)) // fallback to direct mouse button index 2 (middle)
        {
            pressed = true;
            Debug.Log("[LockOnSystem] Input.GetMouseButtonDown(2) detected (middle mouse).");
        }

        // 2) If using the new Input System, Input.GetKeyDown won't fire; detect that and warn
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    // If project uses new Input System only, above checks may not work.
    // We'll warn so you can switch to the Input System API or enable "Both" in Player Settings.
    if (!pressed)
    {
        Debug.LogWarning("[LockOnSystem] No Input detected. If you use the new Input System (Package), enable 'Both' in Project Settings > Player > Active Input Handling or update this script to use the new Input System API.");
    }
#endif

        if (!pressed) return;

        // If we get here, we pressed the lock button: toggle lock
        if (currentTarget == null)
        {
            Debug.Log("[LockOnSystem] Attempting to find and lock target...");
            FindAndLockTarget();
            if (currentTarget != null) Debug.Log($"[LockOnSystem] Locked to: {currentTarget.name}");
            else Debug.Log("[LockOnSystem] No valid target found (range/angle/occlusion).");
        }
        else
        {
            Debug.Log("[LockOnSystem] Clearing lock.");
            ClearLock();
        }
    }


    void FindAndLockTarget()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform best = null;
        float bestScore = Mathf.Infinity;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        foreach (var e in enemies)
        {
            if (e == null) continue;
            Vector3 toEnemy = e.transform.position - transform.position;
            float dist = toEnemy.magnitude;
            if (dist > maxLockRange) continue;

            // occlusion (raycast from player head height)
            Vector3 origin = transform.position + Vector3.up * 1.2f;
            Vector3 dir = (e.transform.position - origin).normalized;
            if (Physics.Raycast(origin, dir, dist, occlusionMask)) continue;

            Vector3 screenPos = cam.WorldToScreenPoint(e.transform.position);
            if (screenPos.z < 0) continue;

            float angleFromForward = Vector3.Angle(transform.forward, toEnemy);
            if (angleFromForward > maxAngleFromForward) continue;

            float screenDist = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), screenCenter);
            if (screenDist < bestScore)
            {
                bestScore = screenDist;
                best = e.transform;
            }
        }

        if (best != null) currentTarget = best;
    }

    void FindAdjacentTarget(int dir)
    {
        var list = new List<Transform>();
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy")) list.Add(e.transform);
        if (list.Count == 0) return;

        list.Sort((a, b) =>
        {
            var sa = cam.WorldToScreenPoint(a.position);
            var sb = cam.WorldToScreenPoint(b.position);
            return sa.x.CompareTo(sb.x);
        });

        int idx = list.IndexOf(currentTarget);
        if (idx == -1) currentTarget = list[0];
        else currentTarget = list[(idx + dir + list.Count) % list.Count];
    }

    public void ClearLock()
    {
        currentTarget = null;
    }
}
