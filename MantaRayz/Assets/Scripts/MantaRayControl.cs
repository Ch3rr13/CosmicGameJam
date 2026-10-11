
using UnityEngine;
using UnityEngine.InputSystem;
public class MantaRayControl : MonoBehaviour
{
    public float speed = 10f;
    public float limitX = 8f;
    public float limitZ = 6f;

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f;
        float v = 0f;

        if (kb.aKey.isPressed) h -= 1f;
        if (kb.dKey.isPressed) h += 1f;
        if (kb.sKey.isPressed) v -= 1f;
        if (kb.wKey.isPressed) v += 1f;

        Vector3 dir = new Vector3(h,v, 0f);
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
    }
}
