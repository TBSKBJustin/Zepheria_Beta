using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    // References to the panels
    [Header("UI Panels")]
    public GameObject MainPanel;
    public GameObject OptionsPanel;

    void Start()
    {
        // Ensure only the MainPanel is visible at the start
        if (MainPanel != null)
            MainPanel.SetActive(true);

        if (OptionsPanel != null)
            OptionsPanel.SetActive(false);
    }

    // Show the OptionsPanel and hide the MainPanel
    public void ShowOptionsPanel()
    {
        if (MainPanel != null)
            MainPanel.SetActive(false);

        if (OptionsPanel != null)
            OptionsPanel.SetActive(true);
    }

    // Show the MainPanel and hide the OptionsPanel
    public void ShowMainPanel()
    {
        if (OptionsPanel != null)
            OptionsPanel.SetActive(false);

        if (MainPanel != null)
            MainPanel.SetActive(true);
    }
}
