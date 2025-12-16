using UnityEngine;
using UnityEngine.UI;

public class SimpleToggleObjects : MonoBehaviour
{
    [Header("Buttons")]
    public Button selectionButton;
    public Button actionButton;

    [Header("Objects to Show")]
    public GameObject[] objectsToShow;

    [Header("Objects to Hide")]
    public GameObject[] objectsToHide;

    [Header("Visual Feedback")]
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    [Header("UI Feedback")]
    public Text statusText;

    private bool isSelected = false;

    void Start()
    {
        // Setup button listeners
        if (selectionButton) selectionButton.onClick.AddListener(ToggleSelection);
        if (actionButton) actionButton.onClick.AddListener(ExecuteAction);

        UpdateUI();
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        UpdateUI();

        Debug.Log($"Selection: {(isSelected ? "Selected" : "Not Selected")}");
    }

    public void ExecuteAction()
    {
        if (!isSelected)
        {
            Debug.Log("Nothing selected - no action taken");
            if (statusText) statusText.text = "Please select first!";
            return;
        }

        Debug.Log("Executing action - toggling objects");

        // Show the objects
        foreach (GameObject obj in objectsToShow)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Showing: {obj.name}");
            }
        }

        // Hide the objects
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"Hiding: {obj.name}");
            }
        }

        if (statusText) statusText.text = "Action completed!";
    }

    void UpdateUI()
    {
        // Update selection button color
        if (selectionButton != null)
        {
            Image buttonImage = selectionButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = isSelected ? selectedColor : normalColor;
            }
        }

        // Update action button
        if (actionButton != null)
        {
            actionButton.interactable = isSelected;
        }

        // Update status text
        if (statusText != null)
        {
            statusText.text = isSelected ? "Ready to execute" : "Click to select";
        }
    }

    // Public methods for external use
    public void ResetSelection()
    {
        isSelected = false;
        UpdateUI();
    }

    public void ForceExecute()
    {
        isSelected = true;
        ExecuteAction();
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}