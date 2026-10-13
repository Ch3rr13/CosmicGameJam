using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverSceneManager : MonoBehaviour
{
    private bool leaving = false;
    private float acceptInputAt;

    private void OnEnable()
    {
        leaving = false;
        acceptInputAt = Time.unscaledTime + 0.25f;
    }

    void Update()
    {
        if (leaving || Time.unscaledTime < acceptInputAt) return;

        // Any key or left mouse click
        bool anyKeyPressed = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (!anyKeyPressed) return;
        leaving = true;

        GameManager.GetOrCreate().InitGame();
    }
}
