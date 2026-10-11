using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Put on an empty object in the scene.
// Scatters random 3D decoration on the background plane (z = 6.5) and moves it from the top of the
// screen to the bottom, at exactly the pace of the scrolling texture (read from ScrollingBackground).
// Every asset has its own fixed scale, rotation and tint. The decoration is not interactive:
// colliders and rigidbodies are removed from every copy.
public class BackgroundDecorSpawner : MonoBehaviour
{
    [System.Serializable]
    public class DecorEntry
    {
        public string name;                      // just a label, so the list is easy to read
        public GameObject prefab;
        [Tooltip("Multiplies the asset's own size. 1 = as it is.")]
        public float scale = 1f;
        [Tooltip("Fixed direction of the asset, in degrees (X, Y, Z).")]
        public Vector3 rotation = Vector3.zero;
        [Tooltip("Multiplies the asset's colour. White = unchanged. A darker, bluer colour helps it blend in.")]
        public Color tint = Color.white;
    }

    [Header("What (each asset has its own scale, rotation and tint)")]
    public DecorEntry[] decor;

    [Header("Where")]
    [Tooltip("Every piece sits at this z position.")]
    public float zPosition = 6.5f;

    [Header("Speed (matched to the scrolling background)")]
    [Tooltip("The height of the background in world units. 0 = measured automatically from its renderer.")]
    public float backgroundHeight = 0f;
    [Tooltip("Nudge the speed if the decor slides against the background. 1 = same as the background.")]
    public float speedMultiplier = 1f;
    [Tooltip("With a perspective camera, things closer to the camera look faster at the same speed. This corrects for the depth difference.")]
    public bool matchLookOfBackground = true;
    [Tooltip("Used when no ScrollingBackground is found.")]
    public float fallbackSpeed = 3f;

    [Header("How much")]
    public int startCount = 8;           // already on screen when the game starts
    public float spawnEvery = 1.5f;      // seconds between new pieces
    public int maxAtOnce = 15;

    [Header("Blending in")]
    [Tooltip("Pieces do not cast or receive shadows, so they do not look stuck on top of the background.")]
    public bool noShadows = true;

    private readonly List<Transform> items = new List<Transform>();
    private Camera cam;
    private MonoBehaviour scroller;      // the ScrollingBackground script
    private FieldInfo scrollSpeedField;  // its public 'scrollSpeed'
    private Renderer backgroundRenderer; // the renderer whose texture scrolls
    private float timer;
    private float findTimer;

    private void Start()
    {
        cam = Camera.main;

        if (cam == null || decor == null || decor.Length == 0)
        {
            Debug.LogError("BackgroundDecorSpawner needs a Main Camera and at least one asset in the Decor list!", this);
            enabled = false;
            return;
        }

        FindScroller();

        for (int i = 0; i < startCount; i++)
            SpawnOne(true);

        timer = spawnEvery;
    }

