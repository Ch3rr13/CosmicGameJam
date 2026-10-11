using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private int health;

    public enum enemyType { Stationary, Honing, Magnetic };//Different types of enemies with different health and speed

    [SerializeField]
    private enemyType EnemyType;

    [SerializeField]
    public int Damage;

    [SerializeField]
    public float difficultyScale = 0.5f;//As time goes on the speed of enemies ebign spawned will increase by this amount in seconds
    public float minSpawnRate = 0.5f;//As time goes on the speed of enemies ebign spawned will increase by this amount in seconds

    private GameObject player;

    public event Action enemyTakeDamage;
    public event Action enemyDeath;

    void Start()//Sets the health and speed of the enemy based on its type
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()//Spawns enemies and moves them down the screen
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (transform.position.z <= -10f)
        {
            Destroy(gameObject);
        }

        if (EnemyType == enemyType.Honing && player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * 2 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)//Destroys the enemy when it collides with a bullet
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Enemy hit by " + other.gameObject.tag);
            health--;
            ScoreManager.addScore(10);
            Debug.Log("Current score" + ScoreManager.getScore());

            enemyTakeDamage?.Invoke();

            if (health <= 0)
            {
                enemyDeath?.Invoke();
                Destroy(gameObject, 0.01f);//Deletes instance of the enemy after less than a second
            }

        }
        else if (other.gameObject.CompareTag("Floater"))
        {
            Debug.Log("Enemy hit by " + other.gameObject.tag);
            //Ideally floater would be assigned health and it would decrease
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy hit by " + other.gameObject.tag);

            Destroy(gameObject, 0.01f);//Enemy immediately dies when it collides with the player, but the player takes damage
        }
    }
}
