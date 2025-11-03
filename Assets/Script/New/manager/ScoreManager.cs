// Assets/Script/New/manager/ScoreManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectAvatar.API; // DTO API kita

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Tooltip("URL path top leaderboard")]
    public string topPath = "/api/leaderboard/top?limit=5";

    [Tooltip("Biar bisa auto buka panel di MainMenu")]
    public bool openLeaderboardOnMenu = false;

    private List<ScoreEntry> _lastFetched = new List<ScoreEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =======================
    // === PUBLIC METHODS ====
    // =======================

    /// <summary>
    /// Submit ke server. Tidak simpan ke PlayerPrefs.
    /// </summary>
    public void SubmitToServer(string playerId, string playerName, int score)
    {
        StartCoroutine(ApiClient.PostJson<SubmitScoreReq, SubmitScoreRes>(
            "/api/leaderboard/submit",
            new SubmitScoreReq { playerId = playerId, playerName = playerName, score = score },
            onOk: res => Debug.Log($"[API] submit ok, rank={res.rank}"),
            onErr: err => Debug.LogWarning("[API] submit gagal: " + err)
        ));
    }

    /// <summary>
    /// Ambil top dari server lalu simpan di memori (bukan PlayerPrefs).
    /// </summary>
    public void RefreshFromServer(int limit = 5, System.Action<List<ScoreEntry>> onDone = null)
    {
        StartCoroutine(ApiClient.GetJson<TopRes>(
            $"/api/leaderboard/top?limit={limit}",
            onOk: res =>
            {
                _lastFetched.Clear();
                if (res != null && res.entries != null)
                {
                    foreach (var e in res.entries)
                    {
                        _lastFetched.Add(new ScoreEntry
                        {
                            playerName = e.playerName,
                            score = e.score
                        });
                    }
                }
                onDone?.Invoke(_lastFetched);
            },
            onErr: err =>
            {
                Debug.LogWarning("[API] fetch top gagal: " + err);
                onDone?.Invoke(null);
            }
        ));
    }

    /// <summary>
    /// Data terakhir yang sudah di-fecth dari server
    /// </summary>
    public List<ScoreEntry> GetCached()
    {
        return _lastFetched;
    }

    /// <summary>
    /// Check apakah skor cukup masuk leaderboard yang *sudah diambil dari server*.
    /// Kalau belum pernah fetch, anggap saja true biar user bisa submit.
    /// </summary>
    public bool IsTopFromServer(int score, int currentCount = 5)
    {
        if (_lastFetched == null || _lastFetched.Count == 0) return true;
        if (_lastFetched.Count < currentCount) return true;
        return score > _lastFetched[_lastFetched.Count - 1].score;
    }
}

// kelas kecil buat UI
[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
}
