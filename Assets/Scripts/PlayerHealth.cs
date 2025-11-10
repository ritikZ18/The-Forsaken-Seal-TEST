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
    
    private bool isDead = false;
    
    public System.Action<int, int> OnHealthChanged;
    public System.Action OnPlayerDeath;
    public System.Action OnPlayerRespawn;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (respawnPosition == Vector3.zero)
            respawnPosition = transform.position;
            
        UpdateHealthUI();
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        UpdateHealthUI();
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        UpdateHealthUI();
        
        Debug.Log($"Player healed {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player died!");
        
        
        OnPlayerDeath?.Invoke();
        
        Invoke(nameof(Respawn), respawnDelay);
    }
    
    void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        
        transform.position = respawnPosition;
        
        
        UpdateHealthUI();
        
        OnPlayerRespawn?.Invoke();
        
        Debug.Log("Player respawned!");
    }
    
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
    
    public bool IsDead => isDead;
    public float HealthPercentage => (float)currentHealth / maxHealth;
}
