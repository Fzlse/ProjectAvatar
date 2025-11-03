using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject leaderboardPanel;

    [Header("Slot Tetap (maks 5)")]
    // urutkan dari rank 1 sampai 5 di Inspector
    public TextMeshProUGUI[] rankTexts;     // optional, bisa diisi "1,2,3,4,5" atau kosong
    public Image[]           rankIcons;     // icon medali per baris (boleh null)
    public TextMeshProUGUI[] nameTexts;     // nama per baris
    public TextMeshProUGUI[] scoreTexts;    // skor per baris
    public GameObject[]      rows;          // root object setiap baris untuk hide/show

    [Header("Ikon Medali (opsional)")]
    // 0=Gold, 1=Silver, 2=Bronze, 3=4th, 4=5th
    public Sprite[] medalSprites = new Sprite[5];

    private const int MAX = 5;

    void Start()
    {
        // kalau dari GameManager disuruh buka leaderboard
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

        // ambil dari server setiap kali dibuka biar sinkron
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RefreshFromServer(MAX, OnLeaderboardLoaded);
        }
        else
        {
            // kalau nggak ada score manager, clear aja
            RefreshFixedRows(null);
        }
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
    }

    // callback setelah fetch ke server
    private void OnLeaderboardLoaded(List<ScoreEntry> list)
    {
        RefreshFixedRows(list);
    }

    private void RefreshFixedRows(List<ScoreEntry> list)
    {
        // list bisa null kalau request gagal
        int count = (list != null) ? Mathf.Min(list.Count, MAX) : 0;

        for (int i = 0; i < MAX; i++)
        {
            bool hasData = i < count;

            // show/hide row
            if (rows != null && i < rows.Length && rows[i] != null)
                rows[i].SetActive(hasData);

            if (!hasData)
                continue;

            var entry = list[i];

            // rank text
            if (rankTexts != null && i < rankTexts.Length && rankTexts[i] != null)
                rankTexts[i].text = (i + 1).ToString();

            // name
            if (nameTexts != null && i < nameTexts.Length && nameTexts[i] != null)
                nameTexts[i].text = entry.playerName;

            // score
            if (scoreTexts != null && i < scoreTexts.Length && scoreTexts[i] != null)
                scoreTexts[i].text = entry.score.ToString("N0"); // 10,000

            // medal
            if (rankIcons != null && i < rankIcons.Length && rankIcons[i] != null)
            {
                if (medalSprites != null && medalSprites.Length > i && medalSprites[i] != null)
                {
                    rankIcons[i].sprite = medalSprites[i];
                    rankIcons[i].enabled = true;
                }
                else
                {
                    rankIcons[i].enabled = false;
                }
            }
        }

        // kalau list kosong/null, pastikan baris lain dimatikan
        if (count == 0 && rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                if (i < MAX && rows[i] != null)
                    rows[i].SetActive(false);
            }
        }
    }
}
