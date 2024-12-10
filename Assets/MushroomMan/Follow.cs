using UnityEngine;
using TMPro;
using System.Collections.Generic; // Add this line for List<> and other generic collections

public class GazeMessage : MonoBehaviour
{
    public Transform mainCamera;
    public TMP_Text messageText;
    public TextAsset promptsFile;
    public AudioSource audioSource;
    public AudioClip[] mushroomSounds;
    public float messageHeightOffset = 0.8f;

    public List<float> messageDurations = new List<float>(); // List of durations for each message

    private List<string> prompts = new List<string>();
    private int currentPromptIndex = 0;
    private float promptTimer = 0f;
    private bool isLookingAtObject = false;
    private bool hasPlayedAudio = false;

    void Start()
    {
        if (promptsFile != null)
        {
            // Load prompts from the file
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

        if (messageText != null)
        {
            messageText.enabled = false;
        }
        else
        {
            Debug.LogError("Message Text is not assigned in the Inspector.");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned. Assign the VR camera in the Inspector.");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned. Please assign an AudioSource in the Inspector.");
        }
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        HandlePromptCycling();
        HandlePlayerGaze();
        RotateTowardsPlayer();
    }

    void HandlePromptCycling()
    {
        if (prompts.Count == 0 || !isLookingAtObject)
            return;

        promptTimer += Time.deltaTime;
        float currentDuration = (messageDurations.Count > currentPromptIndex) ? messageDurations[currentPromptIndex] : 3.0f;

        if (promptTimer >= currentDuration)
        {
            promptTimer = 0f;
            currentPromptIndex = (currentPromptIndex + 1) % prompts.Count;
            messageText.text = prompts[currentPromptIndex];
        }

        Vector3 messagePosition = transform.position;
        messagePosition.y += messageHeightOffset;
        messageText.transform.position = Vector3.Lerp(messageText.transform.position, messagePosition, Time.deltaTime * 10.0f);
        messageText.transform.rotation = Quaternion.LookRotation(messageText.transform.position - mainCamera.position);

        messageText.enabled = true;
    }

    void HandlePlayerGaze()
    {
        Vector3 cameraForward = mainCamera.forward;
        Vector3 toObject = (transform.position - mainCamera.position).normalized;
        float dotProduct = Vector3.Dot(cameraForward, toObject);

        if (dotProduct > 0.8f)
        {
            if (!isLookingAtObject)
            {
                isLookingAtObject = true;
                messageText.enabled = true;
            }

            if (!hasPlayedAudio)
            {
                PlayRandomSound();
                hasPlayedAudio = true;
            }
        }
        else
        {
            if (isLookingAtObject)
            {
                isLookingAtObject = false;
                messageText.enabled = false;
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

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = mainCamera.position - transform.position;
        directionToPlayer.y = 0;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
        }
    }
}
