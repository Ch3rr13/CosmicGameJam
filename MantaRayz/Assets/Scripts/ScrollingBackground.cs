using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 0.5f;

    [SerializeField]
    private Renderer backgroundRenderer;
    
    void Update()
    {
        float offset = Time.time * scrollSpeed;
        backgroundRenderer.material.mainTextureOffset = new Vector2(0, offset);
    }
}
