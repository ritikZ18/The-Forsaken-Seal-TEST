using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float enemyhealth;
    public void TakeDamage(float damage)
    {
        enemyhealth -= damage;
        Debug.Log(enemyhealth);
    }

}
