using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;   // Prefab untuk platform
    public int poolSize = 5;            // Jumlah platform dalam pool
    public float spawnX = 0f;           // Posisi awal spawn
    public float platformWidth = 10f;   // Lebar platform
    public float minY = -2f;            // Posisi minimum Y platform
    public float maxY = 2f;             // Posisi maksimum Y platform

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;      // Prefab untuk enemy
    public float enemySpawnChance = 0.3f; // 30% kemungkinan muncul enemy
    public float minEnemyDistance = 10f; // Jarak minimal dari player sebelum enemy bisa spawn
    private List<GameObject> activeEnemies = new List<GameObject>(); // List enemy yang aktif

    [Header("Player & Cleanup Settings")]
    public Transform playerTransform;   // Referensi ke player
    public float safeZone = 15f;        // Zona aman sebelum platform baru dibuat
    public float deleteOffset = 20f;    // Jarak di mana platform/enemy dihapus dari scene

    private List<GameObject> activePlatforms = new List<GameObject>(); // List platform yang aktif
    private float lastSpawnX;           // Posisi terakhir spawn platform

    void Start()
    {
        StartCoroutine(FindPlayerTransform());

        // Spawn platform awal
        for (int i = 0; i < poolSize; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Jika player melewati batas safeZone, spawn platform baru
        if (playerTransform.position.x - safeZone > lastSpawnX - (poolSize * platformWidth))
        {
            SpawnPlatform();
        }

        // Hapus platform dan enemy yang sudah terlalu jauh
        DeleteOldPlatforms();
        DeleteOldEnemies();
    }

    /// <summary>
    /// Spawn platform di posisi baru dan kemungkinan menambahkan enemy di atasnya.
    /// </summary>
    void SpawnPlatform()
    {
        float randomY = Random.Range(minY, maxY);
        GameObject platform = Instantiate(platformPrefab, new Vector3(spawnX, randomY, 0), Quaternion.identity);
        activePlatforms.Add(platform);

        // Spawn enemy hanya jika cukup jauh dari player
        if (enemyPrefab != null && Random.value < enemySpawnChance && playerTransform != null)
        {
            float distanceFromPlayer = Mathf.Abs(spawnX - playerTransform.position.x);
            if (distanceFromPlayer >= minEnemyDistance)
            {
                float enemyOffsetY = 1.5f; // Posisikan sedikit di atas platform
                Vector3 enemyPosition = new Vector3(spawnX, randomY + enemyOffsetY, 0);
                GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
                activeEnemies.Add(enemy);
            }
        }

        lastSpawnX = spawnX;
        spawnX += platformWidth;
    }

    /// <summary>
    /// Hapus platform yang sudah melewati batas deleteOffset.
    /// </summary>
    void DeleteOldPlatforms()
    {
        if (activePlatforms.Count > 0)
        {
            GameObject firstPlatform = activePlatforms[0];

            if (firstPlatform == null)
            {
                activePlatforms.RemoveAt(0);
                return;
            }

            if (playerTransform != null && firstPlatform.transform.position.x < playerTransform.position.x - deleteOffset)
            {
                activePlatforms.RemoveAt(0);
                Destroy(firstPlatform);
            }
        }
    }

    /// <summary>
    /// Hapus enemy yang sudah melewati batas deleteOffset.
    /// </summary>
    void DeleteOldEnemies()
    {
        if (activeEnemies.Count > 0)
        {
            GameObject firstEnemy = activeEnemies[0];

            if (firstEnemy == null)
            {
                activeEnemies.RemoveAt(0);
                return;
            }

            if (playerTransform != null && firstEnemy.transform.position.x < playerTransform.position.x - deleteOffset)
            {
                activeEnemies.RemoveAt(0);
                Destroy(firstEnemy);
            }
        }
    }

    /// <summary>
    /// Mencari playerTransform secara otomatis jika belum ditemukan.
    /// </summary>
    IEnumerator FindPlayerTransform()
    {
        while (true)
        {
            yield return null;

            if (playerTransform != null) break;

            playerTransform = FindObjectOfType<PlayerController>()?.transform;

            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
            }

            if (playerTransform != null)
            {
                Debug.Log("Player ditemukan kembali!");
                break;
            }
        }
    }

    /// <summary>
    /// Ketika game di-restart, cari ulang playerTransform.
    /// </summary>
    public void OnGameRetry()
    {
        Debug.Log("Game di-restart, mencari ulang player...");
        playerTransform = null;
        StartCoroutine(FindPlayerTransform());
    }
}