    private void Update()
    {
        // Look for the scrolling background (it may appear a moment after the start).
        if (scroller == null)
        {
            findTimer -= Time.deltaTime;
            if (findTimer <= 0f)
            {
                FindScroller();
                findTimer = 1f;
            }
        }

        float speed = BackgroundWorldSpeed() * speedMultiplier;
        float backgroundDepth = backgroundRenderer != null
            ? cam.WorldToViewportPoint(backgroundRenderer.bounds.center).z
            : 0f;
        Vector3 screenDown = ScreenDown();

        for (int i = items.Count - 1; i >= 0; i--)
        {
            Transform item = items[i];

            if (item == null)
            {
                items.RemoveAt(i);
                continue;
            }

            // A perspective camera makes nearer things look faster, so scale this piece's speed by how far it is
            // from the camera compared with the background. Then it slides across the screen at the same pace.
            float factor = 1f;
            Vector3 view = cam.WorldToViewportPoint(item.position);

            if (matchLookOfBackground && !cam.orthographic && backgroundDepth > 0.01f && view.z > 0.01f)
                factor = view.z / backgroundDepth;

            Vector3 pos = item.position + screenDown * (speed * factor * Time.deltaTime);
            pos.z = zPosition;
            item.position = pos;

            // Scrolled off the bottom: remove it.
            if (cam.WorldToViewportPoint(pos).y < -0.25f)
            {
                Destroy(item.gameObject);
                items.RemoveAt(i);
            }
        }

        // Bring in new pieces from the top.
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = spawnEvery;

            if (items.Count < maxAtOnce)
                SpawnOne(false);
        }
    }

    // Anywhere = true: placed anywhere on screen (the start). False: placed just above the top edge.
    private void SpawnOne(bool anywhere)
    {
        DecorEntry entry = decor[Random.Range(0, decor.Length)];
        if (entry == null || entry.prefab == null) return;

        float screenX = Random.value;
        float screenY = anywhere ? Random.value : 1.2f;

        // Where the camera's view of that screen point meets the plane z = 6.5.
        Plane plane = new Plane(Vector3.back, new Vector3(0f, 0f, zPosition));
        Ray ray = cam.ViewportPointToRay(new Vector3(screenX, screenY, 0f));

        if (!plane.Raycast(ray, out float distance))
        {
            Debug.LogWarning("BackgroundDecorSpawner: the camera does not look at the plane z = " + zPosition, this);
            return;
        }

        Vector3 pos = ray.GetPoint(distance);
        pos.z = zPosition;

        // This asset's own fixed rotation and scale.
        GameObject go = Instantiate(entry.prefab, pos, Quaternion.Euler(entry.rotation));
        go.transform.localScale = entry.prefab.transform.localScale * entry.scale;

        // Decoration only: nothing can hit it or be hit by it.
        foreach (Collider c in go.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
            Destroy(c);
        }
        foreach (Rigidbody rb in go.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        Blend(go, entry);

        items.Add(go.transform);
    }

    // The scrolling texture moves "scrollSpeed" texture heights per second. This turns that into world units
    // per second: scrollSpeed x (height of the background in the world) / (how many times the texture repeats).
    private float BackgroundWorldSpeed()
    {
        if (scroller == null || scrollSpeedField == null)
            return fallbackSpeed;

        float textureHeightsPerSecond = Mathf.Abs((float)scrollSpeedField.GetValue(scroller));

        float worldHeight = backgroundHeight;
        if (worldHeight <= 0f && backgroundRenderer != null)
            worldHeight = backgroundRenderer.bounds.size.y;

        if (worldHeight <= 0f)
            return fallbackSpeed;

        float tiling = 1f;
        if (backgroundRenderer != null && backgroundRenderer.sharedMaterial != null)
            tiling = Mathf.Max(0.0001f, Mathf.Abs(backgroundRenderer.sharedMaterial.mainTextureScale.y));

        return textureHeightsPerSecond * worldHeight / tiling;
    }

    // Finds the ScrollingBackground by name, so this script does not depend on it at compile time.
    private void FindScroller()
    {
        foreach (MonoBehaviour m in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (m.GetType().Name != "ScrollingBackground") continue;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo speedField = m.GetType().GetField("scrollSpeed", flags);
            if (speedField == null || speedField.FieldType != typeof(float)) continue;

            Renderer r = null;
            FieldInfo rendererField = m.GetType().GetField("backgroundRenderer", flags);
            if (rendererField != null)
                r = rendererField.GetValue(m) as Renderer;
            if (r == null)
                r = m.GetComponentInChildren<Renderer>();

            scroller = m;
            scrollSpeedField = speedField;
            backgroundRenderer = r;

            Debug.Log("BackgroundDecorSpawner: following " + m.name + ", world speed " + BackgroundWorldSpeed());
            return;
        }
    }

    // Makes a piece sit in the background: no shadows, and this asset's tint.
    private void Blend(GameObject go, DecorEntry entry)
    {
        foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
        {
            if (noShadows)
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
            }

            if (entry.tint == Color.white) continue;

            foreach (Material m in r.materials)       // copies, so the original asset is untouched
            {
                if (m.HasProperty("_BaseColor"))
                    m.SetColor("_BaseColor", m.GetColor("_BaseColor") * entry.tint);
                else if (m.HasProperty("_Color"))
                    m.SetColor("_Color", m.GetColor("_Color") * entry.tint);
            }
        }
    }

    // "Down the screen", worked out from the camera, so it is right whichever way the camera is turned.
    private Vector3 ScreenDown()
    {
        Vector3 d = -cam.transform.up;
        d.z = 0f;
        return d.sqrMagnitude > 0.0001f ? d.normalized : Vector3.down;
    }

#if UNITY_EDITOR
    // Handy: select your assets in the Project window, then right-click this component's header
    // and choose "Add Selected Assets". (Lock the Inspector first.)
    [ContextMenu("Add Selected Assets")]
    private void AddSelectedAssets()
    {
        GameObject[] selected = UnityEditor.Selection.GetFiltered<GameObject>(UnityEditor.SelectionMode.Assets);
        List<DecorEntry> list = new List<DecorEntry>(decor != null ? decor : new DecorEntry[0]);

        foreach (GameObject go in selected)
        {
            bool alreadyThere = false;

            foreach (DecorEntry e in list)
                if (e != null && e.prefab == go) alreadyThere = true;

            if (!alreadyThere)
                list.Add(new DecorEntry { name = go.name, prefab = go });
        }

        decor = list.ToArray();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}