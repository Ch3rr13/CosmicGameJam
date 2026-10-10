using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField]
    private float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);

        if(transform.position.y <= -10f)
        {
            Destroy(gameObject);
        }
    }
}
