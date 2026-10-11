using UnityEngine;
using System.Collections;
using System;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private float waitTime = 5f;//Time before the first enemy spawns

    //public event Action enemySpawn;

    private static float INITIAL_SPAWNRATE = 5f;
    private float difficultyScale = 0.01f;//As time goes on the spawn rate will decrease by this amount in seconds
    private float MIN_SPAWNRATE = 0.1f;//General minimum spawn rate for enemies will be every 0.1seconds
    private float currentSpawnRate = INITIAL_SPAWNRATE;
    private float roundStart = 0;

    void Start()
    {
        difficultyScale = enemyPrefab.GetComponent<Enemy>().difficultyScale; //sets the difficulty scale to the value in the Enemy script
        MIN_SPAWNRATE = enemyPrefab.GetComponent<Enemy>().minSpawnRate; //sets the minimum spawn rate to the value in the Enemy script
        roundStart = Time.time; //sets the round start time to the current time
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            currentSpawnRate = INITIAL_SPAWNRATE - ((Time.time - roundStart) * difficultyScale);
            currentSpawnRate = Mathf.Max(currentSpawnRate, MIN_SPAWNRATE);

            //enemySpawn?.Invoke();
            float randomX = UnityEngine.Random.Range(-8f, 8f); //randomizes spawn point on the x-axis
            Vector3 launch = new Vector3(randomX, 8f, 6f); //enemies get spawned outside of view of the camera by putting z = 8f
            Quaternion rotation = Quaternion.LookRotation(Vector3.down, Vector3.back);
            Instantiate(enemyPrefab, launch, rotation);

            Debug.Log("Spawn Rate: " + currentSpawnRate);

            yield return new WaitForSeconds(currentSpawnRate);
        }
    }
}
