using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Flicker Settings")]
    public GameObject pressToStartIMG; 
    public float flickerSpeed = 0.5f;  
    
    [Header("Transition Settings")]
    public Animator transitionAnimator; 
    public float transitionDuration = 1.5f; // Adjust to match your animation's length in seconds
    public string sceneToLoad = "Background ScrollView";

    public event Action startGame;
    private float timer = 0f;
    private bool isTransitioning = false; // Prevents overlapping loads

    void Update()
    {
        // Stop the flicker and input checking once the transition begins
        if (isTransitioning) return;

        if (pressToStartIMG != null)
        {
            timer += Time.deltaTime;
            if (timer >= flickerSpeed)
            {
                pressToStartIMG.SetActive(!pressToStartIMG.activeSelf);
                timer = 0f;
            }
        }

        bool anyKeyPressed = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || 
                             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (anyKeyPressed)
        {
            isTransitioning = true;
            startGame?.Invoke();
            StartCoroutine(PlayTransitionAndLoad());
        }
    }

    private IEnumerator PlayTransitionAndLoad()
    {
        // 1. Fire the animation
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransition");
        }

        // 2. Wait for the exact length of the animation
        yield return new WaitForSeconds(transitionDuration);

        // 3. Load the actual game
        SceneManager.LoadScene(sceneToLoad);
    }
}

/*
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


            //SceneManager.LoadScene("Background ScrollView"); 
        }
    }
}


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