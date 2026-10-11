using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class GameOverSceneManager : MonoBehaviour
{
    public event Action initGame;
    void Update()
    {
        // Checks for keyboard presses or a left mouse click
        bool anyKeyPressed = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || 
                             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (anyKeyPressed)
        {
            initGame?.Invoke();
            
            SceneManager.LoadScene("Game Scene"); 
        }
    }
}