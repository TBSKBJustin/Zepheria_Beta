using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    // References to the panels
    public GameObject MainPanel;
    public GameObject OptionsPanel;

    void Start()
    {
        // Ensure the OptionsPanel is disabled at the start
        if (OptionsPanel != null)
            OptionsPanel.SetActive(false);
    }

    // Called when the "Options" button is clicked
    public void ShowOptionsPanel()
    {
        if (MainPanel != null)
            MainPanel.SetActive(false);
        if (OptionsPanel != null)
            OptionsPanel.SetActive(true);
    }

    // Called when the "Back" button is clicked
    public void ShowMainPanel()
    {
        if (OptionsPanel != null)
            OptionsPanel.SetActive(false);
        if (MainPanel != null)
            MainPanel.SetActive(true);
    }
}
