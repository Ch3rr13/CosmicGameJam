using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Must match the scene FILE names exactly (and be in Build Settings).
    public const string TitleScene = "Title Screen Scene";
    public const string GameScene = "Game Scene";
    public const string GameOverScene = "Game Over Scene";
    public const string TutorialScene = "Tutorial Scene";

    public static GameManager Instance { get; private set; }

    public event Action gameInit;
    public event Action gameStart;
    public event Action gameOver;

    // Score: persists across scenes so the Game Over scene can show it.
    public int Score { get; private set; }
    public event Action<int> scoreChanged;

    [Tooltip("On: the tutorial is only shown the first time per play session")]
    [SerializeField] private bool skipTutorialAfterFirstView = false;

    private bool isGameOver = false;
    private bool tutorialSeen = false;

    private void Awake()
    {
        // Persistent singleton: extra copies (e.g. the one in Game_Scene) destroy themselves.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called by GameTrigger / other scripts with one of these keys.
    public void GameEvent(string gameevent)
    {
        switch (gameevent)
        {
            case "initGame": InitGame(); break;
            case "showTutorial": ShowTutorial(); break;
            case "startGame": StartGame(); break;
            case "gameOver": GameOver(); break;
            default:
                Debug.LogWarning($"GameManager: unknown game event '{gameevent}'");
                break;
        }
    }

    // Go to the title screen.
    public void InitGame()
    {
        ResetState();
        gameInit?.Invoke();
        SceneManager.LoadScene(TitleScene);
    }

    // Title -> Tutorial. The Tutorial scene then fires "startGame" to begin the run.
    public void ShowTutorial()
    {
        if (skipTutorialAfterFirstView && tutorialSeen)
        {
            StartGame();
            return;
        }

        tutorialSeen = true;
        ResetState();
        SceneManager.LoadScene(TutorialScene);
    }

    // Start (or restart) a run.
    public void StartGame()
    {
        Debug.Log("started game");
        ResetState();
        Score = 0;
        scoreChanged?.Invoke(Score);
        gameStart?.Invoke();
        SceneManager.LoadScene(GameScene);
    }

    public void GameOver()
    {
        if (isGameOver) return;   // ignore duplicate deaths in the same run
        isGameOver = true;

        Debug.Log("GAME OVER");
        gameOver?.Invoke();
        SceneManager.LoadScene(GameOverScene);
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;   // no points after death
        Score += amount;
        scoreChanged?.Invoke(Score);
    }

    private void ResetState()
    {
        isGameOver = false;       // without this, only the first death ever works
        Time.timeScale = 1f;      // never leave the game frozen between scenes
    }
}