using UnityEngine;

public class BulletLoop : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 15f;
    public float topBoundary = 60f; 
    public float fireDelay = 0f;

    [Header("References")]
    public Transform shipOrigin;
    public string shipTag = "Player";

    private float currentDelay;
    private Vector3 startPosition;
    
    // 1. Add this variable to control visibility
    private MeshRenderer bulletMesh; 

    void Start()
    {
        startPosition = transform.position; 
        transform.up = Vector3.up;
        
        // 2. Grab the mesh component when the bullet is created
        bulletMesh = GetComponent<MeshRenderer>();
        
        FindShip();
        ResetBullet();
        transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);
    }

    void Update()
    {
        if (currentDelay > 0)
        {
            // Keep the bullet invisible while it waits
            if (bulletMesh != null) bulletMesh.enabled = false;
            
            // Keep snapping the bullet to the moving Floater while waiting
            if (shipOrigin != null)
            {
                transform.position = shipOrigin.position;
            }
            
            currentDelay -= Time.deltaTime;
            return;
        }

        // Make the bullet visible the exact frame it starts moving
        if (bulletMesh != null) bulletMesh.enabled = true;

        // Move along the World Y-axis
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        // Check the Y boundary
        if (transform.position.y > topBoundary)
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
            transform.position = startPosition;
        }
        
        currentDelay = fireDelay;
    }

    private void FindShip()
    {
        if (shipOrigin == null)
        {
            GameObject ship = GameObject.FindGameObjectWithTag("Floater");
            if (ship != null)
            {
                shipOrigin = ship.transform;
            }
        }
    }
}