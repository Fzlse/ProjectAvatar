using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public int poolSize = 5;
    public float spawnX = 0f;
    public float platformWidth = 10f;
    public float minY = -2f;
    public float maxY = 2f;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float enemySpawnChance = 0.3f;
    public float minEnemyDistance = 10f;
    public float maxEnemyOffsetX = 3f; // Enemy bisa spawn agak bergeser dari platform
    public float enemyOffsetY = 1.5f;
    public float enemySpawnDelay = 10f; // Waktu delay sebelum musuh mulai spawn (bisa diatur di Inspector)

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool canSpawnEnemies = false; // Cegah spawn musuh di awal

    [Header("Player & Cleanup Settings")]
    public Transform playerTransform;
    public float safeZone = 15f;
    public float deleteOffset = 20f;

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnX;

    void Start()
    {
        StartCoroutine(FindPlayerTransform());
        StartCoroutine(EnableEnemySpawnAfterDelay(enemySpawnDelay)); // Gunakan nilai dari Inspector

        for (int i = 0; i < poolSize; i++)
        {
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
        GameObject platform = Instantiate(platformPrefab, new Vector3(spawnX, randomY, 0), Quaternion.identity);
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
            }
        }

        lastSpawnX = spawnX;
        spawnX += platformWidth;
    }

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
        StartCoroutine(EnableEnemySpawnAfterDelay(enemySpawnDelay)); // Pakai nilai delay dari Inspector
    }
}
