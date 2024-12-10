using System.Collections;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    public float bounceHeight = 0.5f;  // Height of each bounce
    public float bounceSpeed = 2f;     // Speed of bounce
    public AudioClip bounceSound;      // Sound to play during bouncing

    private Vector3 originalPosition;
    private AudioSource audioSource;
    private Coroutine bounceCoroutine;
    public bool isDead = false;

    void Start()
    {
        // Store the slime's original position
        originalPosition = transform.position;

        // Set up the audio source for the bounce sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bounceSound;
        audioSource.loop = true;

        // Start the bounce coroutine
        StartBouncing();
    }

    private IEnumerator Bounce()
    {
        while (true)
        {
            Vector3 upPosition = new Vector3(originalPosition.x, originalPosition.y + bounceHeight, originalPosition.z);
            Vector3 downPosition = originalPosition;

            yield return MoveToPosition(transform.position, upPosition, 1f / bounceSpeed);
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

    public void OnPlayerHitWeakness()
    {
        if (isDead) return;

        isDead = true;
        StopBouncing();
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        float duration = 1.5f; // Duration of fade-out
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            Color color = material.color;
            color.a = alpha;
            material.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
