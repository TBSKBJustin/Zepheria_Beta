using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;

    void Update()
    {
        // Get input from the keyboard
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        // Move the player forward and backward
        transform.Translate(0, 0, move);

        // Rotate the player left and right
        transform.Rotate(0, rotate, 0);
    }
}
