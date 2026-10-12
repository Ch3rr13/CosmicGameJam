using UnityEngine;

// On the collectable fish prefab. It swims across the screen and removes itself once it has left the frame.
// To be catchable: tag it "Sphere" and give it a Collider with Is Trigger ticked.
public class CollectibleFish : MonoBehaviour
{
    public float speed = 25f;
    public float bobHeight = 1f;        // how far it wobbles up and down while swimming
    public float bobSpeed = 2f;
    public float maxLifetime = 40f;     // safety: it is always removed after this many seconds

    [Tooltip("Turns the model so its nose points along its swim direction. Try (0,90,0), (0,-90,0) or (0,180,0) until it is right.")]
    public Vector3 facingOffset = Vector3.zero;

    [HideInInspector] public float direction = 1f;   // 1 = swims right, -1 = swims left (set by the spawner)

    private float startY;
    private float startZ;               // the fish never leaves this depth
    private float seed;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        startY = transform.position.y;
        startZ = transform.position.z;
        seed = Random.value * 10f;

        // Face the way it swims (left or right).
        transform.rotation = Quaternion.LookRotation(new Vector3(direction, 0f, 0f), Vector3.up)
                             * Quaternion.Euler(facingOffset);

        // A stuck fish must never block the next one from appearing.
        Destroy(gameObject, maxLifetime);
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.x += direction * speed * Time.deltaTime;
        pos.y = startY + Mathf.Sin((Time.time + seed) * bobSpeed) * bobHeight;
        pos.z = startZ;                 // locked: no drifting towards or away from the camera
        transform.position = pos;

        // Swam out of frame on the far side: remove it.
        if (cam != null)
        {
            float screenX = cam.WorldToViewportPoint(pos).x;

            if ((direction > 0f && screenX > 1.15f) || (direction < 0f && screenX < -0.15f))
                Destroy(gameObject);
        }
    }
}