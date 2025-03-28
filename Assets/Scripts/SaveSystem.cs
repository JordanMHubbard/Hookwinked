using UnityEngine;
using System.IO;

public class SaveSystem 
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]    
    public struct SaveData
    {
        public GameSaveData GameData;
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

    private static void HandleSaveData()
    {
        GameManager.Instance.Save(ref saveData.GameData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        if (saveContent == null) return;

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GameManager.Instance.Load(saveData.GameData);
    }

    public static void ResetDays()
    {
        GameManager.Instance.SetCurrentDay(0);
        Save();
    }

}
