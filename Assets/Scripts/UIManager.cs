using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthBar;
    public Text healthText;
    public Image healthBarFill;
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.red;
    
    [Header("Game UI")]
    public Text scoreText;
    public Text waveText;
    public Text enemiesKilledText;
    
    [Header("Crosshair")]
    public Image crosshair;
    public Color crosshairColor = Color.white;
    
    [Header("Damage Effect")]
    public Image damageOverlay;
    public float damageFlashDuration = 0.2f;
    public Color damageFlashColor = new Color(1f, 0f, 0f, 0.3f);
    
    private PlayerHealth playerHealth;
    private GameManager gameManager;
    private bool isFlashingDamage = false;
    
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        gameManager = GameManager.Instance;
        
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
            playerHealth.OnPlayerDeath += OnPlayerDeath;
            playerHealth.OnPlayerRespawn += OnPlayerRespawn;
        }
        
        if (gameManager != null)
        {
            gameManager.OnScoreChanged += UpdateScoreUI;
            gameManager.OnWaveChanged += UpdateWaveUI;
        }
        
        InitializeUI();
    }
    
    void InitializeUI()
    {
        if (crosshair != null)
        {
            crosshair.color = crosshairColor;
        }
        
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(0f, 0f, 0f, 0f);
        }
        
        if (healthBarFill != null)
        {
            healthBarFill.color = healthyColor;
        }
    }
    
    void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
        
        if (healthBarFill != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            healthBarFill.color = Color.Lerp(damagedColor, healthyColor, healthPercentage);
        }
        
        if (currentHealth < maxHealth && !isFlashingDamage)
        {
            StartCoroutine(FlashDamageEffect());
        }
    }
    
    System.Collections.IEnumerator FlashDamageEffect()
    {
        isFlashingDamage = true;
        
        if (damageOverlay != null)
        {
            damageOverlay.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            
            float fadeTime = 0.5f;
            float elapsed = 0f;
            Color startColor = damageFlashColor;
            Color endColor = new Color(0f, 0f, 0f, 0f);
            
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                damageOverlay.color = Color.Lerp(startColor, endColor, elapsed / fadeTime);
                yield return null;
            }
            
            damageOverlay.color = endColor;
        }
        
        isFlashingDamage = false;
    }
    
    void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    void UpdateWaveUI(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {wave}";
        }
    }
    
    void OnPlayerDeath()
    {
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(false);
        }
    }
    
    void OnPlayerRespawn()
    {
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(true);
        }
    }
    
    void Update()
    {
        if (enemiesKilledText != null && gameManager != null)
        {
            enemiesKilledText.text = $"Enemies Killed: {gameManager.enemiesKilled}";
        }
    }
    
    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
            playerHealth.OnPlayerDeath -= OnPlayerDeath;
            playerHealth.OnPlayerRespawn -= OnPlayerRespawn;
        }
        
        if (gameManager != null)
        {
            gameManager.OnScoreChanged -= UpdateScoreUI;
            gameManager.OnWaveChanged -= UpdateWaveUI;
        }
    }
}
