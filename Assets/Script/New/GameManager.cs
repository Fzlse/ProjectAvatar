using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Settings")]
    public GameObject gameOverPanel;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    private CameraFollow cameraFollow;
    private bool isGameOver = false;

    private Transform playerSpawnPoint;
    public bool IsGameOver() => isGameOver;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();

        // Temukan PlayerInput di scene
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput tidak ditemukan! Tambahkan PlayerInput sebagai objek terpisah di scene.");
        }

        // Cari posisi spawn default dari prefab
        if (playerPrefab != null)
        {
            playerSpawnPoint = playerPrefab.transform;
        }
        else
        {
            Debug.LogError("Player Prefab belum diassign di GameManager!");
        }

        // Cari player di scene
        currentPlayer = GameObject.FindGameObjectWithTag("Player");
        if (currentPlayer == null)
        {
            Debug.Log("Tidak ada player di scene, spawn sekarang.");
            SpawnPlayer();
        }
        else
        {
            cameraFollow.SetPlayer(currentPlayer.transform);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0.01f;
    }

    public void RetryGame()
    {
        if (!isGameOver) return;

        Debug.Log("Retrying Game...");
        isGameOver = false;
        Time.timeScale = 1f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || playerSpawnPoint == null)
        {
            Debug.LogError("PlayerPrefab atau SpawnPoint tidak ditemukan!");
            return;
        }

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        Debug.Log("Player spawned.");

        // Update referensi player di CameraFollow
        if (cameraFollow != null)
        {
            cameraFollow.SetPlayer(currentPlayer.transform);
        }
    }

    public void OnRetry(InputAction.CallbackContext context)
    {
        if (context.performed && isGameOver)
        {
            RetryGame();
        }
    }
}
