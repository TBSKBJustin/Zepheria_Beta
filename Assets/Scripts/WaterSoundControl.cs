using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSoundControl : MonoBehaviour
{
     public AudioSource waterSound;  // Reference to the AudioSource component
    public Transform player;  // Reference to the player (to measure the distance)
    public float maxDistance = 20f;  // The maximum distance at which the sound is still heard
    public float minVolume = 0.1f;  // Minimum volume at the max distance
    public float maxVolume = 1f;  // Maximum volume at the closest distance

    void Update()
    {
        // Calculate the distance between the player and the water (river)
        float distance = Vector3.Distance(player.position, transform.position);

        // Adjust the volume based on the distance
        float volume = Mathf.Clamp01(1 - (distance / maxDistance));  // Normalize the distance

        // Scale the volume to the desired range (between minVolume and maxVolume)
        waterSound.volume = Mathf.Lerp(minVolume, maxVolume, volume);
    }
}
