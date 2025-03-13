using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    public AudioMixer audioMixer;  // Reference to the Audio Mixer
    public Slider volumeSlider;     // Reference to the Slider UI component

    private void Start()
    {
        // Set slider value based on the current audio mixer volume
        float currentVolume;
        audioMixer.GetFloat("MyExposedParam", out currentVolume);
        volumeSlider.value = currentVolume;  // Initialize the slider with the current volume

        // Add a listener to handle changes to the slider
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Method to set the audio mixer volume based on slider value
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MyExposedParam", volume);  // Set the exposed parameter to the slider value
        Debug.Log("Setting volume to: " + volume);
    }
}
