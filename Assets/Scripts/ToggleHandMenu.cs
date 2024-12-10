using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleHandMenu : MonoBehaviour
{
    public InputActionReference menuButtonAction; // Link your custom action here
    public GameObject handMenuCanvas;

    private void Start()
    {
        // Ensure the hand menu is not visible when the scene starts
        handMenuCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        menuButtonAction.action.performed += ToggleMenuVisibility;
    }

    private void OnDisable()
    {
        menuButtonAction.action.performed -= ToggleMenuVisibility;
    }

    private void ToggleMenuVisibility(InputAction.CallbackContext context)
    {
        handMenuCanvas.SetActive(!handMenuCanvas.activeSelf);
    }
}
