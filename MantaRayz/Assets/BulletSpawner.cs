using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject bulletPrefab;
    public int numberOfBullets = 20; 
    public float staggerTime = 0.15f; // Time between each shot

    void Start()
    {
        SpawnBulletPool();
    }

    private void SpawnBulletPool()
    {
            return;
        }

        for (int i = 0; i < numberOfBullets; i++)
        {
            // Instantiate creates a new copy of the prefab
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            
            // Access the BulletLoop script on the new instance
            BulletLoop loopScript = newBullet.GetComponent<BulletLoop>();
            
            if (loopScript != null)
            {
                // Mathematically assign the delay (e.g., 0, 0.15, 0.30, 0.45...)
                loopScript.fireDelay = i * staggerTime;
            }
        }
    }
}