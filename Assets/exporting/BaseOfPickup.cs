using UnityEngine;

public abstract class BaseOfPickups : MonoBehaviour
{
    protected abstract void OnPickup(PlayerHealth player);

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            Debug.Log("Pickup collected!");
            OnPickup(player);
            Destroy(gameObject);
        }
    }
}