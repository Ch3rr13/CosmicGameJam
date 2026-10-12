using UnityEngine;
using UnityEngine.UI; // Required for standard UI Images

public class ScoreOverlay : MonoBehaviour
{
    public static ScoreOverlay Instance;

    private void Awake()
    {
        Instance = this;
        
    }

    [Header("Assign 0-9 Sprites Here")]
    [Tooltip("Make sure Index 0 is the '0' image, Index 1 is the '1' image, etc.")]
    public Sprite[] digitSprites = new Sprite[10]; 

    [Header("Assign UI Image Slots Here (Left to Right)")]
    public Image[] digitSlots;

    void Start()
    {
        ScoreManager.resetScore();
        //ScoreManager.setScore(1234); //For testing purposes delete later
        UpdateScoreDisplay(ScoreManager.currentScore);
    }
    // Call this method whenever the player's score changes
    
    public void UpdateScoreDisplay(int newScore)
    {
        // Forces the string to always be exactly the length of your slots (e.g., "0000")
        string scoreString = newScore.ToString().PadLeft(digitSlots.Length, '0');

        for (int i = 0; i < digitSlots.Length; i++)
        {
            digitSlots[i].enabled = true; 
        
            int currentDigit = int.Parse(scoreString[i].ToString());
            digitSlots[i].sprite = digitSprites[currentDigit];
        }
    }
}