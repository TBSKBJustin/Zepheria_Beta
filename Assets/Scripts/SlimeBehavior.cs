using System.Collections;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    public float bounceHeight = 0.5f; // How high the slime bounces
    public float activationRange = 5.0f; // Distance within which the slime stops bouncing
    public float bounceSpeed = 1.0f; // Controls how fast the bounce happens (lower = slower)
    public Transform player; // Drag the XR Rig or Main Camera here

    public AudioClip bounceSound; // Sound effect for bouncing
    private AudioSource audioSource;

    private bool isBouncing = true; // Ensure the slime bounces by default
    private Vector3 groundPosition;

    void Start()
    {
        // Store the initial ground position for bounce calculations
        groundPosition = transform.position;

        // Add an AudioSource component if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the bounce sound
        audioSource.clip = bounceSound;
        audioSource.playOnAwake = false;

        // Start the bouncing coroutine
        StartCoroutine(BounceContinuously());
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned to the SlimeBehavior script.");
            return;
        }

        // Track the actual player's position
        Vector3 playerPosition = player.position;

        // Calculate the distance between the player and the slime
        float distanceToPlayer = Vector3.Distance(playerPosition, groundPosition);

        // Stop bouncing if within range, otherwise continue bouncing
        isBouncing = distanceToPlayer > activationRange;
    }

    private IEnumerator BounceContinuously()
    {
        while (true)
        {
            if (isBouncing)
            {
                // Play bounce sound
                if (bounceSound != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }

                // Bounce up
                Vector3 targetPosition = new Vector3(groundPosition.x, groundPosition.y + bounceHeight, groundPosition.z);
                float duration = 1.0f / bounceSpeed; // Duration based on bounce speed
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    transform.position = Vector3.Lerp(groundPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Return to the ground
                elapsedTime = 0f;
                while (elapsedTime < duration)
                {
                    transform.position = Vector3.Lerp(targetPosition, groundPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.position = groundPosition;
            }
            else
            {
                // Ensure the slime is at the ground position when not bouncing
                transform.position = groundPosition;

                // Stop bounce sound
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            yield return null; // Wait until the next frame
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the activation range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
