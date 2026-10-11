
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using Unity.Collections.Tests.CoreCLR.TestJobs;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject youDiedImage;

    private Scene mainMenu;
    private Scene levelScene;
    private Scene gameOverScene;
    

    private String mainMenuEvent;
    public static GameManager Instance { get; private set; }


    private bool isGameOver = false;
    public event Action gameOver;
    public event Action gameStart;
    public event Action gameInit;

    private EventInfo boundStartEvent;
    private Action startListener;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        mainMenu = SceneManager.GetSceneByName("Title Screen Scene");
        gameOverScene = SceneManager.GetSceneByName("Game Over Scene");

        levelScene = SceneManager.GetSceneByName("Background ScrollView");

        
    }
    public void GameEvent(string gameevent){
        switch (gameevent){
            case "initGame":
                InitGame();
                break;
            case "startGame":
                StartGame();
                break;
            case "gameOver":
                GameOver();
                break;
        }
    }
    void OnEnable()
    {

        Debug.Log("gamemanager enabled!");
        if (mainMenu == null || string.IsNullOrEmpty(mainMenuEvent))
            return;
        
        boundStartEvent = mainMenu.GetType().GetEvent(
            mainMenuEvent,
            BindingFlags.Public | BindingFlags.Instance
        );

        if (boundStartEvent == null ||
            boundStartEvent.EventHandlerType != typeof(Action))
        {
            Debug.LogWarning(
                $"GameManager: Invalid event {mainMenuEvent}",
                this
            );

            boundStartEvent = null;
            return;
        }

        startListener = OnStartTriggered;

        boundStartEvent.AddEventHandler(mainMenu, startListener);

        InitGame();
    }

    public void InitGame()
    {
        SceneManager.SetActiveScene(mainMenu);
        SceneManager.UnloadSceneAsync(levelScene);
        gameInit?.Invoke();
    }
    private void StartGame()
    {
        Debug.Log("started gakme");
        Time.timeScale = 1f;
        gameStart?.Invoke();
        if (youDiedImage != null)
            youDiedImage.SetActive(false);
        SceneManager.LoadScene(levelScene.name);
        SceneManager.SetActiveScene(levelScene);
        SceneManager.UnloadSceneAsync(mainMenu);
    }
    private void OnStartTriggered()
    {
        Time.timeScale = 1f;
        gameStart?.Invoke();
        if (youDiedImage != null)
            youDiedImage.SetActive(false);
        
        SceneManager.SetActiveScene(levelScene);
        SceneManager.UnloadSceneAsync(mainMenu);

    }




    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER");
        gameOver?.Invoke();

        /*if (youDiedImage == null)
        {
            Debug.LogError("You Died Image is not assigned!");
            return;
        }

        youDiedImage.SetActive(true);
        Debug.Log("You Died Image activated: " + youDiedImage.activeSelf);*/
        SceneManager.LoadScene(gameOverScene.name);
        SceneManager.SetActiveScene(gameOverScene);
        SceneManager.UnloadSceneAsync(levelScene);        

        Time.timeScale = 0f;
    }

}
