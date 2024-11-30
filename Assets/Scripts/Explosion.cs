using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioSource backgroundMusic;  // The background music AudioSource
    public AudioClip scaryMusic;  // Scary music clip (for after the explosion)
    public AudioSource explosionSound;  // Explosion sound effect AudioSource
    public AudioClip helpClip;  // Help sound clip (for after the explosion)
    public Light roomLight;  // Room light to darken
    public GameObject door;  // The door to shake
    
    private float shakeTime = 0.5f;  // Time for shaking the door
    private Vector3 originalPosition;  // Original door position for shaking

    void Start()
    {
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

        // Darken the room
        StartCoroutine(DarkenRoom());

        // Switch to scary music after a short delay
        Invoke("ChangeMusic", 1f);  // Change music after 1 second delay

        // Play the help sound after scary music starts
        Invoke("PlayHelpSound", 2f);  // Play the help sound after 2 seconds
    }

    IEnumerator ShakeDoor()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeTime)
        {
            float shakeAmount = Mathf.Sin(elapsedTime * Mathf.PI * 2f * 10f) * 0.02f;
            door.transform.position = new Vector3(originalPosition.x + shakeAmount, originalPosition.y, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.transform.position = originalPosition;  // Reset door's position
    }

    IEnumerator DarkenRoom()
    {
        float elapsedTime = 0f;
        float targetIntensity = 0.1f;
        float initialIntensity = roomLight.intensity;

        while (elapsedTime < 2f)
        {
            roomLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        roomLight.intensity = targetIntensity;
    }

    void ChangeMusic()
    {
        // Switch to scary music
        backgroundMusic.clip = scaryMusic;
        backgroundMusic.Play();
    }

    void PlayHelpSound()
    {
        // Play the help sound after the scary music starts
        explosionSound.PlayOneShot(helpClip);
    }
}
