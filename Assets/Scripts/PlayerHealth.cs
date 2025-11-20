using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider healthBar;
    public Text healthText;

    [Header("Death Settings")]
    public float respawnDelay = 3f;
    public Vector3 respawnPosition;

    public bool isDead = false;
    public System.Action<int, int> OnHealthChanged;
    public System.Action OnPlayerDeath;
    public System.Action OnPlayerRespawn;
    public int startingHealth = 5;
    public HUDController hud;

    void Start()
    {
        currentHealth = startingHealth;

        if (respawnPosition == Vector3.zero)
            respawnPosition = transform.position;

    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);


        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        hud.SetHealth(maxHealth, currentHealth);

        Debug.Log($"Player healed {healAmount}. Health: {currentHealth}/{maxHealth}");
    }

    void Die()
    {
        GetComponent<ControllerPerson>().enabled = false;
        hud.ShowDeathScreen();
    }



    public bool IsDead => isDead;
    public float HealthPercentage => (float)currentHealth / maxHealth;
}