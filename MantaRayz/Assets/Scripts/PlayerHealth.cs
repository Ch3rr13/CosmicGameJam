
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameManager gameManager;

    private float currentHealth;
    private float enemyDamage;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);

        GameObject gameObject = collision.gameObject;

        if (collision.rigidbody != null && (collision.gameObject.CompareTag("Enemy") /*|| collision.gameObject.CompareTag("Obstacle")*/))//If we fo created obstacles
        {
            Enemy enemy = gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemyDamage = enemy.Damage;
            }

            TakeDamage(enemyDamage);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with: " + other.name);

        if (other.attachedRigidbody != null)
            TakeDamage(hitDamage);
    }*/



    private void TakeDamage(float amount)
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        Debug.Log("Current health: " + currentHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Debug.Log("PLAYER DIED");

            if (gameManager != null)
            {
                Debug.Log("Calling GameManager");
                gameManager.GameOver();
            }
            else
            {
                Debug.LogError("GameManager is NULL!");
            }
        }
    }


}
