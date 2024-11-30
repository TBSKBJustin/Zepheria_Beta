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

    private bool hasTriggered = false;  // Ensure the event only triggers once

    void Start()
    {
        // Make all text fields invisible at the start
        SetTextAlpha(text1, 0);
        SetTextAlpha(text2, 0);
        SetTextAlpha(text3, 0);
        SetTextAlpha(text4, 0);
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
            TriggerScaryVoiceAndText();
        }
    }

    void TriggerScaryVoiceAndText()
    {
        // Play the scary voice at the table's position
        AudioSource.PlayClipAtPoint(scaryVoiceClip, table.position);

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
    }

    IEnumerator ShowText(TextMeshProUGUI text)
    {
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
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
