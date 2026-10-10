using UnityEngine;

public class FloaterOrbit : MonoBehaviour
{
    public Transform target;
    public float radius = 3f;
    public float speed = 120f;
    public float heightOffset = 0f;

    float angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        angle += speed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        transform.position = target.position + offset + Vector3.up * heightOffset;
    }
}
