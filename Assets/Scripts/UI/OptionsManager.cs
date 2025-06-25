using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;
    public float defaultMouseSens = 0.07f;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SaveSystem.LoadOptions();
    }

    #region Save and Load

    public void Save(ref OptionsSaveData data)
    {
        data.mouseSens = defaultMouseSens;
        data.isFullscreen = Screen.fullScreen;
        data.resWidth = Screen.currentResolution.width;
        data.resHeight = Screen.currentResolution.height;
        data.qualIndex = QualitySettings.GetQualityLevel();
    }

    public void Load(OptionsSaveData data)
    {
        defaultMouseSens = data.mouseSens;
        Screen.fullScreen = data.isFullscreen;
        Screen.SetResolution(data.resWidth, data.resHeight, Screen.fullScreen);
        QualitySettings.SetQualityLevel(data.qualIndex);
    }

    public void SaveDefault(ref OptionsSaveData data)
    {
        data.mouseSens = OptionsSaveData.GetDefault().mouseSens;
        data.isFullscreen = OptionsSaveData.GetDefault().isFullscreen;
        data.resWidth = OptionsSaveData.GetDefault().resWidth;
        data.resHeight = OptionsSaveData.GetDefault().resHeight;
        data.qualIndex = OptionsSaveData.GetDefault().qualIndex;
    }

    #endregion

}

[System.Serializable]
public struct OptionsSaveData
{
    public float mouseSens;
    public bool isFullscreen;
    public int resWidth;
    public int resHeight;
    public int qualIndex;

    public static OptionsSaveData GetDefault()
    {
        Resolution maxRes = Screen.resolutions.Length > 0 ?
            Screen.resolutions[Screen.resolutions.Length - 1] :
            Screen.currentResolution;

        return new OptionsSaveData
        {
            mouseSens = 0.07f,
            resWidth = maxRes.width,
            resHeight = maxRes.height,
            qualIndex = QualitySettings.names.Length - 1,
            isFullscreen = true
        };
    }
    
}

