using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using ProjectAvatar.API;   // pakai DTO API

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject confirmationPanel;
    public GameObject saveScorePanel;
    public TMP_InputField nameInputField;
    public GameObject resumeCountdownPanel;
    public TextMeshProUGUI countdownText;

    [Header("Player & Camera")]
    public Transform player;
    public LevelGenerator levelGen;
    public Transform cameraTransform;
    public LoopBG loopBG;

    [Header("Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;   // << tambah untuk tampilkan skor akhir di Game Over

    [Header("Resume Options")]
    public bool autoCountdownOnSceneResume = true;
    public float resumeCountdownSeconds = 3f;
    public float cameraCatchupSpeed = 5f;

    [Header("API")]
    [Tooltip("Kalau kosong pakai deviceUniqueIdentifier")]
    public string apiPlayerIdOverride = "";

    private Vector3 baselinePos;
    private float scoreBaseline = 0f;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    string ApiPlayerId => string.IsNullOrWhiteSpace(apiPlayerIdOverride)
        ? SystemInfo.deviceUniqueIdentifier
        : apiPlayerIdOverride;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (confirmationPanel) confirmationPanel.SetActive(false);
        if (saveScorePanel) saveScorePanel.SetActive(false);
        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(false);

        isGameOver = false;
        Time.timeScale = 1f;

        StartCoroutine(InitFromCloud());
    }

    private IEnumerator InitFromCloud()
    {
        // default start baru
        baselinePos = player ? player.position : Vector3.zero;
        scoreBaseline = 0f;

        // cuma kalau dari menu tadi pilih "Resume"
        if (SaveSystem.ResumeRequested)
        {
            SaveData remoteData = null;
            string err = null;

            yield return SaveSystem.DownloadFromServer(
                ApiPlayerId,
                onOk: d => remoteData = d,
                onErr: e => err = e
            );

            if (remoteData != null)
            {
                // apply data
                if (player) player.position = remoteData.playerPosition;
                if (cameraTransform)
                {
                    cameraTransform.position = remoteData.cameraPosition;
                    cameraTransform.rotation = remoteData.cameraRotation;
                }
                if (loopBG)
                    loopBG.ApplyState(remoteData.loopBGOffsetX, remoteData.loopBGWorldPos, cameraTransform);

                scoreBaseline = remoteData.score;
                baselinePos = player ? player.position : baselinePos;

                if (levelGen)
                {
                    levelGen.disableAutoSpawnAtStart = true;
                    levelGen.RestoreFromSave(remoteData);
                }

                if (autoCountdownOnSceneResume)
                    yield return StartCoroutine(ResumeWithCountdownFromSceneLoad());
                else
                    UpdateScoreText();
            }
            else
            {
                // resume diminta tapi server gak punya → mulai baru
                UpdateScoreText();
            }

            SaveSystem.ResumeRequested = false; // habis dipakai
        }
        else
        {
            // jalur Play (new game)
            UpdateScoreText();
        }
    }

    private void Update()
    {
        if (!isGameOver && player && Time.timeScale > 0f)
            UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        float delta = player ? Vector3.Distance(baselinePos, player.position) : 0f;
        int shown = Mathf.FloorToInt(scoreBaseline + delta);
        if (scoreText) scoreText.text = "Score: " + shown;
    }

    private int GetShownScore()
    {
        float delta = player ? Vector3.Distance(baselinePos, player.position) : 0f;
        return Mathf.FloorToInt(scoreBaseline + delta);
    }

    // ===== GAME OVER =====
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        int finalScore = GetShownScore();
        Time.timeScale = 0f;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = "Final Score: " + finalScore;

        // Tawarkan konfirmasi submit hanya jika skor layak (berdasar cache top server).
        if (ScoreManager.Instance != null && ScoreManager.Instance.IsTopFromServer(finalScore))
        {
            if (confirmationPanel) confirmationPanel.SetActive(true);
        }
        else
        {
            if (confirmationPanel) confirmationPanel.SetActive(false);
        }
    }

    public void ShowSaveScorePanel()
    {
        if (saveScorePanel) saveScorePanel.SetActive(true);
    }

    public void OnSaveScoreConfirm()
    {
        int score = GetShownScore();
        string playerName = nameInputField ? nameInputField.text.Trim() : "";
        if (string.IsNullOrEmpty(playerName)) playerName = "Player";

        // Hindari double-submit. Pakai ScoreManager agar bisa auto-buka leaderboard di Main Menu.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SubmitToServer(ApiPlayerId, playerName, score);
            ScoreManager.Instance.openLeaderboardOnMenu = true; // Main Menu akan auto buka leaderboard
        }
        else
        {
            // Fallback kalau ScoreManager belum ada di scene
            StartCoroutine(SubmitScoreCloud(ApiPlayerId, playerName, score));
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSaveScoreCancel()
    {
        if (saveScorePanel) saveScorePanel.SetActive(false);
    }

    // ===== PAUSE =====
    public void Pause()
    {
        if (isGameOver) return;

        Time.timeScale = 0f;
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (pausePanel) pausePanel.SetActive(false);

        if (resumeCountdownPanel && resumeCountdownSeconds > 0.1f)
        {
            StartCoroutine(ResumeWithCountdown());
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    IEnumerator ResumeWithCountdown()
    {
        Time.timeScale = 0f;

        Vector3 startCamPos = cameraTransform ? cameraTransform.position : Vector3.zero;
        Vector3 playerPos = player ? player.position : startCamPos;
        Vector3 offset = startCamPos - playerPos;
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

        scoreBaseline = GetShownScore();
        baselinePos = player ? player.position : baselinePos;
        UpdateScoreText();

        Time.timeScale = 1f;
    }

    IEnumerator ResumeWithCountdownFromSceneLoad()
    {
        Time.timeScale = 0f;

        Vector3 startCamPos = cameraTransform ? cameraTransform.position : Vector3.zero;
        Vector3 playerPos = player ? player.position : startCamPos;
        Vector3 offset = startCamPos - playerPos;
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

        scoreBaseline = GetShownScore();
        baselinePos = player ? player.position : baselinePos;
        UpdateScoreText();

        Time.timeScale = 1f;
    }

    // ===== CONFIRMATION (misal di tombol Quit) =====
    public void ShowConfirmation()
    {
        if (confirmationPanel) confirmationPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnConfirmYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnConfirmNo()
    {
        if (confirmationPanel) confirmationPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    // ===== SAVE & EXIT (REST ONLY) =====
    public void SaveAndExit()
    {
        StartCoroutine(SaveAndExitRoutine());
    }

    private IEnumerator SaveAndExitRoutine()
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

        yield return SaveSystem.UploadToServer(
            ApiPlayerId,
            data,
            onOk: () => { },
            onErr: err => { Debug.LogWarning(err); }
        );

        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }

    // ===== Cloud helpers =====
    IEnumerator SubmitScoreCloud(string playerId, string playerName, int score)
    {
        var payload = new SubmitScoreReq { playerId = playerId, playerName = playerName, score = score };
        yield return ApiClient.PostJson<SubmitScoreReq, SubmitScoreRes>(
            "/api/leaderboard/submit",
            payload,
            onOk: res => Debug.Log($"[API] Submit OK, rank = {res.rank}"),
            onErr: err => Debug.LogWarning($"[API] Submit gagal: {err}")
        );
    }
}
