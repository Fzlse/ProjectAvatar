using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LeaderboardPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject leaderboardPanel;

    [Header("Slot Tetap (maks 5)")]
    // urutkan dari rank 1 sampai 5 di Inspector
    public TextMeshProUGUI[] rankTexts;     // optional, bisa diisi "1,2,3,4,5" atau kosong
    public Image[]          rankIcons;      // icon medali per baris (boleh null)
    public TextMeshProUGUI[] nameTexts;     // nama per baris
    public TextMeshProUGUI[] scoreTexts;    // skor per baris
    public GameObject[]     rows;           // root object setiap baris untuk hide/show

    [Header("Ikon Medali (opsional)")]
    // 0=Gold, 1=Silver, 2=Bronze, 3=4th, 4=5th (hitam/putih dsb)
    public Sprite[] medalSprites = new Sprite[5];

    private const int MAX = 5;

    void Start()
    {
        if (ScoreManager.Instance != null && ScoreManager.Instance.openLeaderboardOnMenu)
        {
            ScoreManager.Instance.openLeaderboardOnMenu = false;
            OpenLeaderboard();
        }
        else
        {
            if (leaderboardPanel) leaderboardPanel.SetActive(false);
        }
    }

    public void OpenLeaderboard()
    {
        if (leaderboardPanel) leaderboardPanel.SetActive(true);
        RefreshFixedRows();
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
        // tidak perlu clear apa-apa karena slot statis
    }

    private void RefreshFixedRows()
    {
        // safety
        if (ScoreManager.Instance == null) return;

        // ambil data top-5 (ScoreManager sudah menjaga ini)
        var list = ScoreManager.Instance.scores.entries
                   .OrderByDescending(e => e.score)
                   .Take(MAX)
                   .ToList();

        for (int i = 0; i < MAX; i++)
        {
            bool hasData = i < list.Count;

            if (rows != null && i < rows.Length && rows[i] != null)
                rows[i].SetActive(hasData);

            if (!hasData) continue;

            // isi teks
            if (rankTexts != null && i < rankTexts.Length && rankTexts[i] != null)
                rankTexts[i].text = (i + 1).ToString();

            if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null)
                nameTexts[i].text = list[i].playerName;

            if (scoreTexts != null && i < scoreTexts.Length && scoreTexts[i] != null)
                scoreTexts[i].text = list[i].score.ToString("N0"); // format 10,000

            // set ikon medali
            if (rankIcons != null && i < rankIcons.Length && rankIcons[i] != null)
            {
                if (medalSprites != null && medalSprites.Length > i && medalSprites[i] != null)
                {
                    rankIcons[i].sprite = medalSprites[i];
                    rankIcons[i].enabled = true;
                }
                else
                {
                    rankIcons[i].enabled = false; // kalau tak ada sprite
                }
            }
        }
    }
}
