using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Must match the scene FILE names exactly (and be in Build Settings).
    public const string TitleScene = "Title Screen Scene";
    public const string GameScene = "Game Scene";
    public const string GameOverScene = "Game Over Scene";

    public static GameManager Instance { get; private set; }

    public event Action gameInit;
    public event Action startGame;
    public event Action gameOver;

    private bool isGameOver = false;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static GameManager GetOrCreate()
    {
        if (Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
        return Instance;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameScene) return;
        ResetState();
        ScoreManager.resetScore();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    // Called by GameTrigger / other scripts with one of these keys.
    public void GameEvent(string gameevent)
    {
        switch (gameevent)
        {
            case "initGame": InitGame(); break;
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

    // Start (or restart) a run.
    public void StartGame()
    {
        Debug.Log("started game");
        
        ResetState();
        
        SceneManager.LoadScene(GameScene);
        startGame?.Invoke();
    }

    public void GameOver()
    {
        if (isGameOver) return;   // ignore duplicate deaths in the same run
        isGameOver = true;
        Time.timeScale = 1f;

        Debug.Log("GAME OVER");
        
        SceneManager.LoadScene(GameOverScene);
        gameOver?.Invoke();
    }

    private void ResetState()
    {
        isGameOver = false;       // without this, only the first death ever works
        Time.timeScale = 1f;      // never leave the game frozen between scenes
    }
}
