using System;
using System.Collections.Generic;
using UnityEngine;

// Put this on the Player.
// It makes floaters circle the player, and each floater shoots short bursts.
public class FloaterRing : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject floaterPrefab;     // just a model, no scripts
    public GameObject bulletPrefab;      // needs BulletLoop

    [Header("Floaters")]
    public int maxFloaters = 3;
    public string collectableTag = "Sphere";

    [Header("Orbit")]
    public float radius = 3f;
    public float orbitSpeed = 90f;       // degrees per second

    [Header("Shooting")]
    public int bulletsPerBurst = 3;
    public float timeBetweenBullets = 0.1f;
    public float timeBetweenBursts = 0.5f;

    public event Action playerShoot;

    private class Floater
    {
        public Transform t;
        public float slot;               // where it is on the ring
        public float targetSlot;         // where it is sliding to
        public float cooldown;           // time until the next bullet
        public int shotsLeft;            // bullets left in this burst

       
    }

    private readonly List<Floater> floaters = new List<Floater>();

    private void Start()
    {
        if (floaterPrefab == null || bulletPrefab == null)
        {
            Debug.LogError("FloaterRing needs a Floater Prefab and a Bullet Prefab!", this);
            enabled = false;
            return;
        }

        AddFloater();   // the one you start with
    }

    // Picking up a collectable adds a floater.
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(collectableTag)) return;
        if (floaters.Count >= maxFloaters) return;

        Destroy(other.gameObject);
        AddFloater();
    }

    private void AddFloater()
    {
        Floater f = new Floater();
        Quaternion rotation = Quaternion.LookRotation(Vector3.up, Vector3.back);//Makes sure floater looks up
        f.t = Instantiate(floaterPrefab, transform.position, rotation).transform;
        f.shotsLeft = bulletsPerBurst;
        f.cooldown = floaters.Count * 0.2f;      // each floater starts a little later
        floaters.Add(f);

        // Spread the floaters evenly around the ring.
        for (int i = 0; i < floaters.Count; i++)
            floaters[i].targetSlot = i * 360f / floaters.Count;

        f.slot = f.targetSlot;
    }

    private void Update()
    {
        float spin = Time.time * orbitSpeed;

        for (int i = floaters.Count - 1; i >= 0; i--)
        {
            Floater f = floaters[i];

            if (f.t == null)                     // floater was destroyed
            {
                floaters.RemoveAt(i);
                continue;
            }

            // Orbit around the player.
            f.slot = Mathf.MoveTowardsAngle(f.slot, f.targetSlot, 180f * Time.deltaTime);
            float rad = (spin + f.slot) * Mathf.Deg2Rad;
            f.t.position = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;

            // Shoot a burst, then rest.
            f.cooldown -= Time.deltaTime;
            if (f.cooldown <= 0f)
            {
                Instantiate(bulletPrefab, f.t.position, Quaternion.identity);
                f.shotsLeft--;
                playerShoot?.Invoke();

                if (f.shotsLeft > 0)
                {
                    f.cooldown = timeBetweenBullets;
                }
                else
                {
                    f.shotsLeft = bulletsPerBurst;
                    f.cooldown = timeBetweenBursts;
                }
            }
        }
    }
}