using System.Collections;
using System.Collections.Generic; // List<PlatformState>
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Resume Countdown")]
    public GameObject resumeCountdownPanel;      // Panel countdown (default: inactive)
    public TextMeshProUGUI countdownText;        // TMP text "3..2..1"
    public float resumeCountdownSeconds = 3f;    // durasi countdown
    public float cameraCatchupSpeed = 5f;        // kecepatan kamera ngejar target
    public bool autoCountdownOnSceneResume = true;

    [Header("References")]
    public Transform player;
    public LevelGenerator levelGen;
    public Transform cameraTransform;
    public LoopBG loopBG;

    // Skor: baseline + delta jarak dari baselinePos agar bisa resume mulus
    private Vector3 baselinePos;        // posisi referensi untuk menghitung delta jarak
    private float scoreBaseline = 0f;   // skor tersimpan saat resume / tekan Resume
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
        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(false);

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

                // Restore CAMERA
                if (cameraTransform != null)
                {
                    cameraTransform.position = data.cameraPosition;
                    cameraTransform.rotation = data.cameraRotation;
                }

                // Restore LOOP BG (offset + world pos + sinkron lastCameraPosition)
                if (loopBG != null)
                {
                    loopBG.ApplyState(data.loopBGOffsetX, data.loopBGWorldPos, cameraTransform);
                }

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

                // Auto countdown saat datang dari MainMenu → Gameplay (resume)
                if (autoCountdownOnSceneResume)
                {
                    StartCoroutine(ResumeWithCountdownFromSceneLoad());
                    SaveSystem.ResumeRequested = false;
                    return; // hentikan Start() biar tidak lanjut logika lain
                }
            }

            SaveSystem.ResumeRequested = false; // reset flag kalau tidak ada data
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
        if (!isGameOver && player != null && Time.timeScale > 0f)
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

        // Tutup panel pause, mulai countdown (game tetap paused)
        if (pausePanel) pausePanel.SetActive(false);
        StartCoroutine(ResumeWithCountdown());
    }

    private IEnumerator ResumeWithCountdown()
    {
        // Freeze gameplay
        Time.timeScale = 0f;

        // Hitung target kamera: pertahankan offset relatif current camera → player
        Vector3 startCamPos = cameraTransform ? cameraTransform.position : Vector3.zero;
        Vector3 playerPos   = player ? player.position : startCamPos;
        Vector3 offset      = startCamPos - playerPos;
        Vector3 targetCamPos = playerPos + offset;

        // Tampilkan panel countdown
        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(true);

        float remaining = Mathf.Max(1f, resumeCountdownSeconds); // minimal 1 detik
        while (remaining > 0f)
        {
            if (countdownText)
                countdownText.text = Mathf.CeilToInt(remaining).ToString();

            // Kamera mendekati target pakai unscaled delta
            if (cameraTransform)
            {
                cameraTransform.position = Vector3.MoveTowards(
                    cameraTransform.position,
                    targetCamPos,
                    cameraCatchupSpeed * Time.unscaledDeltaTime
                );
            }

            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(false);

        // Unpause & reset baseline supaya skor lanjut mulus
        scoreBaseline = GetShownScore();
        baselinePos   = player != null ? player.position : baselinePos;
        UpdateScoreText();

        Time.timeScale = 1f;
    }

    // === Auto countdown saat resume dari MainMenu → Gameplay
    private IEnumerator ResumeWithCountdownFromSceneLoad()
    {
        // Freeze gameplay
        Time.timeScale = 0f;

        // Target kamera: pertahankan offset relatif yang ada saat ini
        Vector3 startCamPos = cameraTransform ? cameraTransform.position : Vector3.zero;
        Vector3 playerPos   = player ? player.position : startCamPos;
        Vector3 offset      = startCamPos - playerPos;
        Vector3 targetCamPos = playerPos + offset;

        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(true);

        float remaining = Mathf.Max(1f, resumeCountdownSeconds);
        while (remaining > 0f)
        {
            if (countdownText)
                countdownText.text = Mathf.CeilToInt(remaining).ToString();

            if (cameraTransform)
            {
                cameraTransform.position = Vector3.MoveTowards(
                    cameraTransform.position,
                    targetCamPos,
                    cameraCatchupSpeed * Time.unscaledDeltaTime
                );
            }

            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(false);

        // Reset baseline skor supaya lanjut mulus
        scoreBaseline = GetShownScore();
        baselinePos   = player != null ? player.position : baselinePos;
        UpdateScoreText();

        Time.timeScale = 1f;
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

            cameraPosition = cameraTransform ? cameraTransform.position : Vector3.zero,
            cameraRotation = cameraTransform ? cameraTransform.rotation : Quaternion.identity,

            platforms = levelGen ? levelGen.CapturePlatforms() : new List<PlatformState>(),

            loopBGOffsetX = loopBG ? loopBG.GetCurrentOffsetX() : 0f,
            loopBGWorldPos = loopBG ? loopBG.transform.position : Vector3.zero
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
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}
