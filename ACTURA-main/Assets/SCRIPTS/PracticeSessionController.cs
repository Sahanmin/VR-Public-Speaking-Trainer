using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PracticeSessionController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button applauseButton;
    [SerializeField] private Button distractionButton;
    [SerializeField] private Button letsPracticeButton;
    [SerializeField] private Button endButton;

    [Header("Audio Components")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Files")]
    [SerializeField] private AudioClip applauseSound;
    [SerializeField] private AudioClip distractionSound;

    [Header("Distraction Settings")]
    [SerializeField] private float initialInterval = 5f; // First distraction after 5 seconds
    [SerializeField] private float repeatInterval = 10f; // Then every 10 seconds

    // Track user selections
    private bool applauseSelected = false;
    private bool distractionSelected = false;
    private bool practiceStarted = false;

    // Coroutine reference to stop it when needed
    private Coroutine distractionCoroutine;

    void Start()
    {
        SetupButtons();
        ResetSession();
    }

    void SetupButtons()
    {
        applauseButton.onClick.AddListener(OnApplauseClicked);
        distractionButton.onClick.AddListener(OnDistractionClicked);
        letsPracticeButton.onClick.AddListener(OnLetsPracticeClicked);
        endButton.onClick.AddListener(OnEndClicked);
    }

    void OnApplauseClicked()
    {
        applauseSelected = !applauseSelected; // Toggle selection

        // Visual feedback (optional - change button color)
        UpdateButtonVisual(applauseButton, applauseSelected);

        Debug.Log("Applause selected: " + applauseSelected);
    }

    void OnDistractionClicked()
    {
        distractionSelected = !distractionSelected; // Toggle selection

        // Visual feedback (optional - change button color)
        UpdateButtonVisual(distractionButton, distractionSelected);

        Debug.Log("Distraction selected: " + distractionSelected);
    }

    void OnLetsPracticeClicked()
    {
        if (!practiceStarted)
        {
            StartPracticeSession();
        }
    }

    void OnEndClicked()
    {
        EndPracticeSession();
    }

    void StartPracticeSession()
    {
        practiceStarted = true;
        letsPracticeButton.interactable = false; // Disable the button

        Debug.Log("Practice session started!");

        // Start distraction sounds if selected
        if (distractionSelected && distractionSound != null)
        {
            distractionCoroutine = StartCoroutine(PlayDistractionSounds());
        }
    }

    void EndPracticeSession()
    {
        if (!practiceStarted)
        {
            Debug.Log("Practice hasn't started yet!");
            return;
        }

        // Stop distraction sounds
        if (distractionCoroutine != null)
        {
            StopCoroutine(distractionCoroutine);
            distractionCoroutine = null;
        }

        // Play applause sound if it was selected
        if (applauseSelected && applauseSound != null)
        {
            audioSource.PlayOneShot(applauseSound);
            Debug.Log("Playing applause sound!");
        }

        // Reset for next session
        StartCoroutine(ResetAfterDelay(2f)); // Wait 2 seconds before reset
    }

    IEnumerator PlayDistractionSounds()
    {
        // Wait for initial interval
        yield return new WaitForSeconds(initialInterval);

        // Play distraction sounds at intervals
        while (practiceStarted)
        {
            if (distractionSound != null)
            {
                audioSource.PlayOneShot(distractionSound);
                Debug.Log("Playing distraction sound!");
            }

            yield return new WaitForSeconds(repeatInterval);
        }
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetSession();
    }

    void ResetSession()
    {
        practiceStarted = false;
        applauseSelected = false;
        distractionSelected = false;

        letsPracticeButton.interactable = true;

        // Reset button visuals
        UpdateButtonVisual(applauseButton, false);
        UpdateButtonVisual(distractionButton, false);

        Debug.Log("Session reset!");
    }

    void UpdateButtonVisual(Button button, bool selected)
    {
        ColorBlock colors = button.colors;
        if (selected)
        {
            colors.normalColor = Color.green; // Selected color
        }
        else
        {
            colors.normalColor = Color.white; // Default color
        }
        button.colors = colors;
    }

    void OnDestroy()
    {
        // Clean up button listeners
        applauseButton?.onClick.RemoveListener(OnApplauseClicked);
        distractionButton?.onClick.RemoveListener(OnDistractionClicked);
        letsPracticeButton?.onClick.RemoveListener(OnLetsPracticeClicked);
        endButton?.onClick.RemoveListener(OnEndClicked);

        // Stop any running coroutines
        if (distractionCoroutine != null)
        {
            StopCoroutine(distractionCoroutine);
        }
    }
}