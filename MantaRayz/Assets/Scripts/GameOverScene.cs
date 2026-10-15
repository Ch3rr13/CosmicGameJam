using UnityEngine;
using TMPro;

public class GameOverScoreText : MonoBehaviour
{
    [Header("References")]
    //public ScoreManager scoreManager; // Your existing script with getScore()
    public TextMeshProUGUI gameOverText; // Your actual Game Over text component

    [Header("Text Formatting")]
    public string baseText = "GAME OVER\nScore: ";

    void OnEnable()
    {
        // Updates the text immediately when the Game Over screen turns on
        if (gameOverText != null)
        {
            gameOverText.text = baseText + ScoreManager.currentScore.ToString();
        }
    }
}
