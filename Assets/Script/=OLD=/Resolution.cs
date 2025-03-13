using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Resolution : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    private UnityEngine.Resolution[] availableResolutions;

    void Start()
    {
        // Get all available screen resolutions
        availableResolutions = Screen.resolutions;

        // Clear existing options in the dropdown
        resolutionDropdown.ClearOptions();

        // Prepare a list of resolution strings
        List<string> resolutionOptions = new List<string>();

        // Populate the dropdown with available resolutions
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string resolutionText = availableResolutions[i].width + " x " + availableResolutions[i].height;
            resolutionOptions.Add(resolutionText);
        }

        // Add resolution options to the dropdown
        resolutionDropdown.AddOptions(resolutionOptions);

        // Load the saved resolution setting, if any
        LoadResolution();
    }

    public void SetResolution(int resolutionIndex)
    {
        // Set the selected resolution from the dropdown
        UnityEngine.Resolution selectedResolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

        // Save the selected resolution to PlayerPrefs
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    void LoadResolution()
    {
        // Check if a resolution was previously saved
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");

            // Set the dropdown to the saved resolution and apply it
            resolutionDropdown.value = savedResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            SetResolution(savedResolutionIndex);  // Apply the saved resolution
        }
        else
        {
            // If no resolution is saved, set the current screen resolution as default
            SetCurrentResolution();
        }
    }

    void SetCurrentResolution()
    {
        // Automatically select the current resolution in the dropdown
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (Screen.currentResolution.width == availableResolutions[i].width &&
                Screen.currentResolution.height == availableResolutions[i].height)
            {
                resolutionDropdown.value = i;
                break;
            }
        }
        resolutionDropdown.RefreshShownValue();
    }
}
