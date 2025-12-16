using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioVolumeController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText; // Optional - remove if you don't want text display

    [Header("Audio Component")]
    [SerializeField] private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private string textFormat = "Volume: {0:P0}"; // Shows as percentage (e.g., "Volume: 50%")

    void Start()
    {
        // Setup slider range (0 to 1 for audio volume)
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = audioSource.volume;

        // Add listener for slider changes
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Initialize the display
        UpdateDisplay();
    }

    void OnVolumeChanged(float value)
    {
        // Update audio source volume
        audioSource.volume = value;

        // Update text display (if you have text component)
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // Only update text if volumeText is assigned
        if (volumeText != null)
        {
            volumeText.text = string.Format(textFormat, volumeSlider.value);
        }
    }

    void OnDestroy()
    {
        // Clean up listener
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}