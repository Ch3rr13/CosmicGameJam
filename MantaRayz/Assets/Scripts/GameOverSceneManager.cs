using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOverSceneManager : MonoBehaviour
{
    private bool leaving = false;

    void Update()
    {
        if (leaving) return;

        // Any key or left mouse click
        bool anyKeyPressed = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (!anyKeyPressed) return;
        leaving = true;

        if (GameManager.Instance != null)
            GameManager.Instance.GameEvent("initGame");   // back to Title; use "startGame" to retry instantly
        else
        {
            // Opened this scene directly in the editor: no GameManager exists.
            Time.timeScale = 1f;
            SceneManager.LoadScene(GameManager.TitleScene);
        }
    }
}
