using UnityEngine;

// Added to each floater by FloaterRing when it spawns (no prefab changes needed).
// The floater loses one hit each time an enemy touches it, and disappears when none are left.
public class FloaterHealth : MonoBehaviour
{
    [SerializeField] private int maxHits = 1;
    [Tooltip("Seconds of protection after a hit, so one enemy touching several colliders only counts once")]
    [SerializeField] private float hitCooldown = 0.5f;
    [Tooltip("Tags of objects that hurt the floater")]
    [SerializeField] private string[] damageTags = { "Enemy" };

    private int hitsLeft;
    private float nextHitTime;

    public int HitsLeft { get { return hitsLeft; } }

    private void Awake()
    {
        hitsLeft = maxHits;

        // A trigger needs a Rigidbody on at least one side. A kinematic one stays under
        // FloaterRing's control (it moves the floater by position).
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Use the prefab's colliders if it has any; otherwise add a simple one.
        Collider[] cols = GetComponentsInChildren<Collider>();
        if (cols.Length == 0)
        {
            SphereCollider sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = 0.5f;
            cols = new Collider[] { sc };
        }

        // Triggers: the floater detects enemies but never pushes them or the player around.
        foreach (Collider c in cols)
            c.isTrigger = true;
    }

    // Called by FloaterRing right after the floater is created.
    public void Init(int hits, float cooldown)
    {
        maxHits = Mathf.Max(1, hits);
        hitsLeft = maxHits;
        hitCooldown = cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsHazard(other)) return;
        if (Time.time < nextHitTime) return;
        nextHitTime = Time.time + hitCooldown;

        hitsLeft--;
        Debug.Log($"Floater hit! {hitsLeft} / {maxHits} left");

        if (hitsLeft <= 0)
            Destroy(gameObject, 0.1f);   // FloaterRing notices and re-spaces the others
    }

    private bool IsHazard(Collider other)
    {
        foreach (string damageTag in damageTags)
        {
            if (other.tag == damageTag) return true;   // plain comparison: no error if a tag doesn't exist

            Rigidbody body = other.attachedRigidbody;   // collider on a child of the enemy
            if (body != null && body.tag == damageTag) return true;
        }
        return false;
    }
}