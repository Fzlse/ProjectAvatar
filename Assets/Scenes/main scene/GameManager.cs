using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject retryPopup;   // Reference to the retry popup UI
    public UnityEngine.UI.Text scoreText;  // Explicitly using UnityEngine.UI.Text for score
    public UnityEngine.UI.Text timerText;  // Explicitly using UnityEngine.UI.Text for timer
    public float maxSurvivalTime = 60f;  // Maximum survival time before losing
    private float survivalTimer = 0f;    // Tracks how long the player has survived
    private float distanceTraveled = 0f; // Tracks how far the player has moved
    private bool gameOver = false;       // Flag to check if the game is over

    void Start()
    {
        if (retryPopup != null)
            retryPopup.SetActive(false);  // Hide retry popup initially
    }

    void Update()
    {
        if (!gameOver)
        {
            survivalTimer += Time.deltaTime;  // Increase survival timer
            distanceTraveled += Vector3.Distance(transform.position, transform.position + transform.forward * Time.deltaTime); // Track distance

            if (survivalTimer >= maxSurvivalTime)
            {
                EndGame();  // End the game if the player survives the max time
            }

            UpdateTimerText(); // Update the timer display
        }
        else
        {
            // If the game is over, continue updating the timer and show distance
            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        // Display the survival timer and distance
        if (gameOver)
        {
            // Show how long the player survived and the distance they traveled
            timerText.text = "You Survived for: " + Mathf.Floor(survivalTimer).ToString() + "s\nDistance: " + Mathf.Floor(distanceTraveled).ToString() + "m";
            scoreText.text = "Game Over!";  // Show "Game Over" when the game ends
        }
        else
        {
            // During gameplay, show how long the player has survived
            timerText.text = "Survival Time: " + Mathf.Floor(survivalTimer).ToString() + "s";
        }
    }

    public void EndGame()
    {
        gameOver = true;
        survivalTimer = Mathf.Min(survivalTimer, maxSurvivalTime);  // Lock survival time to max value
        ShowRetryPopup();  // Show the retry popup
    }

    void ShowRetryPopup()
    {
        if (retryPopup != null)
        {
            retryPopup.SetActive(true);  // Enable the retry popup
        }
    }

    // This method should be called when the player fails (out of camera view or falls off)
    public void PlayerFailed()
    {
        EndGame();  // Trigger the end game logic when the player fails
    }

    public void RetryGame()
    {
        // Reset the game (player position, survival timer, etc.)
        gameOver = false;
        survivalTimer = 0f;
        distanceTraveled = 0f;
        if (retryPopup != null)
            retryPopup.SetActive(false);  // Hide retry popup
    }
}
