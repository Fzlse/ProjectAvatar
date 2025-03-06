using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Method to load a scene by name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Method to quit the application
    public void QuitGame()
    {
        // Will only work in a built application, not in the Unity Editor
        Application.Quit();
    }
}
