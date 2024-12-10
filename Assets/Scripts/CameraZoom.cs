using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraZoomWithScrollbar : MonoBehaviour
{
    public Camera targetCamera; // The camera to zoom
    public Scrollbar zoomScrollbar; // The scrollbar used for zooming
    public float minZoom = 20f; // Minimum field of view for the camera
    public float maxZoom = 60f; // Maximum field of view for the camera

    void Start()
    {
        if (zoomScrollbar != null)
        {
            zoomScrollbar.onValueChanged.AddListener(OnZoomValueChanged);
            zoomScrollbar.value = 0.5f; // Default zoom level
        }
    }

    void OnZoomValueChanged(float value)
    {
        if (targetCamera != null)
        {
            // Linearly interpolate between minZoom and maxZoom based on scrollbar value
            targetCamera.fieldOfView = Mathf.Lerp(minZoom, maxZoom, value);
        }
    }
}
