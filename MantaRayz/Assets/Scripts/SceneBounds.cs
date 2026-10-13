using UnityEngine;

// Put this on the Player (manta ray).
// Keeps it inside the visible screen area, for perspective or orthographic cameras.
// Assumes a side-on 2D-style view: the camera looks along Z and the player moves on the XY plane.
public class ScreenBounds : MonoBehaviour
{
    [Tooltip("Leave empty to use the Main Camera")]
    [SerializeField] private Camera cam;

    [Tooltip("World units kept between the player's centre and each screen edge (X = left/right, Y = bottom/top). " +
             "Use at least half the sprite size. Raise it to about the floater ring radius to keep floaters on screen too.")]
    [SerializeField] private Vector2 padding = new Vector2(1.5f, 1.5f);

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (cam == null) cam = Camera.main;
    }

    // LateUpdate: runs after the movement script has moved the player this frame.
    private void LateUpdate()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        Vector3 pos = transform.position;

        // How far the player is in front of the camera, so the screen edges are measured on the player's plane.
        float depth = cam.WorldToViewportPoint(pos).z;
        if (depth <= 0f) return;   // behind the camera: nothing sensible to clamp to

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1f, 1f, depth));

        float minX = min.x + padding.x, maxX = max.x - padding.x;
        float minY = min.y + padding.y, maxY = max.y - padding.y;

        // Padding bigger than the screen: just sit in the middle.
        if (minX > maxX) minX = maxX = (min.x + max.x) * 0.5f;
        if (minY > maxY) minY = maxY = (min.y + max.y) * 0.5f;

        Vector3 clamped = new Vector3(
            Mathf.Clamp(pos.x, minX, maxX),
            Mathf.Clamp(pos.y, minY, maxY),
            pos.z);

        if (clamped == pos) return;

        transform.position = clamped;

        // If physics is driving the player, stop it pushing further out so it doesn't jitter on the edge.
        if (rb != null && !rb.isKinematic)
        {
            Vector3 v = rb.linearVelocity;
            if ((pos.x < minX && v.x < 0f) || (pos.x > maxX && v.x > 0f)) v.x = 0f;
            if ((pos.y < minY && v.y < 0f) || (pos.y > maxY && v.y > 0f)) v.y = 0f;
            rb.linearVelocity = v;
        }
    }
}