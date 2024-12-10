using UnityEngine;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    [Header("UI References")]
    public Scrollbar brightnessScrollbar;
    public Scrollbar volumeScrollbar;

    [Header("Scene Settings")]
    public Light directionalLight; // Drag the main scene light here.
    [Range(0, 1)] public float defaultBrightness = 0.5f;
    [Range(0, 1)] public float defaultVolume = 0.5f;

    void Start()
    {
        // Initialize brightness and volume
        if (brightnessScrollbar != null)
        {
            brightnessScrollbar.value = defaultBrightness;
            SetBrightness(defaultBrightness);
        }

        if (volumeScrollbar != null)
        {
            volumeScrollbar.value = defaultVolume;
            SetVolume(defaultVolume);
        }

        // Add listeners to the scrollbars
        if (brightnessScrollbar != null)
            brightnessScrollbar.onValueChanged.AddListener(SetBrightness);

        if (volumeScrollbar != null)
            volumeScrollbar.onValueChanged.AddListener(SetVolume);
    }

    public void SetBrightness(float value)
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(0.1f, 2f, value); // Adjust min/max values as needed
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value; // Sets global volume
    }

    void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        if (brightnessScrollbar != null)
            brightnessScrollbar.onValueChanged.RemoveListener(SetBrightness);

        if (volumeScrollbar != null)
            volumeScrollbar.onValueChanged.RemoveListener(SetVolume);
    }
}
