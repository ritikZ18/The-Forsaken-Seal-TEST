//using UnityEngine;

//public class EnemyProjectile : MonoBehaviour
//{
//    public float damage = 20f;
//    public float lifeTime = 5f;

//    private void Start()
//    {
//        Destroy(gameObject, lifeTime); // Auto destroy after time
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
//            if (playerHealth != null)
//            {
//                playerHealth.TakeDamage(damage);
//            }
//            Destroy(gameObject); // Destroy projectile after hit
//        }

//        // Optional: Destroy on ground impact
//        if (other.CompareTag("Ground"))
//        {
//            Destroy(gameObject);
//        }
//    }
//}
