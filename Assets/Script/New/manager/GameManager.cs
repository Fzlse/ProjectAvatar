using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using ProjectAvatar.API;   // <— pakai DTO API yang barusan

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject confirmationPanel;
    public GameObject saveScorePanel;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI scoreText;

    [Header("Resume Countdown")]
    public GameObject resumeCountdownPanel;
    public TextMeshProUGUI countdownText;
    public float resumeCountdownSeconds = 3f;
    public float cameraCatchupSpeed = 5f;
    public bool autoCountdownOnSceneResume = true;

    [Header("References")]
    public Transform player;
    public LevelGenerator levelGen;
    public Transform cameraTransform;
    public LoopBG loopBG;

    [Header("Cloud API")]
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
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (confirmationPanel) confirmationPanel.SetActive(false);
        if (saveScorePanel) saveScorePanel.SetActive(false);
        if (resumeCountdownPanel) resumeCountdownPanel.SetActive(false);

        isGameOver = false;
        Time.timeScale = 1f;

        if (SaveSystem.ResumeRequested && SaveSystem.HasSave())
        {
            var data = SaveSystem.Load();
            if (data != null)
            {
                if (player) player.position = data.playerPosition;
                if (cameraTransform)
                {
                    cameraTransform.position = data.cameraPosition;
                    cameraTransform.rotation = data.cameraRotation;
                }
                if (loopBG)
                    loopBG.ApplyState(data.loopBGOffsetX, data.loopBGWorldPos, cameraTransform);

                scoreBaseline = data.score;
                baselinePos = player ? player.position : Vector3.zero;
                UpdateScoreText();

                if (levelGen)
                {
                    levelGen.disableAutoSpawnAtStart = true;
                    levelGen.RestoreFromSave(data);
                }

                if (autoCountdownOnSceneResume)
                {
                    StartCoroutine(ResumeWithCountdownFromSceneLoad());
                    SaveSystem.ResumeRequested = false;
                    return;
                }
            }
            SaveSystem.ResumeRequested = false;
        }
        else
        {
            baselinePos = player ? player.position : Vector3.zero;
            scoreBaseline = 0f;
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

        if (ScoreManager.Instance != null && ScoreManager.Instance.IsTopFromServer(finalScore))
        {
            if (confirmationPanel) confirmationPanel.SetActive(true);
        }
        else
        {
            if (confirmationPanel) confirmationPanel.SetActive(false);
        }
    }

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
        string playerName = nameInputField ? nameInputField.text.Trim() : "";
        if (string.IsNullOrEmpty(playerName)) playerName = "Player";

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SubmitToServer(ApiPlayerId, playerName, finalScore);
            ScoreManager.Instance.openLeaderboardOnMenu = true;
        }

        StartCoroutine(SubmitScoreCloud(ApiPlayerId, playerName, finalScore));

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ===== PAUSE =====
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
        StartCoroutine(ResumeWithCountdown());
    }

    private IEnumerator ResumeWithCountdown()
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

    private IEnumerator ResumeWithCountdownFromSceneLoad()
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

    // ===== SAVE & EXIT =====
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
        StartCoroutine(UploadSaveCloud(ApiPlayerId, data));

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

    IEnumerator UploadSaveCloud(string playerId, SaveData local)
    {
        var req = new SaveUpsertReq
        {
            version = 1,
            score = local.score,
            playerPosition = new Vec3 { x = local.playerPosition.x, y = local.playerPosition.y, z = local.playerPosition.z },
            camera = new SaveCamera
            {
                position = new Vec3 { x = local.cameraPosition.x, y = local.cameraPosition.y, z = local.cameraPosition.z },
                rotation = new Quat { x = local.cameraRotation.x, y = local.cameraRotation.y, z = local.cameraRotation.z, w = local.cameraRotation.w }
            },
            level = new SaveLevel
            {
                spawnX = local.spawnX,
                platforms = ConvertPlatforms(local.platforms)
            },
            loopBG = new SaveLoop
            {
                offsetX = local.loopBGOffsetX,
                worldPos = new Vec3 { x = local.loopBGWorldPos.x, y = local.loopBGWorldPos.y, z = local.loopBGWorldPos.z }
            }
        };

        yield return ApiClient.PutJson($"/api/saves/{playerId}", req,
            onOk: () => Debug.Log("[API] Save OK"),
            onErr: err => Debug.LogWarning($"[API] Save gagal: {err}")
        );
    }

    List<ApiPlatformState> ConvertPlatforms(List<PlatformState> src)
    {
        var list = new List<ApiPlatformState>();
        if (src == null) return list;

        foreach (var p in src)
        {
            list.Add(new ApiPlatformState
            {
                position = new Vec3 { x = p.position.x, y = p.position.y, z = p.position.z },
                isMoving = p.isMoving,
                moveDistance = p.moveDistance,
                moveSpeed = p.moveSpeed,
                movingRight = p.movingRight,
                startPosX = p.startPosX
            });
        }
        return list;
    }
}
