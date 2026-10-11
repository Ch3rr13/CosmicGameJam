using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Keeps the score private so other scripts can't accidentally break it
    public static int currentScore = 0;

    // GETTER: Returns the current score (used by your Game Over screen)
    public int getScore()
    {
        return currentScore;
    }

    // SETTER: Overwrites the score with a specific number
    public void setScore(int newScore)
    {
        currentScore = newScore;
    }

    // HELPER: Adds points to the existing score (best for defeating enemies)
    public void addScore(int pointsToAdd)
    {
        currentScore += pointsToAdd;
    }

    // HELPER: Resets the score to zero when restarting the game
    public void resetScore()
    {
        currentScore = 0;
    }
}