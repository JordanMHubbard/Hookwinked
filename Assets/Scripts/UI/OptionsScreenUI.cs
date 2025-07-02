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
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private TMPro.TMP_Dropdown fpsDropdown;
    private Resolution[] resolutions;
    private int[] fpsValues = { 30, 60, 90, 120, 144, 165, 240, -1 };

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
        vsyncToggle.isOn = QualitySettings.vSyncCount == 1;
        SetDefaultMaxFPS();
    }

    private void SetAvailableResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();


        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width % 16 != 0 || resolutions[i].height % 9 != 0) continue;

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

    private void SetDefaultMaxFPS()
    {
        int currentFPS = Application.targetFrameRate;
        int fpsIndex = System.Array.IndexOf(fpsValues, currentFPS);
        if (fpsIndex >= 0)
        {
            fpsDropdown.value = fpsIndex;
        }
        else
        {
            Debug.LogWarning("FPS value not found in list!");
            fpsDropdown.value = fpsValues.Length-1;
        }   
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

    public void SetVsync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        Debug.Log("VSync is " + (isOn ? "ON" : "OFF"));
    }

    public void SetMaxFPS(int fpsIndex)
    {
        Application.targetFrameRate = fpsValues[fpsIndex];
        Debug.Log("Max fps is " +Application.targetFrameRate);
    }

    public void Back()
    {
        SaveSystem.Save();
        if (PauseManager.Instance) PauseManager.Instance.ShowPausedScreen();
        if (MainMenu.Instance) MainMenu.Instance.Show();
        gameObject.SetActive(false);
    }

}



