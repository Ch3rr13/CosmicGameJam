using UnityEngine;

// Put on an empty object in the scene.
// A fish swims in every few seconds, until the player has all the floaters they can hold.
public class CollectibleSpawner : MonoBehaviour
{
    public CollectibleFish fishPrefab;

    [Header("When")]
    public float spawnEvery = 3f;       // seconds between fish
    public int maxFishAtOnce = 1;       // fish on screen at the same time

    [Header("Where (0 = bottom of screen, 1 = top)")]
    [Range(0f, 1f)] public float minHeight = 0.35f;   // keeps fish away from the corners
    [Range(0f, 1f)] public float maxHeight = 0.65f;

    private float timer;
    private FloaterRing player;

    private void Start()
    {
        timer = spawnEvery;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = spawnEvery;

        if (player == null)
            player = FindFirstObjectByType<FloaterRing>();

        if (player == null)
        {
            Debug.LogWarning("CollectibleSpawner: no FloaterRing found. Is the Player in the scene?", this);
            return;
        }

        // Fish already swimming in count towards the limit, so we never send more than the player can use.
        int fishNow = FindObjectsByType<CollectibleFish>(FindObjectsSortMode.None).Length;

        Debug.Log("Spawner check: fish on screen " + fishNow + "/" + maxFishAtOnce
                  + ", floaters " + player.FloaterCount + "/" + player.maxFloaters);

        if (fishNow >= maxFishAtOnce) return;
        if (player.FloaterCount + fishNow >= player.maxFloaters) return;

        Spawn();
    }

    // Test: in Play mode, right-click the CollectibleSpawner component header and choose "Spawn Fish Now".
    [ContextMenu("Spawn Fish Now")]
    private void SpawnNow()
    {
        Spawn();
    }

    private void Spawn()
    {
        Camera cam = Camera.main;

        if (player == null)
            player = FindFirstObjectByType<FloaterRing>();

        if (fishPrefab == null || cam == null)
        {
            Debug.LogError("CollectibleSpawner needs a Fish Prefab and a Main Camera!", this);
            enabled = false;
            return;
        }

        // Start on the player's depth (z), just outside the left or right edge of the screen.
        float planeZ = player != null ? player.transform.position.z : 0f;
        Plane gamePlane = new Plane(Vector3.back, new Vector3(0f, 0f, planeZ));

        bool fromLeft = Random.value < 0.5f;
        float screenX = fromLeft ? -0.1f : 1.1f;
        float screenY = Random.Range(minHeight, maxHeight);

        Ray ray = cam.ViewportPointToRay(new Vector3(screenX, screenY, 0f));

        if (!gamePlane.Raycast(ray, out float distance))
        {
            Debug.LogWarning("CollectibleSpawner: the camera does not look at the game plane.", this);
            return;
        }

        CollectibleFish fish = Instantiate(fishPrefab, ray.GetPoint(distance), Quaternion.identity);
        fish.direction = fromLeft ? 1f : -1f;

        Debug.Log("Fish spawned at " + fish.transform.position + " (floaters: "
                  + (player != null ? player.FloaterCount : 0) + ")");
    }
}