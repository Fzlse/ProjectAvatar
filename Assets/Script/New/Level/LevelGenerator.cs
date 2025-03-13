using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public int poolSize = 5;
    public float spawnX = 0f;
    public float platformWidth = 10f;
    public float minY = -2f;
    public float maxY = 2f;
    public Transform playerTransform;
    public float safeZone = 15f;
    public float deleteOffset = 20f;

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float lastSpawnX;

    void Start()
    {
        StartCoroutine(FindPlayerTransform());  // Cari Player dengan Coroutine

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
    }

    void SpawnPlatform()
    {
        float randomY = Random.Range(minY, maxY);
        GameObject platform = Instantiate(platformPrefab, new Vector3(spawnX, randomY, 0), Quaternion.identity);
        activePlatforms.Add(platform);

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
                Debug.Log("Player Transform ditemukan kembali!");
                break;
            }
        }
    }

    public void OnGameRetry()
    {
        Debug.Log("Game di-retry, mencari ulang player...");
        playerTransform = null;
        StartCoroutine(FindPlayerTransform());
    }
}
