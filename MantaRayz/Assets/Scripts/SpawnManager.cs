using UnityEngine;
using System.Collections;
using System;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    public event Action enemySpawn;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            enemySpawn?.Invoke();
            float randomX = UnityEngine.Random.Range(-8f, 8f); //randomizes spawn point on the x-axis
            Vector3 launch = new Vector3(randomX, 8f, -0.1f); //enemies get spawned outside of view of the camera by putting z = 8f
            Quaternion rotation = Quaternion.LookRotation(Vector3.down, Vector3.back);
            Instantiate(enemyPrefab, launch, rotation);
            yield return new WaitForSeconds(5f);
        }
    }
}
