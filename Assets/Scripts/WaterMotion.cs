using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMotion : MonoBehaviour
{
       public float waveSpeed = 1.0f; // Speed of the wave motion
    public float waveHeight = 0.1f; // Height of the waves
    public float waveFrequency = 1.0f; // Frequency of the wave motion

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the water
        initialPosition = transform.position;
    }

    void Update()
    {
        // Create a wave effect
        float newY = initialPosition.y + Mathf.Sin(Time.time * waveFrequency) * waveHeight;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
