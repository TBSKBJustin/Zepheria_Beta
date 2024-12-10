using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundandLightControl : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Light sceneLight;

    void Start()
    {
        // Load saved settings
        LoadSettings();

        // Set the initial value of the volume slider to the current audio volume
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        // Set the initial value of the brightness slider to the current light intensity
        if (sceneLight != null)
        {
            brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
        }
    }

    void ChangeVolume(float value)
    {
        AudioListener.volume = value;  // Adjust audio volume
        SaveSettings();  // Save settings whenever the volume changes
    }

    void ChangeBrightness(float value)
    {
        if (sceneLight != null)
        {
            sceneLight.intensity = value;  // Adjust light intensity to match slider value
        }
        SaveSettings();  // Save settings whenever the brightness changes
    }

    // Save settings to PlayerPrefs
    void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);  // Save volume
        if (sceneLight != null)
        {
            PlayerPrefs.SetFloat("Brightness", sceneLight.intensity);  // Save brightness
        }
        PlayerPrefs.Save();  // Save all PlayerPrefs changes
    }

    // Load settings from PlayerPrefs
    void LoadSettings()
    {
        // Check if the settings exist
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");  // Get saved volume
            AudioListener.volume = savedVolume;  // Set AudioListener volume
            volumeSlider.value = savedVolume;  // Update slider value
        }
        
        if (PlayerPrefs.HasKey("Brightness") && sceneLight != null)
        {
            float savedBrightness = PlayerPrefs.GetFloat("Brightness");  // Get saved brightness
            sceneLight.intensity = savedBrightness;  // Set light intensity
            brightnessSlider.value = savedBrightness;  // Update slider value
        }
    }
}
