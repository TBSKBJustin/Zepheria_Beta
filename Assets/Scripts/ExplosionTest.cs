using System.Collections;
using UnityEngine;

public class ExplosionTest : MonoBehaviour
{
    public GameObject door;  // The door to shake
    public Light roomLight;  // Room light to dim

    private float shakeTime = 0.5f;  // Time for shaking the door
    private Vector3 originalDoorPosition;  // Original door position for shaking

    void Start()
    {
        // Store the original door position
        originalDoorPosition = door.transform.position;

        // Start both effects
        StartCoroutine(ShakeDoorAndDimLight());
    }

    IEnumerator ShakeDoorAndDimLight()
    {
        // Start both shaking and dimming in parallel
        StartCoroutine(ShakeDoor());
        yield return StartCoroutine(DarkenRoom());
    }

    IEnumerator ShakeDoor()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeTime)
        {
            float shakeAmount = Mathf.Sin(elapsedTime * Mathf.PI * 2f * 10f) * 0.02f;
            door.transform.position = new Vector3(originalDoorPosition.x + shakeAmount, originalDoorPosition.y, originalDoorPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the door's position after shaking
        door.transform.position = originalDoorPosition;
    }

    IEnumerator DarkenRoom()
    {
        float elapsedTime = 0f;
        float targetIntensity = 0.1f;  // Final intensity of the light
        float initialIntensity = roomLight.intensity;

        while (elapsedTime < 2f)
        {
            roomLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set final light intensity
        roomLight.intensity = targetIntensity;
    }
}
