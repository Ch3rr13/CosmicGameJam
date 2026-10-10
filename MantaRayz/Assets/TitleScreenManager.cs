using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleScreenManager : MonoBehaviour
{
    void Update()
    {
        // Checks for keyboard presses or a left mouse click
        bool anyKeyPressed = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || 
                             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (anyKeyPressed)
        {
            SceneManager.LoadScene("Background ScrollView"); 
        }
    }
}