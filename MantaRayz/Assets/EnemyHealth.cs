using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 1; // Set to 1 for weak enemies that die in one hit

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Destroys the enemy GameObject, removing it from the scene
        Destroy(gameObject); 
    }
}