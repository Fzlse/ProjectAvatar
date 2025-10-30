using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int score;
}

public class ScoreApiClient : MonoBehaviour
{
    private const string BASE_URL = "https://localhost:7046"; // Ganti sesuai port API kamu

    public IEnumerator SubmitScore(string playerName, int score)
    {
        PlayerScore data = new PlayerScore { playerName = playerName, score = score };

        string json = JsonUtility.ToJson(data);
        using (UnityWebRequest req = new UnityWebRequest(BASE_URL + "/api/scores", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[API] Sending score: {json}");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[API] Score submitted successfully!");
            }
            else
            {
                Debug.LogError($"[API] Error submitting score: {req.error}");
            }
        }
    }
}
