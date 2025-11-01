using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject pausePanel;            // Panel Pause
    public GameObject gameOverPanel;         // Panel utama saat game over
    public GameObject confirmationPanel;     // Panel konfirmasi "Save Score?"
    public GameObject saveScorePanel;        // Panel input nama
    public TMP_InputField nameInputField;    // Input nama pemain
    public TextMeshProUGUI finalScoreText;   // Teks skor akhir
    public TextMeshProUGUI scoreText;        // Teks skor real-time

    [Header("References")]
    public Transform player;
    public LevelGenerator levelGen;
    public Transform cameraTransform;   // NEW


    // --- Skor: baseline + delta jarak dari baselinePos agar bisa resume mulus
    private Vector3 baselinePos;             // posisi referensi untuk menghitung delta jarak
    private float scoreBaseline = 0f;        // skor tersimpan saat resume / saat tekan Resume
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        // Panel default
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (confirmationPanel) confirmationPanel.SetActive(false);
        if (saveScorePanel) saveScorePanel.SetActive(false);

        isGameOver = false;
        Time.timeScale = 1f;

        // ====== RESUME FLOW ======
        if (SaveSystem.ResumeRequested && SaveSystem.HasSave())
        {
            var data = SaveSystem.Load();
            if (data != null)
            {
                // Player position
                if (player != null) player.position = data.playerPosition;

                // Score baseline & display
                scoreBaseline = data.score;
                baselinePos = player != null ? player.position : Vector3.zero;
                UpdateScoreText();

                // Level restore
                if (levelGen != null)
                {
                    levelGen.disableAutoSpawnAtStart = true; // jangan auto-spawn pool awal
                    levelGen.RestoreFromSave(data);
                }
            }

            SaveSystem.ResumeRequested = false; // reset flag
        }
        else
        {
            // New run: baseline dari posisi awal player
            baselinePos = player != null ? player.position : Vector3.zero;
            scoreBaseline = 0f;
            UpdateScoreText();
        }
    }

    private void Update()
    {
        if (!isGameOver && player != null)
        {
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        float delta = player != null ? Vector3.Distance(baselinePos, player.position) : 0f;
        int shown = Mathf.FloorToInt(scoreBaseline + delta);
        if (scoreText) scoreText.text = "Score: " + shown;
    }

    private int GetShownScore()
    {
        float delta = player != null ? Vector3.Distance(baselinePos, player.position) : 0f;
        return Mathf.FloorToInt(scoreBaseline + delta);
    }

    // ======================
    // === GAME OVER LOGIC ==
    // ======================
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        int finalScore = GetShownScore();
        Time.timeScale = 0f;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = "Final Score: " + finalScore;

        // Leaderboard confirmation sesuai logika lama
        if (ScoreManager.Instance != null && ScoreManager.Instance.IsTopFive(finalScore))
        {
            if (confirmationPanel) confirmationPanel.SetActive(true);
        }
        else
        {
            if (confirmationPanel) confirmationPanel.SetActive(false);
        }
    }

    // ============================
    // === KONFIRMASI SAVE SCORE ==
    // ============================
    public void ChooseSaveScore(bool save)
    {
        if (save)
        {
            if (confirmationPanel) confirmationPanel.SetActive(false);
            if (saveScorePanel) saveScorePanel.SetActive(true);
        }
        else
        {
            BackToMainMenu();
        }
    }

    public void ConfirmSaveScore()
    {
        int finalScore = GetShownScore();
        string playerName = nameInputField != null ? nameInputField.text.Trim() : "";
        if (string.IsNullOrEmpty(playerName)) playerName = "Player";

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(playerName, finalScore);
            ScoreManager.Instance.openLeaderboardOnMenu = true; // auto buka leaderboard di MainMenu
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ==================
    // === PAUSE FLOW ===
    // ==================
    public void PauseGame()
    {
        if (isGameOver) return;
        Time.timeScale = 0f;
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (isGameOver) return;
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;

        // jadikan posisi saat ini sebagai baseline baru supaya skor lanjut halus
        scoreBaseline = GetShownScore();
        baselinePos = player != null ? player.position : baselinePos;
        UpdateScoreText();
    }

    // ============================
    // === SAVE & EXIT TO MENU  ===
    // ============================
    public void SaveAndExit()
{
    var data = new SaveData
    {
        playerPosition = player ? player.position : Vector3.zero,
        score = GetShownScore(),
        spawnX = levelGen ? levelGen.spawnX : 0f,
        cameraPosition = cameraTransform ? cameraTransform.position : Vector3.zero,   // NEW
        cameraRotation = cameraTransform ? cameraTransform.rotation : Quaternion.identity, // optional
        platforms = levelGen ? levelGen.CapturePlatforms() : new List<PlatformState>()
    };

    SaveSystem.Save(data);

    Time.timeScale = 1f;
    isGameOver = false;
    SceneManager.LoadScene("MainMenu");
}


    // =======================
    // === BACK TO MAIN MENU ==
    // =======================
    public void BackToMainMenu()
    {
        // digunakan saat user memilih "No" pada leaderboard, atau cancel dsb.
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}
