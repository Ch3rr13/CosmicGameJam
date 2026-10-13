using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int currentScore = 0;

    public static int getScore() => currentScore;

    public static void setScore(int newScore)
    {
        currentScore = Mathf.Max(0, newScore);
        if (ScoreOverlay.Instance != null)
            ScoreOverlay.Instance.UpdateScoreDisplay(currentScore);
    }

    public static void addScore(int pointsToAdd)
    {
        setScore((int)System.Math.Clamp((long)currentScore + pointsToAdd, 0L, int.MaxValue));
    }

    public static void resetScore() => setScore(0);
}
