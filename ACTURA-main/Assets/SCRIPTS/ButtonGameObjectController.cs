using UnityEngine;
using UnityEngine.UI;

public class ButtonGameObjectController : MonoBehaviour
{
    [Header("Assign Buttons (in order)")]
    [SerializeField] private Button[] buttons;

    [Header("Assign GameObjects (in same order)")]
    [SerializeField] private GameObject[] targets;

    void Start()
    {
        HideAll();

        if (buttons.Length != targets.Length)
        {
            Debug.LogError("Buttons and targets length mismatch!");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int idx = i;
            buttons[i].onClick.AddListener(() => ShowUpTo(idx));
        }
    }

    void HideAll()
    {
        foreach (var go in targets)
            if (go) go.SetActive(false);
    }

    void ShowUpTo(int index)
    {
        // Show all up to and including index, hide the rest
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i])
                targets[i].SetActive(i <= index);
        }
    }
}