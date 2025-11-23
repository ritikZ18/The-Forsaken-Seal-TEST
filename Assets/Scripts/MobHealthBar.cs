using UnityEngine;
using UnityEngine.UI;

public class MobHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MobHealth mobHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2.5f, 0);
    [SerializeField] private bool hideWhenFull = false; // Changed to false - always visible
    [SerializeField] private bool alwaysFaceCamera = true;

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color halfHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    private Camera mainCamera;
    private Canvas canvas;

    void Start()
    {
        mainCamera = Camera.main;

        // Get MobHealth if not assigned
        if (mobHealth == null)
        {
            mobHealth = GetComponentInParent<MobHealth>();
        }

        if (mobHealth == null)
        {
            Debug.LogError("MobHealth not found! Attach MobHealthBar to health bar UI and assign MobHealth.");
            enabled = false;
            return;
        }

        // Subscribe to health changes
        mobHealth.OnHealthChanged += UpdateHealthBar;
        mobHealth.OnDeath += OnMobDeath;

        // Initial update
        UpdateHealthBar(mobHealth.GetCurrentHealth());

        // Get canvas
        canvas = GetComponentInParent<Canvas>();
    }

    void OnDestroy()
    {
        if (mobHealth != null)
        {
            mobHealth.OnHealthChanged -= UpdateHealthBar;
            mobHealth.OnDeath -= OnMobDeath;
        }
    }

    void LateUpdate()
    {
        if (mobHealth == null) return;

        // Position above mob
        transform.position = mobHealth.transform.position + offset;

        // Always face camera
        if (alwaysFaceCamera && mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }

    void UpdateHealthBar(float currentHealth)
    {
        if (healthSlider == null) return;

        float healthPercent = mobHealth.GetHealthPercentage();
        healthSlider.value = healthPercent;

        // Update color based on health
        if (fillImage != null)
        {
            if (healthPercent > 0.5f)
            {
                fillImage.color = Color.Lerp(halfHealthColor, fullHealthColor, (healthPercent - 0.5f) * 2f);
            }
            else
            {
                fillImage.color = Color.Lerp(lowHealthColor, halfHealthColor, healthPercent * 2f);
            }
        }

        // Hide when full health
        if (hideWhenFull && canvas != null)
        {
            canvas.enabled = healthPercent < 1f;
        }
    }

    void OnMobDeath()
    {
        // Optionally hide health bar on death
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}

