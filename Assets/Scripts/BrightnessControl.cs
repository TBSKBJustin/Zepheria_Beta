using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessControl : MonoBehaviour
{
    public Light sceneLight; // Reference to the light source (e.g., directional light)
    public Slider brightnessSlider;

    void Start()
    {
        // Set the slider to current brightness
        brightnessSlider.value = sceneLight.intensity;
        // Add listener for slider changes
        brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
    }

    // Change brightness based on slider
    void ChangeBrightness(float value)
    {
        sceneLight.intensity = value;
    }
}
