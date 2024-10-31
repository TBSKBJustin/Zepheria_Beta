using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Access the Slider class
using UnityEngine.Audio;  // Add this to access the AudioMixer class

public class SoundControl : MonoBehaviour
{
    public AudioMixer MainAudioMixer;  // Reference to your AudioMixer
    public Slider VolumeSlider;    // Reference to your UI slider

    // Function to set volume, called by slider value changes
    public void SetVolume(float volume)
    {
        MainAudioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // Adjust volume logarithmically
    }
}
