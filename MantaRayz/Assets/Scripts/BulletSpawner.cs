using System;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject bulletPrefab;
    public int numberOfBullets = 40; 
    
    [Header("Frequency Settings")]
    [Tooltip("How many bullets shoot per second")]
    public float fireRate = 5f; 

    public event Action playerShoot;
    void Start()
    {
        SpawnBulletPool();
    }

    private void SpawnBulletPool()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab is not assigned to the spawner!");
            return;
        }

        // Calculate the actual time gap based on the frequency
        // Example: 5 bullets a second = 1 / 5 = 0.2 seconds between each shot
        float timeBetweenShots = 1f / fireRate;

        for (int i = 0; i < numberOfBullets; i++)
        {
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            playerShoot?.Invoke();
            BulletLoop loopScript = newBullet.GetComponent<BulletLoop>();
            
            if (loopScript != null)
            {
                // Assign the delay using the new frequency calculation
                loopScript.fireDelay = i * timeBetweenShots;
            }
        }
    }
}