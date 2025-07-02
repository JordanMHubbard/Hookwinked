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
        data.isVsyncOn = QualitySettings.vSyncCount == 1 ? true : false;
        data.maxFPS = Application.targetFrameRate;
    }

    public void Load(OptionsSaveData data)
    {
        defaultMouseSens = data.mouseSens;
        Screen.fullScreen = data.isFullscreen;
        Screen.SetResolution(data.resWidth, data.resHeight, Screen.fullScreen);
        QualitySettings.SetQualityLevel(data.qualIndex);
        QualitySettings.vSyncCount = data.isVsyncOn ? 1 : 0;
        Application.targetFrameRate = data.maxFPS;
        Debug.Log("vsync is: " +QualitySettings.vSyncCount);
        Debug.Log("current fps is: " +Application.targetFrameRate);
    }

    public void SaveDefault(ref OptionsSaveData data)
    {
        data.mouseSens = OptionsSaveData.GetDefault().mouseSens;
        data.isFullscreen = OptionsSaveData.GetDefault().isFullscreen;
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
    public bool isFullscreen;
    public int resWidth;
    public int resHeight;
    public int qualIndex;
    public bool isVsyncOn;
    public int maxFPS;

    public static OptionsSaveData GetDefault()
    {
        Resolution maxRes = Screen.currentResolution;
        for (int i = Screen.resolutions.Length - 1; i >= 0; i--)
        {
            if (Screen.resolutions[i].width % 16 == 0 && Screen.resolutions[i].height % 9 == 0)
            {
                maxRes = Screen.resolutions[i];
                break;
            }
        }

        return new OptionsSaveData
        {
            mouseSens = 0.07f,
            resWidth = maxRes.width,
            resHeight = maxRes.height,
            qualIndex = QualitySettings.names.Length - 1,
            isFullscreen = true,
            isVsyncOn = false,
            maxFPS = -1
        };
    }
    
}

