using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject pressToStartIMG;
    public float flickerSpeed = 0.5f;
    [SerializeField] private UISpriteSequence transition;

    public event Action startGame;
    private float timer;
    private float acceptInputAt;
    private bool starting;

    private void OnEnable() => acceptInputAt = Time.unscaledTime + 0.25f;

    private void Update()
    {
        if (starting) return;
        if (pressToStartIMG != null)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= flickerSpeed)
            {
                pressToStartIMG.SetActive(!pressToStartIMG.activeSelf);
                timer = 0f;
            }
        }

        if (Time.unscaledTime >= acceptInputAt &&
            ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
             (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)))
            BeginGame();
    }

    public void BeginGame()
    {
        if (starting) return;
        starting = true;
        if (pressToStartIMG != null) pressToStartIMG.SetActive(false);
        if (transition != null) transition.TransitionTo(LoadGame);
        else LoadGame();
    }

    private void LoadGame()
    {
        startGame?.Invoke();
        GameManager.GetOrCreate().StartGame();
    }
}
