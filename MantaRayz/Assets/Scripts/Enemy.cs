using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private int health;


    // Update is called once per frame
    void Update()//Spawns enemies and moves them down the screen
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);

        if(transform.position.y <= -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)//Destroys the enemy when it collides with a bullet
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            health--;

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
