using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Settings")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText; // Tambahkan ini untuk menampilkan skor akhir

    [Header("Score Settings")]
    public Transform player;
    private Vector3 startPosition;
    private float score = 0f;
    public TextMeshProUGUI scoreText;

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
        isGameOver = false;
        Time.timeScale = 1f;

        if (player != null)
            startPosition = player.position;
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
        Debug.Log("Game Over! Final Score: " + Mathf.FloorToInt(score));

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + Mathf.FloorToInt(score);
        }

        Time.timeScale = 0f;
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
