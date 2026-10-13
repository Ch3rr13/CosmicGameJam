using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient gradient;
    [SerializeField] private float smoothSpeed = 5f;

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
        targetValue = max > 0f ? Mathf.Clamp01(current / max) : 0f;
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
