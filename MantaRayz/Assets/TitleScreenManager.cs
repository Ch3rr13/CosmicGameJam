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

    // GameTrigger (eventName = "startGame") listens to this and tells GameManager to load the game.
    public event Action startGame;

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

        // 3. Hand off to GameManager (via GameTrigger), which loads Game_Scene
        startGame?.Invoke();
    }
}