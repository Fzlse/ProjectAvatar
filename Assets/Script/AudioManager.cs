using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer; // Reference to your Audio Mixer
    public Slider volumeSlider;    // Reference to the UI Slider

    private void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject); // Keep this GameObject across scenes
    }

    private void Start()
    {
        // Load the saved volume level from PlayerPrefs or set a default
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0f);
        SetVolume(savedVolume); // Set the initial volume from saved preferences
        volumeSlider.value = savedVolume; // Update the slider UI
        volumeSlider.onValueChanged.AddListener(SetVolume); // Add listener for slider value changes
    }

    // Method to set the volume in the Audio Mixer
    public void SetVolume(float volume)
    {
        // Set the exposed parameter "Volume" in the Audio Mixer
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume); // Save the volume level
        PlayerPrefs.Save(); // Ensure changes are saved
    }
}
