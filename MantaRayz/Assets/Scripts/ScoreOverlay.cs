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
        // Convert the score to a string without adding extra zeros
        string scoreString = newScore.ToString();

        // Calculate where we should start drawing numbers to keep them aligned to the right
        int emptySlots = digitSlots.Length - scoreString.Length;

        for (int i = 0; i < digitSlots.Length; i++)
        {
            /*
            if (i < emptySlots)
            {
                // Hide the image component if this slot isn't needed yet
                digitSlots[i].enabled = false;
            }
            else
            */
            
            // Turn the image on and assign the correct digit sprite
            digitSlots[i].enabled = true;
            
            // Get the exact character from the score string based on the offset
            int currentDigit = int.Parse(scoreString[i - emptySlots].ToString());
            digitSlots[i].sprite = digitSprites[currentDigit];
            
        }
    }
}