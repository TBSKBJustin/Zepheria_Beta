using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeTrigger : MonoBehaviour
{
    public string triggerSide; // "Left" "Mid" "Right"
    private bool playerInside = false;

    public Renderer triggerRenderer;       // 触发器对象的Renderer
    public Material enterMaterial;         // 玩家进入时的材质
    private Material originalMaterial;     // 保存触发器的原始材质

    void Start()
    {

        if (triggerRenderer != null)
        {
            originalMaterial = triggerRenderer.material;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log($"Player entered {triggerSide} Trigger.");

            if (triggerRenderer != null && enterMaterial != null)
            {
                triggerRenderer.material = enterMaterial;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log($"Player exited {triggerSide} Trigger.");

            if (triggerRenderer != null && originalMaterial != null)
            {
                triggerRenderer.material = originalMaterial;
            }
        }
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }
}

