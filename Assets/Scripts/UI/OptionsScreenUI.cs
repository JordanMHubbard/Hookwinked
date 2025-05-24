using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class OptionsScreenUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        InitializeSettings();
        SetAvailableResolutions();
    }
    private void InitializeSettings()
    {
        mixer.GetFloat("masterVolume", out float vol);
        masterAudioSlider.value = vol;

        mouseSensSlider.value = InputManager.Instance.mouseSensitivity * 10f;
    }
    private void SetAvailableResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();


        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("masterVolume", volume);
    }
    public void SetMouseSens(float sensitivity)
    {
        Debug.Log("Slider sens: " + sensitivity);
        float sens = sensitivity / 10f;
        InputManager.Instance.mouseSensitivity = sens;
    }
    public void SetFullscreen(int windowIndex)
    {
        switch (windowIndex)
        {
            case 0:
                Screen.fullScreen = false;
                break;
            case 1:
                Screen.fullScreen = true;
                break;
        }
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void Back()
    {
        PauseManager.Instance.ShowPausedScreen();
        gameObject.SetActive(false);
    }
}
