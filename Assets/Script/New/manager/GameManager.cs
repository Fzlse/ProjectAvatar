using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameOverPanel;         // Panel utama saat game over
    public GameObject confirmationPanel;     // Panel konfirmasi "Save Score?"
    public GameObject saveScorePanel;        // Panel input nama
    public TMP_InputField nameInputField;    // Input nama pemain
    public TextMeshProUGUI finalScoreText;   // Teks skor akhir
    public TextMeshProUGUI scoreText;        // Teks skor real-time

    [Header("Score Settings")]
    public Transform player;                 // Referensi player
    private Vector3 startPosition;
    private float score = 0f;

    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

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
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (saveScorePanel != null) saveScorePanel.SetActive(false);

        isGameOver = false;
        Time.timeScale = 1f;

        if (player != null)
            startPosition = player.position;
    }

    private void Update()
    {
        if (!isGameOver && player != null)
        {
            score = Vector3.Distance(startPosition, player.position);
            if (scoreText != null)
                scoreText.text = "Score: " + Mathf.FloorToInt(score);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        int finalScore = Mathf.FloorToInt(score);
        Time.timeScale = 0f;

        Debug.Log($"Game Over! Final Score: {finalScore}");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + finalScore;
        }

        if (ScoreManager.Instance != null && ScoreManager.Instance.IsTopFive(finalScore))
        {
            if (confirmationPanel != null)
                confirmationPanel.SetActive(true);
        }
        else
        {
            if (confirmationPanel != null)
                confirmationPanel.SetActive(false);
        }
    }

    public void ChooseSaveScore(bool save)
    {
        if (save)
        {
            if (confirmationPanel != null) confirmationPanel.SetActive(false);
            if (saveScorePanel != null) saveScorePanel.SetActive(true);
        }
        else
        {
            BackToMainMenu();
        }
    }

    public void ConfirmSaveScore()
    {
        int finalScore = Mathf.FloorToInt(score);
        string playerName = nameInputField != null ? nameInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(playerName))
            playerName = "Player";

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(playerName, finalScore);
            ScoreManager.Instance.openLeaderboardOnMenu = true; // auto buka leaderboard
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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

    public int GetScore()
    {
        return Mathf.FloorToInt(score);
    }
}
