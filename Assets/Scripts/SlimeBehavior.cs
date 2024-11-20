using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceHeight = 0.5f; // How high the slime bounces
    public float bounceSpeed = 2.0f; // How fast the slime bounces
    private float bounceTimer = 0.0f;

    [Header("Detection Settings")]
    public float activationRange = 5.0f; // Distance within which the slime activates
    public Transform player; // Drag the XR Rig or Main Camera here

    [Header("Sound Settings")]
    public AudioClip bounceSound;
    private AudioSource audioSource;

    private bool isPlayerInRange = false;
    private bool isBouncing = false;

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
        audioSource.loop = true; // Keep playing the sound while bouncing
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

        // Debugging output for distance
        Debug.Log($"Distance to Player: {distanceToPlayer}");

        // Check if the player is within range
        if (distanceToPlayer <= activationRange)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                StartBouncing();
            }

            // Perform the bounce animation if bouncing is active
            if (isBouncing)
            {
                bounceTimer += Time.deltaTime * bounceSpeed;
                float bounceOffset = Mathf.Sin(bounceTimer) * bounceHeight;
                transform.position = new Vector3(groundPosition.x, groundPosition.y + Mathf.Abs(bounceOffset), groundPosition.z);
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                StopBouncing();
            }
        }
    }

    private void StartBouncing()
    {
        isBouncing = true;
        bounceTimer = 0.0f; // Reset the bounce timer to sync the bounce
        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        Debug.Log("Player entered range. Slime started bouncing.");
    }

    private void StopBouncing()
    {
        isBouncing = false;
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Reset position to the original ground state
        transform.position = groundPosition;

        Debug.Log("Player left range. Slime stopped bouncing.");
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the activation range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
