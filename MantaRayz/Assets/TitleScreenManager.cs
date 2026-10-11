using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Flicker Settings")]
    public GameObject pressToStartIMG; // Assign your image here in the Inspector
    public float flickerSpeed = 0.5f;  // How fast it blinks in seconds
    private GameManager gameManager;

    public event Action startGame;
    private float timer = 0f;

    void Update()
    {
        // 1. Handle the flickering
        if (pressToStartIMG != null)
        {
            timer += Time.deltaTime;
            if (timer >= flickerSpeed)
            {
                // Toggles the active state to the opposite of what it currently is
                pressToStartIMG.SetActive(!pressToStartIMG.activeSelf);
                timer = 0f;
            }
        }

        // 2. Handle the scene transition input
        bool anyKeyPressed = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || 
                             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (anyKeyPressed)
        {
            startGame?.Invoke();


            SceneManager.LoadScene("Game Scene"); 
        }
    }
}

/*
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
*/