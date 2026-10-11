using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Keeps the score private so other scripts can't accidentally break it
    public static int currentScore = 0;

    // GETTER: Returns the current score (used by your Game Over screen)
    public static int getScore()
    {
        return currentScore;
    }

    // SETTER: Overwrites the score with a specific number
    public static void setScore(int newScore)
    {
        currentScore = newScore;
    }

    // HELPER: Adds points to the existing score (best for defeating enemies)
// HELPER: Adds points to the existing score (best for defeating enemies)
    public static void addScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;

        // Add ".Instance." to target the active UI object
        if (ScoreOverlay.Instance != null)
        {
            ScoreOverlay.Instance.UpdateScoreDisplay(currentScore);
        }
    }

    // HELPER: Resets the score to zero when restarting the game
    public static void resetScore()
    {
        currentScore = 0;

        if (ScoreOverlay.Instance != null)
        {
            ScoreOverlay.Instance.UpdateScoreDisplay(currentScore);
        }
    }
}