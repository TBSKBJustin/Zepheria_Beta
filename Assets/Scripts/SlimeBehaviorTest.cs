using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehaviorTest : MonoBehaviour
{
    public GameObject player;          // Field to assign the player GameObject in the Inspector
    public float detectionRadius = 5f; // Distance to detect the player
    public float bounceHeight = 0.5f;  // Height of each bounce
    public float bounceSpeed = 2f;     // Speed of bounce
    public AudioClip bounceSound;      // Sound for bouncing
    public AudioClip grabSound;        // Sound for grabbing
    public float fadeDuration = 1f;    // Time it takes for the slime to fade away

    private bool isNearPlayer = false;
    private bool isGrabbed = false;
    private bool hasBounced = false;   // Track whether it has bounced
    private Vector3 originalPosition;
    private Renderer slimeRenderer;
    private AudioSource audioSource;

    void Start()
    {
        originalPosition = transform.position;

        slimeRenderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
        if (slimeRenderer == null)
        {
            Debug.LogWarning("No Renderer found on the slime. Ensure it has a Renderer component.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (slimeRenderer == null || player == null) return;

        Vector3 playerPosition = player.transform.position;

        // Log for debugging player and slime position
        Debug.Log("Player Position: " + playerPosition);
        Debug.Log("Slime Position: " + transform.position);

        // Detect player within radius
        float distance = Vector3.Distance(transform.position, playerPosition);
        Debug.Log("Distance to player: " + distance);

        isNearPlayer = distance <= detectionRadius;

        // If the player is within range, start bouncing
        if (isNearPlayer && !hasBounced && !isGrabbed)
        {
            Debug.Log("Player detected within range. Starting bounce.");
            StartCoroutine(Bounce());
        }
    }

    private IEnumerator Bounce()
    {
        hasBounced = true;

        // Play bounce sound
        if (bounceSound != null)
        {
            audioSource.PlayOneShot(bounceSound);
        }

        // First bounce
        Vector3 firstBounce = new Vector3(originalPosition.x, originalPosition.y + bounceHeight, originalPosition.z);
        yield return MoveToPosition(originalPosition, firstBounce, 0.25f);
        yield return MoveToPosition(firstBounce, originalPosition, 0.25f);

        // Second, smaller bounce
        Vector3 secondBounce = new Vector3(originalPosition.x, originalPosition.y + (bounceHeight * 0.75f), originalPosition.z);
        yield return MoveToPosition(originalPosition, secondBounce, 0.2f);
        yield return MoveToPosition(secondBounce, originalPosition, 0.2f);

        hasBounced = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (isGrabbed) return;

        // No need for specific hand detection, only check if grabbed by any object
        if (other.CompareTag("PlayerHand"))
        {
            OnGrab();
        }
    }

    public void OnGrab()
    {
        if (isGrabbed) return;

        isGrabbed = true;

        // Play grab sound
        if (grabSound != null)
        {
            audioSource.PlayOneShot(grabSound);
        }

        // Fade and destroy
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        if (slimeRenderer != null)
        {
            Material material = slimeRenderer.material;
            Color originalColor = material.color;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}