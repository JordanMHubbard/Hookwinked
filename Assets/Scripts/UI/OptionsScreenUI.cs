using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class OptionsScreenUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private TMPro.TMP_Dropdown windowDropdown;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMPro.TMP_Dropdown qualityDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        InitializeSettings();
    }
    private void InitializeSettings()
    {
        mixer.GetFloat("masterVolume", out float vol);
        masterAudioSlider.value = vol;

        if (InputManager.Instance) mouseSensSlider.value = InputManager.Instance.mouseSensitivity * 10f;
        windowDropdown.value = Screen.fullScreen ? 1 : 0;
        SetAvailableResolutions();
        qualityDropdown.value = QualitySettings.GetQualityLevel();
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
        if (InputManager.Instance) InputManager.Instance.mouseSensitivity = sens;
        OptionsManager.Instance.defaultMouseSens = sens;
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
        SaveSystem.Save();
        if (PauseManager.Instance) PauseManager.Instance.ShowPausedScreen();
        if (MainMenu.Instance) MainMenu.Instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}



