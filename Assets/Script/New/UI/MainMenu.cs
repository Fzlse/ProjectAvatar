using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public LeaderboardPanelController leaderboardCtrl;
    public GameObject mainMenuPanel;
    public GameObject resumeButton; // akan aktif kalau ada save di server

    private void Start()
    {
        if (resumeButton != null)
            resumeButton.SetActive(false);

        // cek ke server: ada save atau nggak
        StartCoroutine(CheckCloudSave());
    }

    private IEnumerator CheckCloudSave()
    {
        string playerId = SystemInfo.deviceUniqueIdentifier;
        bool hasSave = false;

        yield return SaveSystem.DownloadFromServer(
            playerId,
            onOk: data =>
            {
                hasSave = data != null;
            },
            onErr: err =>
            {
                Debug.LogWarning("[MainMenu] Gagal cek save cloud: " + err);
            }
        );

        if (resumeButton != null)
            resumeButton.SetActive(hasSave);
    }

    // NEW GAME
    public void Play()
    {
        SaveSystem.ResumeRequested = false;   // penting: jangan load save
        SceneManager.LoadScene("gameplay");
    }

    // RESUME
    public void Resume()
    {
        SaveSystem.ResumeRequested = true;    // penting: suruh GM ambil dari server
        SceneManager.LoadScene("gameplay");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenLeaderboard()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (leaderboardCtrl != null) leaderboardCtrl.OpenLeaderboard();
    }

    public void CloseLeaderboard()
    {
        if (leaderboardCtrl != null) leaderboardCtrl.CloseLeaderboard();
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }
}
