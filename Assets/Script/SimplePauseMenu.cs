using UnityEngine;
using UnityEngine.UI;  // Required for accessing UI components like RawImage and Text

public class SimplePauseMenu : MonoBehaviour
{
    public RawImage pauseMenuImage;  // Reference to the Raw Image for the pause menu background
    public Button pauseButton;  // Reference to the Button on the pause menu
    public Text pauseMenuText;  // Reference to the Text component on the pause menu
    public GameObject[] objectsToHide;  // Array of game objects whose meshes will be hidden
    private bool isPaused = false;  // To track if the game is currently paused

    void Start()
    {
        // Ensure the Raw Image, Button, and Text are disabled at the start
        pauseMenuImage.enabled = false;  // Hide the Raw Image
        pauseButton.gameObject.SetActive(false);  // Disable the pause button
        pauseMenuText.gameObject.SetActive(false);  // Disable the pause menu text
    }

    void Update()
    {
        // Listen for the Backspace key to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Backspace))  // KeyCode.Backspace is the Backspace key
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Function to pause the game
    public void PauseGame()
    {
        pauseMenuImage.enabled = true;  // Show the Raw Image for the pause menu
        pauseButton.gameObject.SetActive(true);  // Enable the pause button
        pauseMenuText.gameObject.SetActive(true);  // Enable the pause menu text
        ToggleMeshes(false);  // Hide the meshes of the specified game objects
        Time.timeScale = 0;  // Stop the game time
        isPaused = true;  // Set the paused state to true
    }

    // Function to resume the game
    public void ResumeGame()
    {
        pauseMenuImage.enabled = false;  // Hide the Raw Image
        pauseButton.gameObject.SetActive(false);  // Disable the pause button
        pauseMenuText.gameObject.SetActive(false);  // Disable the pause menu text
        ToggleMeshes(true);  // Show the meshes again
        Time.timeScale = 1;  // Resume the game time
        isPaused = false;  // Set the paused state to false
    }

    // Function to toggle mesh visibility
    private void ToggleMeshes(bool state)
    {
        foreach (GameObject obj in objectsToHide)
        {
            // Toggle mesh renderer visibility
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = state;
            }
        }
    }
}
