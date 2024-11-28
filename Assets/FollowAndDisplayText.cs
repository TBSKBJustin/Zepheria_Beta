using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FollowAndDisplayText : MonoBehaviour
{
    public Transform mainCamera;             // Assign the VR camera in Inspector
    public Transform objectToFollow;         // Assign the Mushroom Man in Inspector
    public TMP_Text messageText;             // Assign the message text in Inspector
    public AudioSource audioSource;          // Assign an AudioSource in Inspector
    public AudioClip[] mushroomSounds;       // Assign mushroom sounds in Inspector
    public float followDistance = 3.0f;      // Minimum distance for the Mushroom Man to follow
    public float stopDistance = 1.0f;        // Distance at which the Mushroom Man stops moving
    public float teleportDistance = 10.0f;   // Distance at which the Mushroom Man teleports
    public float moveSpeed = 2.0f;           // Speed of following the player
    public float rotationSpeed = 5.0f;       // Speed of rotation to face the player
    public float messageHeightOffset = 0.8f; // Height above the Mushroom Man for the message
    public float raiseAboveFloor = 0.5f;     // Distance to raise the Mushroom Man above the floor
    public float promptInterval = 3.0f;      // Time between prompts
    public TextAsset promptsFile;            // Assign Prompts.txt in Inspector

    private Vector3 lastPlayerPosition;
    private bool hasPlayedAudio = false;
    private List<string> prompts = new List<string>();
    private int currentPromptIndex = 0;
    private float promptTimer = 0f;
    private Renderer[] mushroomRenderers;
    private bool isLookingAtObject = false;  // Tracks if the player is looking at the Mushroom Man

    void Start()
    {
        // Initialize the Mushroom Man's starting position
        Vector3 startPosition = objectToFollow.position;
        startPosition.y += raiseAboveFloor;
        objectToFollow.position = startPosition;

        // Load prompts from the provided TextAsset file
        if (promptsFile != null)
        {
            prompts = new List<string>(promptsFile.text.Split('\n'));
            for (int i = prompts.Count - 1; i >= 0; i--)
            {
                prompts[i] = prompts[i].Trim();
                if (string.IsNullOrEmpty(prompts[i]))
                {
                    prompts.RemoveAt(i);
                }
            }
        }
        else
        {
            Debug.LogError("Prompts file not assigned. Please assign a TextAsset in the Inspector.");
        }

        // Get all renderers attached to the Mushroom Man for visibility handling
        mushroomRenderers = objectToFollow.GetComponentsInChildren<Renderer>();
        if (mushroomRenderers == null || mushroomRenderers.Length == 0)
        {
            Debug.LogError("No Renderer found on the Mushroom Man or its children.");
        }

        // Hide the text initially
        if (messageText != null)
        {
            messageText.enabled = false;
        }
        else
        {
            Debug.LogError("Message Text is not assigned in the Inspector.");
        }

        // Ensure mainCamera is assigned
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned. Assign the VR camera in the Inspector.");
        }
    }

    void Update()
    {
        if (mainCamera == null || objectToFollow == null)
            return;

        float distanceToPlayer = Vector3.Distance(mainCamera.position, objectToFollow.position);

        // Determine behavior based on the distance to the player
        if (distanceToPlayer > followDistance && distanceToPlayer <= teleportDistance)
        {
            FollowPlayer();
        }
        else if (distanceToPlayer > teleportDistance)
        {
            TeleportBehindPlayer();
        }

        // Update prompts and audio when applicable
        HandlePromptCycling();
        HandlePlayerGaze();
    }

    void FollowPlayer()
    {
        Vector3 targetPosition = mainCamera.position - mainCamera.forward * followDistance;
        targetPosition.y = objectToFollow.position.y; // Keep the Y position constant

        // Smoothly move the Mushroom Man toward the target position
        objectToFollow.position = Vector3.MoveTowards(objectToFollow.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate the Mushroom Man to face the player
        RotateTowardsPlayer();
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = mainCamera.position - objectToFollow.position;
        directionToPlayer.y = 0; // Lock rotation to the horizontal plane
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            objectToFollow.rotation = Quaternion.Slerp(objectToFollow.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void TeleportBehindPlayer()
    {
        Vector3 teleportPosition = mainCamera.position - mainCamera.forward * followDistance;
        teleportPosition.y = objectToFollow.position.y; // Maintain current height
        objectToFollow.position = teleportPosition;
    }

    void HandlePromptCycling()
    {
        if (prompts.Count == 0 || !isLookingAtObject) // Only cycle prompts if the player is looking at the object
            return;

        promptTimer += Time.deltaTime;
        if (promptTimer >= promptInterval)
        {
            promptTimer = 0f;
            currentPromptIndex = (currentPromptIndex + 1) % prompts.Count;
            messageText.text = prompts[currentPromptIndex];
        }

        // Display the message above the Mushroom Man
        Vector3 messagePosition = objectToFollow.position;
        messagePosition.y += messageHeightOffset;
        messageText.transform.position = Vector3.Lerp(messageText.transform.position, messagePosition, Time.deltaTime * 10.0f);
        messageText.transform.rotation = Quaternion.LookRotation(messageText.transform.position - mainCamera.position);

        messageText.enabled = true;
    }

    void HandlePlayerGaze()
    {
        Vector3 cameraForward = mainCamera.forward;
        Vector3 toObject = (objectToFollow.position - mainCamera.position).normalized;
        float dotProduct = Vector3.Dot(cameraForward, toObject);

        if (dotProduct > 0.8f) // Player is looking at the object
        {
            if (!isLookingAtObject)
            {
                isLookingAtObject = true;
                messageText.enabled = true; // Show the message
            }

            if (!hasPlayedAudio)
            {
                PlayRandomSound();
                hasPlayedAudio = true;
            }
        }
        else // Player is not looking at the object
        {
            if (isLookingAtObject)
            {
                isLookingAtObject = false;
                messageText.enabled = false; // Hide the message
            }
            hasPlayedAudio = false;
        }
    }

    void PlayRandomSound()
    {
        if (mushroomSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, mushroomSounds.Length);
            audioSource.PlayOneShot(mushroomSounds[randomIndex]);
        }
    }
}
