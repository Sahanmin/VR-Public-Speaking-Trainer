using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ButtonSceneMapping
{
    [Header("Button and Scene")]
    public Button sceneButton;
    [Tooltip("Name of the scene to load (must match exactly with scene name in Build Settings)")]
    public string sceneName;
    [Tooltip("Optional: Scene description for UI feedback")]
    public string sceneDescription;

    [Header("Visual Feedback")]
    [Tooltip("Color when this scene is selected")]
    public Color selectedColor = Color.green;
    [Tooltip("Color when this scene is not selected")]
    public Color normalColor = Color.white;

    [HideInInspector]
    public bool isSelected = false;
}

public class SceneManager : MonoBehaviour
{
    [Header("Scene Selection Buttons")]
    [SerializeField] private ButtonSceneMapping[] sceneButtons = new ButtonSceneMapping[4];

    [Header("Load Scene Button")]
    [SerializeField] private Button loadSceneButton;
    [Tooltip("Text component to show selected scene name (optional)")]
    [SerializeField] private Text selectedSceneText;

    [Header("Settings")]
    [SerializeField] private bool allowDeselectAll = false;
    [SerializeField] private bool autoLoadOnSelection = false;

    private int selectedSceneIndex = -1; // -1 means no selection

    private void Start()
    {
        // Setup button listeners
        SetupSceneButtonListeners();
        SetupLoadButtonListener();

        // Initialize UI
        UpdateUI();
    }

    private void SetupSceneButtonListeners()
    {
        for (int i = 0; i < sceneButtons.Length; i++)
        {
            if (sceneButtons[i].sceneButton != null)
            {
                int index = i; // Capture index for lambda
                sceneButtons[i].sceneButton.onClick.AddListener(() => SelectScene(index));
                Debug.Log($"Scene selection button listener added for index {i}: {sceneButtons[i].sceneName}");
            }
            else
            {
                Debug.LogWarning($"Scene button at index {i} is not assigned!");
            }
        }
    }

    private void SetupLoadButtonListener()
    {
        if (loadSceneButton != null)
        {
            loadSceneButton.onClick.AddListener(() => LoadSelectedScene());
            Debug.Log("Load scene button listener added");
        }
        else
        {
            Debug.LogWarning("Load Scene Button is not assigned!");
        }
    }

    public void SelectScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= sceneButtons.Length)
        {
            Debug.LogError($"Invalid scene index: {sceneIndex}");
            return;
        }

        // Check if clicking the same button
        if (selectedSceneIndex == sceneIndex)
        {
            if (allowDeselectAll)
            {
                // Deselect current selection
                selectedSceneIndex = -1;
                Debug.Log("Scene deselected");
            }
            else
            {
                // If auto-load is enabled, load immediately
                if (autoLoadOnSelection)
                {
                    LoadSelectedScene();
                    return;
                }
            }
        }
        else
        {
            // Select new scene
            selectedSceneIndex = sceneIndex;
            Debug.Log($"Scene selected: {sceneButtons[sceneIndex].sceneName}");

            // If auto-load is enabled, load immediately
            if (autoLoadOnSelection)
            {
                LoadSelectedScene();
                return;
            }
        }

        // Update visual feedback
        UpdateUI();
    }

    public void LoadSelectedScene()
    {
        if (selectedSceneIndex == -1)
        {
            Debug.LogWarning("No scene selected! Please select a scene first.");
            if (selectedSceneText != null)
            {
                selectedSceneText.text = "Please select a scene first!";
            }
            return;
        }

        var selectedScene = sceneButtons[selectedSceneIndex];

        if (string.IsNullOrEmpty(selectedScene.sceneName))
        {
            Debug.LogError($"Scene name is empty for button at index {selectedSceneIndex}!");
            return;
        }

        Debug.Log($"Loading scene: {selectedScene.sceneName}");

        // Check if scene exists in build settings
        if (Application.CanStreamedLevelBeLoaded(selectedScene.sceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(selectedScene.sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{selectedScene.sceneName}' is not found in Build Settings! Make sure to add it to Build Settings.");
            if (selectedSceneText != null)
            {
                selectedSceneText.text = $"Error: Scene '{selectedScene.sceneName}' not found!";
            }
        }
    }

    private void UpdateUI()
    {
        // Update button colors and selection states
        for (int i = 0; i < sceneButtons.Length; i++)
        {
            if (sceneButtons[i].sceneButton != null)
            {
                bool isSelected = (i == selectedSceneIndex);
                sceneButtons[i].isSelected = isSelected;

                // Update button color
                var buttonImage = sceneButtons[i].sceneButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = isSelected ? sceneButtons[i].selectedColor : sceneButtons[i].normalColor;
                }
            }
        }

        // Update selected scene text
        if (selectedSceneText != null)
        {
            if (selectedSceneIndex >= 0)
            {
                string displayText = !string.IsNullOrEmpty(sceneButtons[selectedSceneIndex].sceneDescription)
                    ? sceneButtons[selectedSceneIndex].sceneDescription
                    : sceneButtons[selectedSceneIndex].sceneName;
                selectedSceneText.text = $"Selected: {displayText}";
            }
            else
            {
                selectedSceneText.text = "No scene selected";
            }
        }

        // Update load button interactability
        if (loadSceneButton != null)
        {
            loadSceneButton.interactable = (selectedSceneIndex >= 0);
        }
    }

    // Public methods for external control
    public void SelectSceneByIndex(int index)
    {
        SelectScene(index);
    }

    public void SelectSceneByName(string sceneName)
    {
        for (int i = 0; i < sceneButtons.Length; i++)
        {
            if (sceneButtons[i].sceneName == sceneName)
            {
                SelectScene(i);
                return;
            }
        }
        Debug.LogWarning($"Scene with name '{sceneName}' not found in the scene buttons!");
    }

    public string GetSelectedSceneName()
    {
        if (selectedSceneIndex >= 0)
        {
            return sceneButtons[selectedSceneIndex].sceneName;
        }
        return null;
    }

    public bool HasSceneSelected()
    {
        return selectedSceneIndex >= 0;
    }

    public void ClearSelection()
    {
        selectedSceneIndex = -1;
        UpdateUI();
    }

    // Load scene directly by index (bypassing selection)
    public void LoadSceneDirectly(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < sceneButtons.Length)
        {
            SelectScene(sceneIndex);
            LoadSelectedScene();
        }
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        foreach (var sceneButton in sceneButtons)
        {
            if (sceneButton.sceneButton != null)
            {
                sceneButton.sceneButton.onClick.RemoveAllListeners();
            }
        }

        if (loadSceneButton != null)
        {
            loadSceneButton.onClick.RemoveAllListeners();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensure we always have 4 scene buttons
        if (sceneButtons.Length != 4)
        {
            System.Array.Resize(ref sceneButtons, 4);
        }
    }
#endif
}