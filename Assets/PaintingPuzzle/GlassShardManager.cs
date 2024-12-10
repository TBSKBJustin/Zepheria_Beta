using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GlassPaneManager : MonoBehaviour
{
    [System.Serializable]
    public class ShardSlot
    {
        public GameObject shard; // The glass shard object
        public Transform slot; // The slot position
        public Material defaultMaterial; // The original glass material
        public bool isPlaced = false; // Whether the shard is placed
        public bool isBeingGrabbed = false; // Whether the shard is currently being grabbed
    }

    public ShardSlot[] shards; // Array for all shards and slots
    public Material flashMaterial; // Flashing material (Flashmat)
    public Material photoMaterial; // Material to apply to the "Image" object
    public GameObject imageObject; // The object labeled "Image"
    public GameObject paintingObject; // The painting to flash when a shard is grabbed
    public GameObject flashObject1; // The first object to flash when a shard is grabbed
    public GameObject flashObject2; // The second object to flash when a shard is grabbed
    public float snapDistance = 0.5f; // Distance to snap shards into slots
    public float flashInterval = 0.5f; // Time between flashes

    private bool isPaintingFlashing = false; // Whether the painting is currently flashing
    private bool hasReplacedImageMaterial = false; // To ensure image material is replaced only once

    private Material originalPaintingMaterial; // To store the original painting material
    private Material originalFlashMaterial1; // To store the original material of flashObject1
    private Material originalFlashMaterial2; // To store the original material of flashObject2

    // Cached MeshRenderer references
    private MeshRenderer imageRenderer;
    private MeshRenderer paintingRenderer;
    private MeshRenderer flashRenderer1;
    private MeshRenderer flashRenderer2;

    // Coroutine references
    private Coroutine paintingFlashCoroutine;
    private Coroutine flashObject1Coroutine;
    private Coroutine flashObject2Coroutine;

    public AudioClip respawnSound; // The sound to play when respawning the object
    public AudioClip snapSound; // Separate audio clip for snapping a shard
    public GameObject respawnObject; // The object to despawn and respawn
    private AudioSource audioSource; // The AudioSource for playing the sound

    private void Start()
    {
        // Cache MeshRenderer references
        if (imageObject != null)
        {
            imageRenderer = imageObject.GetComponent<MeshRenderer>();
            if (imageRenderer == null)
            {
                Debug.LogError("Image Object does not have a MeshRenderer component.");
            }
        }
        else
        {
            Debug.LogError("Image Object is not assigned.");
        }

        if (paintingObject != null)
        {
            paintingRenderer = paintingObject.GetComponent<MeshRenderer>();
            if (paintingRenderer != null)
            {
                originalPaintingMaterial = paintingRenderer.material;
            }
            else
            {
                Debug.LogError("Painting Object does not have a MeshRenderer component.");
            }
        }
        else
        {
            Debug.LogError("Painting Object is not assigned.");
        }

        if (flashObject1 != null)
        {
            flashRenderer1 = flashObject1.GetComponent<MeshRenderer>();
            if (flashRenderer1 != null)
            {
                originalFlashMaterial1 = flashRenderer1.material;
            }
            else
            {
                Debug.LogError("Flash Object 1 does not have a MeshRenderer component.");
            }
        }
        else
        {
            Debug.LogError("Flash Object 1 is not assigned.");
        }

        if (flashObject2 != null)
        {
            flashRenderer2 = flashObject2.GetComponent<MeshRenderer>();
            if (flashRenderer2 != null)
            {
                originalFlashMaterial2 = flashRenderer2.material;
            }
            else
            {
                Debug.LogError("Flash Object 2 does not have a MeshRenderer component.");
            }
        }
        else
        {
            Debug.LogError("Flash Object 2 is not assigned.");
        }

        // Ensure no shards are marked as placed initially
        foreach (var shardSlot in shards)
        {
            shardSlot.isPlaced = false;
            shardSlot.isBeingGrabbed = false;

            // Subscribe to grab and release events for each shard
            var grabInteractable = shardSlot.shard.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                // Capture the current shardSlot in the lambda to avoid closure issues
                var currentShardSlot = shardSlot;
                grabInteractable.selectEntered.AddListener((args) => OnShardGrabbed(currentShardSlot));
                grabInteractable.selectExited.AddListener((args) => OnShardReleased(currentShardSlot));
            }
            else
            {
                Debug.LogWarning($"Shard {shardSlot.shard.name} does not have an XRGrabInteractable component.");
            }

            StartCoroutine(FlashShard(shardSlot)); // Start flashing each shard
        }

        // Ensure the respawn object is disabled at the start
        if (respawnObject != null)
        {
            respawnObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Respawn Object is not assigned.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource component added.");
        }
    }

    private void Update()
    {
        foreach (var shardSlot in shards)
        {
            if (!shardSlot.isPlaced && shardSlot.shard != null)
            {
                // Check the distance between the shard and its slot
                float distance = Vector3.Distance(shardSlot.shard.transform.position, shardSlot.slot.position);

                // Only snap when the shard is within the snapDistance
                if (distance <= snapDistance)
                {
                    SnapShardToSlot(shardSlot);
                }
            }
        }

        // Check if all shards are placed before completing the puzzle
        if (!hasReplacedImageMaterial && AreAllShardsPlaced())
        {
            ReplaceImageMaterial();
            hasReplacedImageMaterial = true;
        }
    }

    private void RespawnObject()
    {
        if (respawnObject != null)
        {
            respawnObject.SetActive(true); // Respawn the object
            Debug.Log("Respawn object has been activated.");
        }
        else
        {
            Debug.LogError("Respawn Object is not assigned.");
        }

        if (respawnSound != null && audioSource != null)
        {
            audioSource.clip = respawnSound; // Assign the audio clip
            audioSource.Play(); // Play the sound
            Debug.Log("Respawn sound played.");
        }
        else
        {
            if (respawnSound == null)
                Debug.LogError("Respawn Sound is not assigned.");
            if (audioSource == null)
                Debug.LogError("AudioSource component is missing.");
        }
    }

    private void SnapShardToSlot(ShardSlot shardSlot)
    {
        // Move the shard to the slot's position and rotation
        shardSlot.shard.transform.position = shardSlot.slot.position;
        shardSlot.shard.transform.rotation = shardSlot.slot.rotation;

        // Make the shard a child of the slot so it doesn't leave that position
        shardSlot.shard.transform.SetParent(shardSlot.slot);

        // Disable physics and lock the shard in place
        Rigidbody rb = shardSlot.shard.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll; // Prevents any movement or rotation
        }

        // Disable grabbing
        var grabInteractable = shardSlot.shard.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        // Update shard state
        shardSlot.isPlaced = true;
        shardSlot.isBeingGrabbed = false;

        // Restore the shard's default material
        MeshRenderer renderer = shardSlot.shard.GetComponent<MeshRenderer>();
        if (renderer != null && shardSlot.defaultMaterial != null)
        {
            renderer.material = shardSlot.defaultMaterial;
        }

        Debug.Log($"Shard {shardSlot.shard.name} snapped into position and is now permanent.");

        // Play audio clip when a shard is snapped
        if (audioSource != null && respawnSound != null)
        {
            audioSource.PlayOneShot(snapSound);
            Debug.Log("Shard snapped audio played.");
        }
    }


    private bool AreAllShardsPlaced()
    {
        foreach (var shardSlot in shards)
        {
            if (!shardSlot.isPlaced)
            {
                return false; // Return false if any shard is not placed
            }
        }
        return true; // All shards are placed
    }

    private void ReplaceImageMaterial()
    {
        // Ensure the image renderer and photo material are assigned
        if (imageRenderer == null)
        {
            Debug.LogError("Image Renderer is not assigned.");
            return;
        }

        if (photoMaterial == null)
        {
            Debug.LogError("Photo Material is not assigned.");
            return;
        }

        // Replace the material
        imageRenderer.material = photoMaterial;
        Debug.Log("All shards placed. Image material replaced.");

        // Respawn the object and play the sound
        RespawnObject();
    }

    private IEnumerator FlashShard(ShardSlot shardSlot)
    {
        MeshRenderer renderer = shardSlot.shard.GetComponent<MeshRenderer>();
        if (renderer == null || flashMaterial == null || shardSlot.defaultMaterial == null)
        {
            Debug.LogError($"Cannot flash shard {shardSlot.shard.name}: Missing components.");
            yield break;
        }

        while (!shardSlot.isPlaced)
        {
            // Flash only if the shard is not being grabbed
            if (!shardSlot.isBeingGrabbed)
            {
                renderer.material = flashMaterial; // Apply flashing material
                yield return new WaitForSeconds(flashInterval);
                renderer.material = shardSlot.defaultMaterial; // Revert to default material
                yield return new WaitForSeconds(flashInterval);
            }
            else
            {
                // Ensure it has the default material while being grabbed
                renderer.material = shardSlot.defaultMaterial;
                yield return null; // Wait until the next frame
            }
        }

        // Ensure the shard has the default material once placed
        if (renderer.material != shardSlot.defaultMaterial)
        {
            renderer.material = shardSlot.defaultMaterial;
        }
    }

    public void StartFlashingPainting()
    {
        if (!isPaintingFlashing)
        {
            isPaintingFlashing = true;
            paintingFlashCoroutine = StartCoroutine(FlashPainting());
            Debug.Log("Started flashing painting.");
        }
    }

    private IEnumerator FlashPainting()
    {
        if (paintingRenderer == null || flashMaterial == null || originalPaintingMaterial == null)
        {
            Debug.LogError("Cannot flash painting: Missing components.");
            yield break;
        }

        while (isPaintingFlashing)
        {
            paintingRenderer.material = flashMaterial; // Apply flashing material
            yield return new WaitForSeconds(flashInterval);
            paintingRenderer.material = originalPaintingMaterial; // Revert to original material
            yield return new WaitForSeconds(flashInterval);
        }

        // Ensure the painting has the original material once flashing stops
        paintingRenderer.material = originalPaintingMaterial;
        Debug.Log("Stopped flashing painting.");
    }

    public void StopFlashingPainting()
    {
        if (paintingFlashCoroutine != null)
        {
            StopCoroutine(paintingFlashCoroutine);
            paintingFlashCoroutine = null;
            Debug.Log("Stopped painting flash coroutine.");
        }

        isPaintingFlashing = false;

        if (paintingRenderer != null && originalPaintingMaterial != null)
        {
            paintingRenderer.material = originalPaintingMaterial; // Restore original material
            Debug.Log("Painting material restored.");
        }
    }

    // Call this method when a shard is grabbed
    public void OnShardGrabbed(ShardSlot shardSlot)
    {
        shardSlot.isBeingGrabbed = true; // Mark the shard as being grabbed
        Debug.Log($"Shard {shardSlot.shard.name} has been grabbed.");

        if (shardSlot.shard != null)
        {
            var renderer = shardSlot.shard.GetComponent<MeshRenderer>();
            if (renderer != null && shardSlot.defaultMaterial != null)
            {
                renderer.material = shardSlot.defaultMaterial; // Ensure it displays the default material
            }
        }

        StartFlashingObjects(); // Start flashing the other objects
        StartFlashingPainting(); // Start flashing the painting
    }

    // Call this method when a shard is released
    public void OnShardReleased(ShardSlot shardSlot)
    {
        shardSlot.isBeingGrabbed = false; // Mark the shard as no longer being grabbed
        Debug.Log($"Shard {shardSlot.shard.name} has been released.");

        if (!shardSlot.isPlaced)
        {
            StartCoroutine(FlashShard(shardSlot)); // Resume flashing
            Debug.Log($"Resumed flashing shard {shardSlot.shard.name}.");
        }
        else
        {
            if (shardSlot.shard != null)
            {
                var renderer = shardSlot.shard.GetComponent<MeshRenderer>();
                if (renderer != null && shardSlot.defaultMaterial != null)
                {
                    renderer.material = shardSlot.defaultMaterial; // Ensure it stays on the default material
                }
            }
        }

        StopFlashingObjects(); // Stop flashing the other objects
        StopFlashingPainting(); // Stop flashing the painting
    }

    private void StartFlashingObjects()
    {
        Debug.Log("Starting to flash objects.");

        if (flashObject1 != null && originalFlashMaterial1 != null && flashRenderer1 != null && flashObject1Coroutine == null)
        {
            flashObject1Coroutine = StartCoroutine(FlashObject(flashObject1, originalFlashMaterial1, flashRenderer1));
        }
        else
        {
            Debug.LogWarning("flashObject1 is not properly assigned or already flashing.");
        }

        if (flashObject2 != null && originalFlashMaterial2 != null && flashRenderer2 != null && flashObject2Coroutine == null)
        {
            flashObject2Coroutine = StartCoroutine(FlashObject(flashObject2, originalFlashMaterial2, flashRenderer2));
        }
        else
        {
            Debug.LogWarning("flashObject2 is not properly assigned or already flashing.");
        }
    }

    private void StopFlashingObjects()
    {
        if (flashObject1Coroutine != null)
        {
            StopCoroutine(flashObject1Coroutine);
            flashObject1Coroutine = null;
            ResetObjectMaterial(flashObject1, originalFlashMaterial1, flashRenderer1);
            Debug.Log("Stopped flashing flashObject1.");
        }

        if (flashObject2Coroutine != null)
        {
            StopCoroutine(flashObject2Coroutine);
            flashObject2Coroutine = null;
            ResetObjectMaterial(flashObject2, originalFlashMaterial2, flashRenderer2);
            Debug.Log("Stopped flashing flashObject2.");
        }
    }

    private void ResetObjectMaterial(GameObject obj, Material originalMaterial, MeshRenderer renderer)
    {
        if (obj != null && originalMaterial != null && renderer != null)
        {
            renderer.material = originalMaterial;
            Debug.Log($"Reset material for {obj.name}.");
        }
        else
        {
            Debug.LogWarning($"Cannot reset material for {obj?.name}. Missing components.");
        }
    }

    private IEnumerator FlashObject(GameObject obj, Material originalMaterial, MeshRenderer renderer)
    {
        if (renderer == null || flashMaterial == null)
        {
            Debug.LogError($"Cannot flash {obj.name}: Renderer or flashMaterial is null.");
            yield break;
        }

        Debug.Log($"Started flashing {obj.name}.");

        while (true)
        {
            renderer.material = flashMaterial; // Apply flashing material
            yield return new WaitForSeconds(flashInterval);
            renderer.material = originalMaterial; // Revert to original material
            yield return new WaitForSeconds(flashInterval);
        }
    }
}
