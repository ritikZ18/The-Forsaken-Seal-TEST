using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    public void OnTriggerEnter(Collider other)
    {
        if (other == null) return;


        if (!other.CompareTag("Enemy") && !other.transform.root.CompareTag("Enemy"))
            return;


        EnemyHealthSystem enemy = other.GetComponent<EnemyHealthSystem>();
        if (enemy == null)
            enemy = other.GetComponentInParent<EnemyHealthSystem>();

        if (enemy == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"DealDamage: Hit '{other.name}' (tag={other.tag}) but no EnemyHealthSystem found on it or parents.", other);
#endif
            return;
        }


        enemy.TakeDamage(damage);
    }
}
