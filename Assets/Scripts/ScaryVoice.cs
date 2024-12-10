using System.Collections;
using UnityEngine;
using TMPro;

public class RoomTrigger : MonoBehaviour
{
    public Transform player;  // Reference to the player object
    public Transform table;   // Reference to the table in the room
    public float activationRange = 3f;  // Distance to trigger the voice and text
    public AudioClip scaryVoiceClip;  // Scary voice audio clip
    public TextMeshProUGUI text1;  // First text to display
    public TextMeshProUGUI text2;  // Second text to display
    public TextMeshProUGUI text3;  // Third text to display
    public TextMeshProUGUI text4;  // Fourth text to display
    public float fadeInDuration = 1f;  // Time for text to fade in
    public float displayDuration = 3f;  // Time for text to stay visible
    public float fadeOutDuration = 1f;  // Time for text to fade out

    public GameObject door; // Reference to the door object

    private bool hasTriggered = false;  // Ensure the event only triggers once

    void Start()
    {
        // Make all text fields invisible at the start
        SetTextAlpha(text1, 0);
        SetTextAlpha(text2, 0);
        SetTextAlpha(text3, 0);
        SetTextAlpha(text4, 0);

        // Ensure the door is initially active unless intentionally hidden
        if (door == null)
        {
            Debug.LogError("Door is not assigned in the inspector.");
        }
    }

    void Update()
    {
        // Track the player's position
        Vector3 playerPosition = player.position;

        // Track the table's position
        Vector3 tablePosition = table.position;

        // Calculate the distance between the player and the table
        float distanceToPlayer = Vector3.Distance(playerPosition, tablePosition);

        // Trigger the voice and text if the player is within range and the event hasn't triggered yet
        if (distanceToPlayer <= activationRange && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("Triggering scary voice and text sequence.");
            TriggerScaryVoiceAndText();
        }
    }

    public void TriggerScaryVoiceAndText()
    {
        // Play the scary voice at the table's position
        if (scaryVoiceClip != null)
        {
            AudioSource.PlayClipAtPoint(scaryVoiceClip, table.position);
        }
        else
        {
            Debug.LogError("Scary voice clip is not assigned.");
        }

        // Start displaying text in sequence
        StartCoroutine(ShowTextsSequentially());
    }

    IEnumerator ShowTextsSequentially()
    {
        // Display each text field in order
        yield return ShowText(text1);
        yield return ShowText(text2);
        yield return ShowText(text3);
        yield return ShowText(text4);

        // Activate the door after the last text
        if (door != null)
        {
            Debug.Log("Activating the door.");
            door.SetActive(true);
        }
        else
        {
            Debug.LogError("Door is not assigned or is missing.");
        }
    }

    IEnumerator ShowText(TextMeshProUGUI text)
    {
        if (text == null) yield break;

        // Fade in the text
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            SetTextAlpha(text, Mathf.Lerp(0, 1, elapsedTime / fadeInDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully visible
        SetTextAlpha(text, 1);

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out the text
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            SetTextAlpha(text, Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully invisible
        SetTextAlpha(text, 0);
    }

    void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text == null) return;

        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
