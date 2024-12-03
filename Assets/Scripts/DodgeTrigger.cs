using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeTrigger : MonoBehaviour
{
    public string triggerSide; // "Left" »ò "Right"
    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log($"Player entered {triggerSide} Trigger.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log($"Player exited {triggerSide} Trigger.");
        }
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }
}

