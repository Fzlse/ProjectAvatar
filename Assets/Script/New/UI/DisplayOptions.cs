using UnityEngine;

public class DisplayOptions : MonoBehaviour
{
    private const string FirstLaunchKey = "FirstLaunch";
    private const string FullscreenKey = "IsFullscreen";

    private void Start()
    {
        // Cek apakah ini pertama kali game dijalankan
        if (!PlayerPrefs.HasKey(FirstLaunchKey))
        {
            // Pertama kali dijalankan: aktifkan fullscreen
            SetFullscreenMode();
            PlayerPrefs.SetInt(FullscreenKey, 1); // 1 = fullscreen
            PlayerPrefs.SetInt(FirstLaunchKey, 1); // tandai sudah pernah dibuka
            PlayerPrefs.Save();
        }
        else
        {
            // Sudah pernah dibuka, ambil preferensi sebelumnya
            bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
            ToggleFullscreen(isFullscreen);
        }
    }

    public void SetWindowedMode()
    {
        Screen.fullScreen = false;
        Screen.SetResolution(1280, 720, false); // Width, Height, Windowed
        PlayerPrefs.SetInt(FullscreenKey, 0); // simpan status
        PlayerPrefs.Save();
    }

    public void SetFullscreenMode()
    {
        Screen.fullScreen = true;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        PlayerPrefs.SetInt(FullscreenKey, 1); // simpan status
        PlayerPrefs.Save();
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0); // simpan status
        PlayerPrefs.Save();
    }
}
