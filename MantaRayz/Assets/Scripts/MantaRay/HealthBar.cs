using UnityEngine;
using UnityEngine.UI;

// Put on the Slider. Set Fill Image and Gradient in the Inspector.
// Gradient: left = 0% health (red), right = 100% health (green).
// Call SetHealth(current, max) whenever health changes.
[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient gradient;       // set in the Inspector: left = red, right = green
    [SerializeField] private float drainSpeed = 2f;   // bar widths per second (2 = empties from full in half a second)

    private Slider slider;
    private float targetValue = 1f;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        UpdateColor();
    }

    public void SetHealth(float current, float max)
    {
        targetValue = Mathf.Clamp01(current / max);
    }

    private void Update()
    {
        // Moves at a steady speed and lands exactly on the target, including the final hit.
        // Unscaled time, so it keeps moving when the game freezes on game over.
        slider.value = Mathf.MoveTowards(slider.value, targetValue, drainSpeed * Time.unscaledDeltaTime);
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (fillImage == null) return;

        fillImage.color = gradient.Evaluate(slider.normalizedValue);

        // Fully empty means nothing drawn at all (hides the leftover red sliver at 0).
        fillImage.enabled = slider.normalizedValue > 0.001f;
    }
}