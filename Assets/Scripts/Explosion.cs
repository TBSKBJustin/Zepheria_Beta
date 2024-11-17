using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
   public AudioSource backgroundMusic;  // The background music AudioSource
    public AudioClip scaryMusic;  // Scary music clip (for after the explosion)
    public AudioSource explosionSound;  // Explosion sound effect AudioSource
    public Light roomLight;  // Room light to darken
    public GameObject door;  // The door to shake
    
    private float shakeTime = 0.5f;  // Time for shaking the door
    private Vector3 originalPosition;  // Original door position for shaking

    void Start()
    {
        // Set the initial calm music (you can set this directly in the inspector) and play it
        originalPosition = door.transform.position;

        // Start the sequence after 6 seconds
        Invoke("TriggerExplosion", 6f);
    }

    void TriggerExplosion()
    {
        // Play explosion sound
        explosionSound.Play();

        // Start shaking the door
        StartCoroutine(ShakeDoor());

        // Darken the room (optional)
        StartCoroutine(DarkenRoom());

        // Switch to scary music after a short delay
        Invoke("ChangeMusic", 1f);  // Change music after 1 second delay
    }

    IEnumerator ShakeDoor()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeTime)
        {
            // Shake the door slightly side to side (only along the X-axis)
            float shakeAmount = Mathf.Sin(elapsedTime * Mathf.PI * 2f * 10f) * 0.02f;  // Small side-to-side shake
            door.transform.position = new Vector3(originalPosition.x + shakeAmount, originalPosition.y, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the door to its original position
        door.transform.position = originalPosition;
    }

    IEnumerator DarkenRoom()
    {
        float elapsedTime = 0f;
        float targetIntensity = 0.1f;  // Low light intensity
        float initialIntensity = roomLight.intensity;

        // Gradually darken the room
        while (elapsedTime < 2f)  // Darken over 2 seconds
        {
            roomLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        roomLight.intensity = targetIntensity;  // Ensure it's fully darkened
    }

    void ChangeMusic()
    {
        // Switch to scary music
        backgroundMusic.clip = scaryMusic;
        backgroundMusic.Play();
    }
}
