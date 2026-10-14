using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Flicker Settings")]
    public GameObject pressToStartIMG;
    public float flickerSpeed = 0.5f;

    [Header("Transition Settings")]
    public Animator transitionAnimator;
    public float transitionDuration = 1.5f; // Adjust to match your animation's length in seconds

    [Tooltip("Tick on the Title screen: pressing a key goes to the Tutorial first. Leave off in the Tutorial scene: it goes straight to the game.")]
    public bool tutorialNext = false;

    // GameTrigger listens to ONE of these (its Event Name must match) and tells GameManager what to load.
    public event Action showTutorial;   // Title -> Tutorial
    public event Action startGame;      // Tutorial -> Game

    private float timer = 0f;
    private bool isTransitioning = false; // Prevents overlapping starts

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
            StartCoroutine(PlayTransitionAndStart());
        }
    }

    private IEnumerator PlayTransitionAndStart()
    {
        // 1. Fire the animation
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("StartTransition");

        // 2. Wait for the animation to finish
        yield return new WaitForSeconds(transitionDuration);

        // 3. Hand off to GameManager (via GameTrigger), which loads the next scene
        if (tutorialNext)
            showTutorial?.Invoke();
        else
            startGame?.Invoke();
    }
}