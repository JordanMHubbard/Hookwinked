using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;
    public float defaultMouseSens = 0.07f;
    private void Awake()
    {
        if (Instance == null) Instance = this;
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
        if (data.mouseSens > 0)
        { 
            defaultMouseSens = data.mouseSens;
            if (InputManager.Instance) InputManager.Instance.mouseSensitivity = data.mouseSens;
        }
        Screen.fullScreen = data.isFullscreen;
        if (data.resWidth > 0 && data.resHeight > 0) Screen.SetResolution(data.resWidth, data.resHeight, Screen.fullScreen);
        QualitySettings.SetQualityLevel(data.qualIndex);
        
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
    
}

