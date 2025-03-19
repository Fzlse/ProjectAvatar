using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip gameplayBGM;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayBGM(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopBGM();
        PlayBGM(scene.name);
    }

    void PlayBGM(string sceneName)
    {
        if (bgmSource == null)
        {
            Debug.LogError("AudioSource tidak ditemukan!");
            return;
        }

        AudioClip selectedBGM = sceneName == "MainMenu" ? mainMenuBGM : gameplayBGM;
        if (selectedBGM == null)
        {
            Debug.LogWarning($"BGM untuk {sceneName} tidak tersedia!");
            return;
        }

        if (!bgmSource.enabled) bgmSource.enabled = true;

        bgmSource.clip = selectedBGM;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying) bgmSource.Stop();
    }
}
