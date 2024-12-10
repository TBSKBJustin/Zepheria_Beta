using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Game is quitting...");

#if UNITY_EDITOR
        // Exit play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application in a built version
        Application.Quit();
#endif
    }
}
