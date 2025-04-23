using UnityEngine;

public class DisplayOptions : MonoBehaviour
{
    public void SetWindowedMode()
    {
        Screen.fullScreen = false;
        Screen.SetResolution(1280, 720, false); // Width, Height, Windowed
    }

    public void SetFullscreenMode()
    {
        Screen.fullScreen = true;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
