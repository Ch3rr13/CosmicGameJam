using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float randomX = Random.Range(-8f, 8f); //randomizes spawn point on the x-axis
            Vector3 launch = new Vector3(randomX, 8f, 0f); //enemies get spawned outside of view of the camera by putting z = 8f
            Instantiate(enemyPrefab, launch, Quaternion.Euler(0f, 180f, 0f)); //Euler fct allows enemies to spawn pointing downwards towards the player
            yield return new WaitForSeconds(5f);
        }
    }
}
