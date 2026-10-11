
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float hitDamage = 20f;
    [SerializeField] private GameManager gameManager;

    private float currentHealth;
    public event Action playerTakeDamage;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);

        if (collision.rigidbody != null)
            TakeDamage(hitDamage);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with: " + other.name);

        if (other.attachedRigidbody != null)
            TakeDamage(hitDamage);
    }



    private void TakeDamage(float amount)
    {
        if (currentHealth <= 0f) return;
        playerTakeDamage?.Invoke();
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
