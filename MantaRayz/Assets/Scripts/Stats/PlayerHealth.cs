using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        // Ininitialize health
        currentHealth = maxHealth;
    }

    // Other scripts (like the bullets) will call this method when they hit the ship
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Player hit! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Logic for death
        Destroy(gameObject); 
    }
}