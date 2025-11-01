using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class ScoreList
{
    public List<ScoreEntry> entries = new List<ScoreEntry>();
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private const string SaveKey = "LeaderboardData";
    public ScoreList scores = new ScoreList();

    // Flag agar MainMenu otomatis buka leaderboard setelah save
    public bool openLeaderboardOnMenu = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(string name, int value)
    {
        scores.entries.Add(new ScoreEntry { playerName = name, score = value });
        scores.entries = scores.entries.OrderByDescending(e => e.score).Take(5).ToList();
        SaveScores();
    }

    public bool IsTopFive(int score)
    {
        if (scores.entries.Count < 5) return true;
        return score > scores.entries.Last().score;
    }

    public void SaveScores()
    {
        string json = JsonUtility.ToJson(scores);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadScores()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            scores = JsonUtility.FromJson<ScoreList>(json);
        }
    }
}
