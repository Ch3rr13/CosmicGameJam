using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float hitDamage = 10f;

    [Header("Leave empty: found automatically in the scene")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameManager gameManager;

    [Header("What hurts the player (tags of the objects)")]
    [SerializeField] private string[] damageTags = { "Enemy" };

    [Header("Testing")]
    [SerializeField] private bool spaceTest = false;   // tick to press Space to take damage

    private float currentHealth;
    public event Action playerTakeDamage;

    private void Start()
    {
        // The Player prefab works with no setup: it finds the UI in the scene by itself.
        if (healthBar == null) healthBar = FindFirstObjectByType<HealthBar>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();

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
        if (HurtsPlayer(collision.gameObject))
            TakeDamage(hitDamage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HurtsPlayer(other.gameObject))
            TakeDamage(hitDamage);
    }

    // Only objects with one of the damage tags hurt the player.
    // (Floaters, bullets, fish and pickups have other tags, so they never cost health.)
    private bool HurtsPlayer(GameObject other)
    {
        foreach (string damageTag in damageTags)
        {
            if (other.tag == damageTag)      // plain comparison: no error if a tag does not exist
                return true;
        }
        return false;
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
                gameManager.GameOver();
            else
                Debug.LogError("PlayerHealth: no GameManager found in the scene!");
        }
    }
}