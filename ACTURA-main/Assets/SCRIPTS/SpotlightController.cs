using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpotlightController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider intensitySlider;
    [SerializeField] private TextMeshProUGUI intensityText;

    [Header("Light Component")]
    [SerializeField] private Light spotlight;

    [Header("Settings")]
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 200f;
    [SerializeField] private string textFormat = "Intensity: {0:F1}"; // Shows one decimal place

    void Start()
    {
        // Setup slider range
        intensitySlider.minValue = minIntensity;
        intensitySlider.maxValue = maxIntensity;
        intensitySlider.value = spotlight.intensity;

        // Add listener for slider changes
        intensitySlider.onValueChanged.AddListener(OnSliderValueChanged);

        // Initialize the display
        UpdateDisplay();
    }

    void OnSliderValueChanged(float value)
    {
        // Update spotlight intensity
        spotlight.intensity = value;

        // Update text display
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        intensityText.text = string.Format(textFormat, intensitySlider.value);
    }

    void OnDestroy()
    {
        // Clean up listener
        if (intensitySlider != null)
        {
            intensitySlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}