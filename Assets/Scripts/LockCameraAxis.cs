using UnityEngine;

public class LockCameraXAxis : MonoBehaviour
{
    public Camera miniMapCamera; // Reference to the MiniMap Camera
    public Transform target;     // The Main Camera (or player) Transform
    public float offsetY = 10f;  // Height for the minimap camera
    public float fixedZ = 0f;    // Fixed depth for the minimap view

    void Update()
    {
        if (miniMapCamera != null && target != null)
        {
            // Lock the minimap camera position to only follow the X-axis of the target
            miniMapCamera.transform.position = new Vector3(target.position.x, offsetY, fixedZ);

            // Keep the camera looking straight down
            miniMapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("MiniMap Camera or Target is not assigned!");
        }
    }
}
