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
        data.fullscreenMode = Screen.fullScreenMode;
        data.resWidth = Screen.currentResolution.width;
        data.resHeight = Screen.currentResolution.height;
        data.qualIndex = QualitySettings.GetQualityLevel();
        data.isVsyncOn = QualitySettings.vSyncCount == 1 ? true : false;
        data.maxFPS = Application.targetFrameRate;
    }

    public void Load(OptionsSaveData data)
    {
        defaultMouseSens = data.mouseSens;
        Screen.SetResolution(data.resWidth, data.resHeight, data.fullscreenMode);
        QualitySettings.SetQualityLevel(data.qualIndex);
        QualitySettings.vSyncCount = data.isVsyncOn ? 1 : 0;
        Application.targetFrameRate = data.maxFPS;
    }

    public void SaveDefault(ref OptionsSaveData data)
    {
        data.mouseSens = OptionsSaveData.GetDefault().mouseSens;
        data.fullscreenMode = OptionsSaveData.GetDefault().fullscreenMode;
        data.resWidth = OptionsSaveData.GetDefault().resWidth;
        data.resHeight = OptionsSaveData.GetDefault().resHeight;
        data.qualIndex = OptionsSaveData.GetDefault().qualIndex;
        data.isVsyncOn = OptionsSaveData.GetDefault().isVsyncOn;
        data.maxFPS = OptionsSaveData.GetDefault().maxFPS;
    }

    #endregion

}

[System.Serializable]
public struct OptionsSaveData
{
    public float mouseSens;
    public FullScreenMode fullscreenMode;
    public int resWidth;
    public int resHeight;
    public int qualIndex;
    public bool isVsyncOn;
    public int maxFPS;

    public static OptionsSaveData GetDefault()
    {
        Resolution maxRes = Screen.resolutions[Screen.resolutions.Length-1];
        
        return new OptionsSaveData
        {
            mouseSens = 0.07f,
            resWidth = maxRes.width,
            resHeight = maxRes.height,
            qualIndex = QualitySettings.names.Length - 1,
            fullscreenMode = FullScreenMode.FullScreenWindow,
            isVsyncOn = false,
            maxFPS = -1
        };
    }
    
}

