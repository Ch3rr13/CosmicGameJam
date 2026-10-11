using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject youDiedImage;

    private bool isGameOver = false;
    public event Action gameOver;

    private void Start()
    {
        Time.timeScale = 1f;

        if (youDiedImage != null)
            youDiedImage.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER");

        Time.timeScale = 0f;        // stop the game first, even if the image is missing
        gameOver?.Invoke();

        if (youDiedImage == null)
        {
            Debug.LogError("You Died Image is not assigned!");
            return;
        }

        youDiedImage.SetActive(true);
    }
}