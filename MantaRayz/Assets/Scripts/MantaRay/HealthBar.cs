using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;   
    [SerializeField] private Gradient gradient; // set green -> red in the Inspector
    [SerializeField] private float smoothSpeed = 5f;

    private Slider slider;
    private float targetValue = 1f;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Fallback so it works even if you forget to set the gradient.
        if (gradient == null || gradient.colorKeys.Length == 0)
        {
            gradient = new Gradient();
            gradient.SetKeys(
                new[] {
                    new GradientColorKey(Color.red, 0f),
                    new GradientColorKey(Color.yellow, 0.5f),
                    new GradientColorKey(Color.green, 1f)
                },
                new[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                });
        }
        UpdateColor();
    }

    public void SetHealth(float current, float max)
    {
        targetValue = Mathf.Clamp01(current / max);
    }

    private void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (fillImage != null)
            fillImage.color = gradient.Evaluate(slider.normalizedValue);
    }

}