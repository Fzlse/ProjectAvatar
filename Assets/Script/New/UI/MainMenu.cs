using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LeaderboardPanelController leaderboardCtrl;
    public GameObject mainMenuPanel;
    public GameObject resumeButton; // optional: untuk hide kalau tak ada save

    private void Start()
    {
        if (resumeButton != null)
            resumeButton.SetActive(SaveSystem.HasSave());
    }

    public void Play()
    {
        SaveSystem.ResumeRequested = false;
        SceneManager.LoadScene("gameplay");
    }

    public void Resume()
    {
        if (!SaveSystem.HasSave()) return;
        SaveSystem.ResumeRequested = true;
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
