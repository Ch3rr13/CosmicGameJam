
using UnityEngine;

public class BulletLoop : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 15f;
    public float topBoundary = 60f;
    public float fireDelay = 1f;

    [Header("References")]
    public Transform shipOrigin;

    private float currentDelay;
    private MeshRenderer bulletMesh;

    private void Awake()
    {
        bulletMesh = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        transform.up = Vector3.up;
        transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);

        if (shipOrigin == null)
        {
            Debug.LogError(
                "Bullet has no shipOrigin assigned!", this
            );
            return;
        }

        ResetBullet();
    }

    public void Initialize()
    {
        if (shipOrigin == null)
        {
            Debug.LogError(
                "Cannot initialize bullet: shipOrigin is missing!", this
            );
            return;
        }

        ResetBullet();
    }

    private void Update()
    {
        if (shipOrigin == null)
        {
            if (bulletMesh != null)
                bulletMesh.enabled = false;

            return;
        }

        if (currentDelay > 0f)
        {
            if (bulletMesh != null)
                bulletMesh.enabled = false;

            transform.position = shipOrigin.position;
            currentDelay -= Time.deltaTime;
            return;
        }

        if (bulletMesh != null)
            bulletMesh.enabled = true;

        transform.Translate(
            Vector3.up * speed * Time.deltaTime,
            Space.World
        );

        if (transform.position.y >= topBoundary)
        {
            ResetBullet();
        }
    }

    private void ResetBullet()
    {
        if (shipOrigin == null)
            return;

        transform.position = shipOrigin.position;
        currentDelay = fireDelay;

        if (bulletMesh != null)
            bulletMesh.enabled = currentDelay <= 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(1);
                ResetBullet();
            }
        }
    }
}
