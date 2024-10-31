using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
  public float detectionRadius = 5f;  // Distance within which the slime starts bouncing
    public float bounceHeight = 0.5f;   // Height of each bounce
    public float bounceSpeed = 2f;      // Speed of bounce
    public Color grabbedColor = Color.red; // Color to change when grabbed

    private bool isNearPlayer = false;
    private bool isGrabbed = false;
    private bool hasBounced = false; // Track whether it has bounced
    private Vector3 originalPosition;
    private Renderer slimeRenderer;
    private Color originalColor;

    void Start()
    {
        originalPosition = transform.position;

        // Attempt to get Renderer component from self or children
        slimeRenderer = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();

        if (slimeRenderer != null)
        {
            originalColor = slimeRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("No Renderer found on the slime or its children. Ensure it has a Renderer component.");
        }
    }

    void Update()
    {
        if (slimeRenderer == null) return; // Stop if no Renderer found

        // Check if player is within detection radius and it hasn’t bounced yet
        isNearPlayer = Vector3.Distance(transform.position, PlayerPosition()) <= detectionRadius;

        if (isNearPlayer && !hasBounced && !isGrabbed)
        {
            StartCoroutine(Bounce());
        }

        // Check if the player clicks on the slime to grab it
        if (Input.GetMouseButtonDown(0) && !isGrabbed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                OnGrab();
            }
        }
        // Release the slime when the player clicks again
        else if (Input.GetMouseButtonDown(0) && isGrabbed)
        {
            OnRelease();
        }
    }

    private IEnumerator Bounce()
{
    hasBounced = true; // Set the bounce state to true, so it won't bounce repeatedly while player is nearby

    // Get the terrain height at the slime's current position
    float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);

    // Calculate bounce positions for the two bounces
    Vector3 firstBounce = new Vector3(originalPosition.x, terrainHeight + bounceHeight, originalPosition.z);
    Vector3 secondBounce = new Vector3(originalPosition.x, terrainHeight + (bounceHeight * 0.75f), originalPosition.z);

    // First bounce
    yield return MoveToPosition(originalPosition, firstBounce, 0.25f);
    yield return MoveToPosition(firstBounce, originalPosition, 0.25f);

    // Second, smaller bounce
    yield return MoveToPosition(originalPosition, secondBounce, 0.2f);
    yield return MoveToPosition(secondBounce, originalPosition, 0.2f);

    // Reset bounce state so it can bounce again if the player re-enters the radius
    hasBounced = false;
}

    // Helper coroutine to move the slime between two positions over a duration
    private IEnumerator MoveToPosition(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition; // Ensure it reaches the exact end position
    }

    // Simulate player position (replace with actual player position in your game)
    Vector3 PlayerPosition()
    {
        // Use your VR player's actual position here
        return Camera.main.transform.position;
    }

    // This method should be called by the grabbing system in your VR setup
    public void OnGrab()
    {
        if (slimeRenderer != null)
        {
            isGrabbed = true;
            hasBounced = true; // Stop bouncing once grabbed
            slimeRenderer.material.color = grabbedColor;
            Debug.Log("Slime grabbed!");
        }
    }

    // Call this method when the player releases the slime
    public void OnRelease()
    {
        if (slimeRenderer != null)
        {
            isGrabbed = false;
            hasBounced = false; // Allow bouncing again when released
            slimeRenderer.material.color = originalColor;
            Debug.Log("Slime released!");
        }
    }
}
