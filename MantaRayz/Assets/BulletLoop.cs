using UnityEngine;

public class BulletLoop : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float topBoundary = 6f; 
    public float fireDelay = 0f;

    [Header("References")]
    public Transform shipOrigin;
    public string shipTag = "Player";

    private float currentDelay;
    private Vector3 startPosition; // Stores the bullet's initial placement in the scene

    void Start()
    {
        startPosition = transform.position; // Save initial position as a fallback
        FindShip();
        ResetBullet();
    }

    void Update()
    {
        if (currentDelay > 0)
        {
            currentDelay -= Time.deltaTime;
            return;
        }

        // Move along the World X-axis since the camera's Y rotation is 90
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);

        // Check the X boundary
        if (transform.position.x > topBoundary)
        {
            ResetBullet();
        }
    }

    private void ResetBullet()
    {
        FindShip();

        if (shipOrigin != null)
        {
            transform.position = shipOrigin.position;
        }
        else
        {
            // Fallback to the original starting position if the ship does not exist yet
            transform.position = startPosition;
        }
        
        currentDelay = fireDelay;
    }

    private void FindShip()
    {
        if (shipOrigin == null)
        {
            GameObject ship = GameObject.FindGameObjectWithTag(shipTag);
            if (ship != null)
            {
                shipOrigin = ship.transform;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // REPLACE TAG LATER WITH ACTIAL ENEMY OBJECT NAME!!!!
        if (other.CompareTag("Enemy"))
        {
            // Try to find the EnemyHealth script on that object
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            
            if (enemy != null)
            {
                // Deal 1 damage to the enemy: 1 hit
                enemy.TakeDamage(1);
                
                // Reset the bullet back to the ship immediately so it can fire again
                ResetBullet();
            }
        }
    }
}