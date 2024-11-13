using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlimeTally : MonoBehaviour
{
    public TextMeshProUGUI slimeCountText; // Assign in the Inspector
    private int slimeCount = 0;

    private void Start()
    {
        UpdateSlimeCountUI();  // Update the UI text on start
    }

    public void IncrementSlimeCount()
    {
        slimeCount++;  // Increment slime count
        UpdateSlimeCountUI();  // Update UI to reflect new count
    }

    private void UpdateSlimeCountUI()
    {
        slimeCountText.text = "Slimes Collected: " + slimeCount;  // Display the slime count
    }
}
