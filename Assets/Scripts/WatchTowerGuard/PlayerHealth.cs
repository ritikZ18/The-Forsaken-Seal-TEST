//using UnityEngine;
//using UnityEngine.UI; // Only if using UI health bar

//public class PlayerHealth : MonoBehaviour
//{
//    [Header("Player Health Settings")]
//    public float maxHealth = 100f;
//    public float currentHealth;

//    [Header("UI (Optional)")]
//    public Slider healthSlider;  // Drag your UI health bar here
//    public bool useHealthUI = false;

//    private Animator animator;   // If you have a death animation

//    void Start()
//    {
//        currentHealth = maxHealth;
//        animator = GetComponent<Animator>();

//        if (useHealthUI && healthSlider != null)
//        {
//            healthSlider.maxValue = maxHealth;
//            healthSlider.value = currentHealth;
//        }
//    }

//    public void TakeDamage(float damage)
//    {
//        currentHealth -= damage;

//        if (useHealthUI && healthSlider != null)
//        {
//            healthSlider.value = currentHealth;
//        }

//        if (currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    void Die()
//    {
//        Debug.Log("Player Died!");

//        if (animator != null)
//            animator.SetTrigger("Die"); // If you have a death animation

//        // Disable movement or other scripts
//        GetComponent<CharacterController>().enabled = false;  // or your movement script
//        GetComponent<PlayerMovement>().enabled = false;       // Example script name

//        // Optionally reload scene or show game over
//        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }
//}
