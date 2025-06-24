using UnityEngine;
using System.IO;

public class SaveSystem 
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public GameSaveData GameData;
        public OptionsSaveData OptionsData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    public static void SaveOptions()
    {
        if (OptionsManager.Instance != null) OptionsManager.Instance.Save(ref saveData.OptionsData);
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        if (GameManager.Instance != null) GameManager.Instance.Save(ref saveData.GameData);
    }

    public static void Load()
    {
        if (!File.Exists(SaveFileName())) return;
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    public static void LoadOptions()
    {
        if (!File.Exists(SaveFileName()))
        { 
            if (OptionsManager.Instance != null) OptionsManager.Instance.Load(OptionsSaveData.GetDefault());
            return;
        }
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        if (OptionsManager.Instance != null) OptionsManager.Instance.Load(saveData.OptionsData);
    }

    private static void HandleLoadData()
    {
        if (GameManager.Instance != null) GameManager.Instance.Load(saveData.GameData);
    }

    public static void ResetSaveData()
    {
        if (File.Exists(SaveFileName()))
        {
            File.Delete(SaveFileName());
            Debug.Log("Save file deleted.");
        }
    }

}
