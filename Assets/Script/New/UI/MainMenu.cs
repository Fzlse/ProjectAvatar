using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LeaderboardPanelController leaderboardCtrl;

    public void Play()
    {
        SceneManager.LoadScene("gameplay");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenLeaderboard()
    {
        if (leaderboardCtrl != null)
            leaderboardCtrl.OpenLeaderboard();
    }

    public void CloseLeaderboard()
    {
        if (leaderboardCtrl != null)
            leaderboardCtrl.CloseLeaderboard();
    }
}
