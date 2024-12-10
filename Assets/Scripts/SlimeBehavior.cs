using System.Collections;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    public float detectionRadius = 5f; // Distance within which the slime stops bouncing
    public float bounceHeight = 0.5f;  // Height of each bounce
    public float bounceSpeed = 2f;     // Speed of bounce
    public AudioClip bounceSound;      // Sound to play during bouncing
    public Camera mainCamera;          // Assign this in the Inspector to track the player’s camera

    private Vector3 originalPosition;
    private AudioSource audioSource;
    private bool isNearPlayer = false; // Whether the player is within range
    private Coroutine bounceCoroutine; // Reference to the bounce coroutine

    void Start()
    {
        // Store the slime's original position
        originalPosition = transform.position;

        // Set up the audio source for the bounce sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bounceSound;
        audioSource.loop = true;

        // Start the bounce coroutine
        bounceCoroutine = StartCoroutine(Bounce());
    }

    void Update()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera is not assigned. Please assign it in the Inspector.");
            return;
        }

        // Check if the player is within the detection radius
        isNearPlayer = Vector3.Distance(transform.position, mainCamera.transform.position) <= detectionRadius;

        if (isNearPlayer)
        {
            StopBouncing();
        }
        else
        {
            StartBouncing();
        }
    }

    private IEnumerator Bounce()
    {
        while (true)
        {
            // Calculate the bounce positions
            Vector3 upPosition = new Vector3(originalPosition.x, originalPosition.y + bounceHeight, originalPosition.z);
            Vector3 downPosition = originalPosition;

            // Move to the up position
            yield return MoveToPosition(transform.position, upPosition, 1f / bounceSpeed);

            // Move to the down position
            yield return MoveToPosition(upPosition, downPosition, 1f / bounceSpeed);
        }
    }

    private IEnumerator MoveToPosition(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private void StartBouncing()
    {
        // Resume bouncing and play the sound
        if (bounceCoroutine == null)
        {
            bounceCoroutine = StartCoroutine(Bounce());
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopBouncing()
    {
        // Stop bouncing and pause the sound
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
