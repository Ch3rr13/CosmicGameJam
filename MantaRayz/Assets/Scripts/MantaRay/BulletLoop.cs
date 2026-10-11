using UnityEngine;

// On the bullet prefab. Flies straight up, then disappears.
public class BulletLoop : MonoBehaviour
{
    public float speed = 25f;
    public float range = 25f;       // how far it flies before disappearing

    private Vector3 start;

    private void Start()
    {
        transform.up = Vector3.up;  // point straight up
        start = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(start, transform.position) > range)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}