
using UnityEngine;

public class FloaterShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireDelay = 1f;

    private void Start()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab is missing on " + gameObject.name);
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        BulletLoop bulletLoop = bullet.GetComponent<BulletLoop>();

        if (bulletLoop == null)
        {
            Debug.LogError("Bullet prefab has no BulletLoop component!");
            Destroy(bullet);
            return;
        }

        // Make this floater the bullet's permanent origin.
        bulletLoop.shipOrigin = transform;
        bulletLoop.fireDelay = fireDelay;
        bulletLoop.Initialize();

        Debug.Log("Bullet origin assigned to: " + transform.name);
    }
}
