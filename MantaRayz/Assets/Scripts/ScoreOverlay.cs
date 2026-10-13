using UnityEngine;
using UnityEngine.UI;

public class ScoreOverlay : MonoBehaviour
{
    public static ScoreOverlay Instance { get; private set; }
    [Tooltip("Sprites for 0 through 9, in that order.")]
    public Sprite[] digitSprites = new Sprite[10];
    public Image[] digitSlots;

    private void OnEnable()
    {
        Instance = this;
        UpdateScoreDisplay(ScoreManager.currentScore);
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (digitSlots == null || digitSlots.Length == 0 || digitSprites == null || digitSprites.Length < 10) return;
        // Saturate the visible counter instead of silently losing the last digits.
        string digits = Mathf.Max(0, newScore).ToString();
        digits = digits.Length > digitSlots.Length
            ? new string('9', digitSlots.Length)
            : digits.PadLeft(digitSlots.Length, '0');

        for (int i = 0; i < digitSlots.Length; i++)
        {
            if (digitSlots[i] == null) continue;
            digitSlots[i].sprite = digitSprites[digits[i] - '0'];
            digitSlots[i].enabled = true;
        }
    }
}
