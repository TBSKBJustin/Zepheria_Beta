using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    public float speedX = 0.01f;  // Speed for horizontal movement
    public float speedY = 0.01f;  // Speed for vertical movement
    public Material skyboxMaterial;  // Assign the skybox material here

    void Update()
    {
        float offsetX = Time.time * speedX;
        float offsetY = Time.time * speedY;

        skyboxMaterial.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }
}
