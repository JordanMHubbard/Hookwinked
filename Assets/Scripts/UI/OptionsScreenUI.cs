using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class OptionsScreenUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private TextMeshProUGUI masterAudioText;
    [SerializeField] private TextMeshProUGUI mouseSensText;
    [SerializeField] private TMPro.TMP_Dropdown windowDropdown;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMPro.TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private TMPro.TMP_Dropdown fpsDropdown;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private Resolution selectedResolution;
    private RefreshRate currentRefreshRate;
    private FullScreenMode fullscreenMode;
    private int[] fpsValues = { 30, 60, 90, 120, 144, 165, 240, -1 };
    private bool hasInitialized = false;

    private void Start()
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        mixer.GetFloat("masterVolume", out float vol);
        masterAudioSlider.value = Mathf.Round((vol + 80f) * 1.25f);
        masterAudioText.text = masterAudioSlider.value.ToString();

        if (InputManager.Instance) mouseSensSlider.value = InputManager.Instance.mouseSensitivity * 10f;
        mouseSensText.text = (Mathf.Round(mouseSensSlider.value * 100f) / 100f).ToString();
        SetCurrentWindowMode();
        SetAvailableResolutions();
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        vsyncToggle.isOn = QualitySettings.vSyncCount == 1;
        SetDefaultMaxFPS();
        hasInitialized = true;
    }

    private void SetCurrentWindowMode()
    {
        fullscreenMode = Screen.fullScreenMode;
        switch (fullscreenMode)
        {
            case FullScreenMode.Windowed:
                windowDropdown.value = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                windowDropdown.value = 1;
                break;
            case FullScreenMode.ExclusiveFullScreen:
                windowDropdown.value = 2;
                break;
        }
    }

    private void SetAvailableResolutions()
    {
        selectedResolution = Screen.currentResolution;
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        List<string> resOptions = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate.value)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            resOptions.Add(option);
            if (filteredResolutions[i].width == Screen.width &&
                filteredResolutions[i].height == Screen.height)
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
        if (!hasInitialized) return;

        float masterVol = Mathf.Round((volume / 100 * 30f) - 30f);

        if (Mathf.Approximately(volume, 0)) mixer.SetFloat("masterVolume", -80);
        else mixer.SetFloat("masterVolume", masterVol);

        masterAudioText.text = Mathf.Round(volume).ToString();
    }

    public void SetMouseSens(float sensitivity)
    {
        if (!hasInitialized) return;

        float sensText = Mathf.Round(sensitivity * 100f) / 100f;
        mouseSensText.text = sensText.ToString();

        float sens = sensitivity / 10f;
        if (InputManager.Instance) InputManager.Instance.mouseSensitivity = sens;
        OptionsManager.Instance.defaultMouseSens = sens;
    }
    
    public void SetFullscreen(int windowIndex)
    {
        if (!hasInitialized) return;

        switch (windowIndex)
        {
            case 0:
                fullscreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenMode);
                break;
            case 1:
                fullscreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenMode);
                break;
            case 2:
                fullscreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenMode);
                break;
        }
    }

    public void SetResolution(int resIndex)
    {
        if (!hasInitialized) return;
        
        selectedResolution = filteredResolutions[resIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenMode);
    }

    public void SetQuality(int qualityIndex)
    {
        if (!hasInitialized) return;

        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetVsync(bool isOn)
    {
        if (!hasInitialized) return;

        QualitySettings.vSyncCount = isOn ? 1 : 0;
        Debug.Log("VSync is " + (isOn ? "ON" : "OFF"));
    }

    public void SetMaxFPS(int fpsIndex)
    {
        if (!hasInitialized) return;

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



