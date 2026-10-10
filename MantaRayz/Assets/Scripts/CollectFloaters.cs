
using System.Collections.Generic;
using UnityEngine;

public class CollectFloaters : MonoBehaviour
{
    public GameObject floaterPrefab;
    public float orbitRadius = 3f;
    public float orbitSpeed = 120f;
    public float heightOffset = 0f;

    private List<FloaterOrbit> floaters = new List<FloaterOrbit>();

    void Start()
    {
        FloaterOrbit[] existingFloaters =
            FindObjectsByType<FloaterOrbit>(FindObjectsSortMode.None);

        foreach (FloaterOrbit floater in existingFloaters)
        {
            if (floater.target == transform)
            {
                floaters.Add(floater);
            }
        }

        ArrangeFloaters();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Sphere")) return;

        Destroy(other.gameObject);

        if (floaterPrefab == null)
        {
            Debug.LogError("Floater Prefab is not assigned!", this);
            return;
        }

        GameObject newFloater = Instantiate(
            floaterPrefab,
            transform.position,
            Quaternion.identity
        );

        FloaterOrbit orbit = newFloater.GetComponent<FloaterOrbit>();
        FloaterShooter shooter = newFloater.GetComponent<FloaterShooter>();

        if (orbit == null)
        {
            Debug.LogError("Floater prefab needs FloaterOrbit!", newFloater);
            Destroy(newFloater);
            return;
        }

        if (shooter == null)
        {
            Debug.LogError("Floater prefab needs FloaterShooter!", newFloater);
        }

        orbit.target = transform;
        orbit.radius = orbitRadius;
        orbit.speed = orbitSpeed;
        orbit.heightOffset = heightOffset;

        floaters.Add(orbit);
        ArrangeFloaters();
    }

    void ArrangeFloaters()
    {
        floaters.RemoveAll(floater => floater == null);

        for (int i = 0; i < floaters.Count; i++)
        {
            float angle = i * (360f / floaters.Count);
            floaters[i].SetAngle(angle);
        }
    }
}
