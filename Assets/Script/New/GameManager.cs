using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Settings")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Score Settings")]
    public Transform player;
    private Vector3 startPosition;
    private float score = 0f;
    public TextMeshProUGUI scoreText;

    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    [Header("API Settings")]
    public string playerName = "Player1"; // sementara default
    private ScoreApiClient apiClient;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1f;

        if (player != null)
            startPosition = player.position;

        apiClient = gameObject.AddComponent<ScoreApiClient>(); // attach runtime
    }

    private void Update()
    {
        if (isGameOver && Keyboard.current.rKey.wasPressedThisFrame)
        {
            BackToMainMenu();
        }

        if (!isGameOver && player != null)
        {
            score = Vector3.Distance(startPosition, player.position);
            scoreText.text = "Score: " + Mathf.FloorToInt(score);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        int finalScore = Mathf.FloorToInt(score);
        Debug.Log($"Game Over! Final Score: {finalScore}");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + finalScore;
        }

        Time.timeScale = 0f;

        // Kirim skor ke API
        StartCoroutine(apiClient.SubmitScore(playerName, finalScore));
    }

    public void BackToMainMenu()
    {
        if (!isGameOver) return;

        Debug.Log("Returning to Main Menu...");
        Time.timeScale = 1f;
        isGameOver = false;
        Destroy(Instance.gameObject);

        SceneManager.LoadScene("MainMenu");
    }

    public int GetScore() => Mathf.FloorToInt(score);
}
