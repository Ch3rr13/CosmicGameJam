using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float hitDamage = 0f;

    [Header("Leave empty: found automatically in the scene")]
    [SerializeField] private HealthBar healthBar;

    [Header("What hurts the player (tags of the objects)")]
    [SerializeField] private string[] damageTags = { "Enemy" };

    [Header("Testing")]
    [SerializeField] private bool spaceTest = false;   // tick to press Space to take damage

    private float currentHealth;
    public event Action playerTakeDamage;

    private void Start()
    {
        if (healthBar == null) healthBar = FindFirstObjectByType<HealthBar>();

        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
        else
            Debug.LogWarning("PlayerHealth: no HealthBar found in the scene.");
    }

    private void Update()
    {
        if (spaceTest && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            TakeDamage(hitDamage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHit(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other.gameObject);
    }

    private void TryHit(GameObject other)
    {
        if (!HurtsPlayer(other)) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning($"PlayerHealth: '{other.name}' has a damage tag but no Enemy component.");
            return;
        }

        hitDamage = enemy.Damage;
        TakeDamage(hitDamage);
    }

    // Only objects with one of the damage tags hurt the player.
    private bool HurtsPlayer(GameObject other)
    {
        foreach (string damageTag in damageTags)
        {
            for (Transform candidate = other.transform; candidate != null; candidate = candidate.parent)
                if (candidate.CompareTag(damageTag)) return true;
        }
        return false;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0f || amount <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        playerTakeDamage?.Invoke();

        Debug.Log("Current health: " + currentHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Debug.Log("PLAYER DIED");

            // Always use the persistent singleton. A reference assigned in the inspector
            // can point at a duplicate GameManager that destroyed itself.
            GameManager.GetOrCreate().GameOver();
        }
    }
}
