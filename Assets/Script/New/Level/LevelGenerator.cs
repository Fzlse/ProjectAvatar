using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // untuk ToList()

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public int poolSize = 5;
    public float spawnX = 0f;
    public float platformWidth = 10f;
    public float minY = -2f;
    public float maxY = 2f;

    [Header("Moving Platform Settings")]
    public GameObject movingPlatformPrefab;
    public float movingPlatformChance = 0.3f; 

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float enemySpawnChance = 0.3f;
    public float minEnemyDistance = 10f;
    public float maxEnemyOffsetX = 3f;
    public float enemyOffsetY = 1.5f;
    public float enemySpawnDelay = 10f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool canSpawnEnemies = false;

    [Header("Player & Cleanup Settings")]
    public Transform playerTransform;
    public float safeZone = 15f;
    public float deleteOffset = 20f;

    [HideInInspector] public List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnX;

    // === tambahan: agar Start bisa skip auto-spawn saat resume ===
    [HideInInspector] public bool disableAutoSpawnAtStart = false;

    void Start()
    {
        StartCoroutine(FindPlayerTransform());
        StartCoroutine(EnableEnemySpawnAfterDelay(enemySpawnDelay));

        if (!disableAutoSpawnAtStart)
        {
            for (int i = 0; i < poolSize; i++)
                SpawnPlatform();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (playerTransform.position.x - safeZone > lastSpawnX - (poolSize * platformWidth))
        {
            SpawnPlatform();
        }

        DeleteOldPlatforms();
        DeleteOldEnemies();
    }

    void SpawnPlatform()
    {
        float randomY = Random.Range(minY, maxY);

        bool allowMovingPlatform = activePlatforms.Count >= 3;
        bool isMovingPlatform = allowMovingPlatform && (Random.value < movingPlatformChance);

        GameObject prefab = isMovingPlatform ? movingPlatformPrefab : platformPrefab;

        GameObject platform = Instantiate(prefab, new Vector3(spawnX, randomY, 0), Quaternion.identity);
        activePlatforms.Add(platform);

        if (canSpawnEnemies && enemyPrefab != null && Random.value < enemySpawnChance && playerTransform != null)
        {
            float distanceFromPlayer = Mathf.Abs(spawnX - playerTransform.position.x);
            if (distanceFromPlayer >= minEnemyDistance)
            {
                float enemyXOffset = Random.Range(-maxEnemyOffsetX, maxEnemyOffsetX);
                Vector3 enemyPosition = new Vector3(spawnX + enemyXOffset, randomY + enemyOffsetY, 0);
                GameObject enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
                activeEnemies.Add(enemy);

                if (isMovingPlatform)
                    enemy.transform.SetParent(platform.transform);
            }
        }

        lastSpawnX = spawnX;
        spawnX += platformWidth;
    }

    void DeleteOldPlatforms()
    {
        if (activePlatforms.Count > 0)
        {
            var firstPlatform = activePlatforms[0];

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

    void DeleteOldEnemies()
    {
        if (activeEnemies.Count > 0)
        {
            var firstEnemy = activeEnemies[0];

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
                if (playerObj != null) playerTransform = playerObj.transform;
            }

            if (playerTransform != null)
            {
                Debug.Log("Player ditemukan kembali!");
                break;
            }
        }
    }

    IEnumerator EnableEnemySpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canSpawnEnemies = true;
    }

    public void OnGameRetry()
    {
        Debug.Log("Game di-restart, mencari ulang player...");
        playerTransform = null;
        canSpawnEnemies = false;
        StartCoroutine(FindPlayerTransform());
        StartCoroutine(EnableEnemySpawnAfterDelay(enemySpawnDelay));
    }

    // ====== CAPTURE & RESTORE ======

    public List<PlatformState> CapturePlatforms()
    {
        var list = new List<PlatformState>();

        foreach (var go in activePlatforms)
        {
            if (go == null) continue;

            var mp = go.GetComponent<MovingPlatform>();
            if (mp != null)
            {
                list.Add(mp.CaptureState());
            }
            else
            {
                list.Add(new PlatformState
                {
                    position = go.transform.position,
                    isMoving = false
                });
            }
        }

        return list;
    }

    public void RestoreFromSave(SaveData data)
    {
        // bersihkan yang ada
        foreach (var p in activePlatforms) if (p) Destroy(p);
        activePlatforms.Clear();

        foreach (var e in FindObjectsOfType<Enemy>())
            Destroy(e.gameObject);
        // kosongkan list musuh yang dilacak
        // (kalau kamu simpan musuh juga, tambahkan rekonstruksi di sini)
        
        // pulihkan spawnX supaya generator lanjut dari titik ini
        spawnX = data.spawnX;
        lastSpawnX = spawnX; // aman: akan ditimpa ketika Spawn berikutnya

        // spawn ulang platform sesuai state
        foreach (var st in data.platforms)
        {
            var prefab = st.isMoving ? movingPlatformPrefab : platformPrefab;
            var go = Instantiate(prefab, st.position, Quaternion.identity);
            activePlatforms.Add(go);

            if (st.isMoving)
            {
                var mp = go.GetComponent<MovingPlatform>();
                if (mp != null)
                {
                    mp.ApplyState(st);
                }
            }
        }
    }
}
