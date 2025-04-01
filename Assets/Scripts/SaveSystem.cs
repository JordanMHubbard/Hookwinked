using UnityEngine;
using System.IO;

public class SaveSystem 
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]    
    public struct SaveData
    {
        public GameSaveData GameData;
        public PerkSaveData PerkData;
        public PlayerSaveData PlayerData;
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
        if (GameManager.Instance != null) GameManager.Instance.Save(ref saveData.GameData);
        if (GameManager.Instance.Player != null) GameManager.Instance.Player.Save(ref saveData.PlayerData);
        if (GameManager.Instance.PerkUpgrades != null) GameManager.Instance.PerkUpgrades.Save(ref saveData.PerkData);
    }

    public static void Load()
    {
        if (!File.Exists(SaveFileName())) return;
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        if (GameManager.Instance != null) GameManager.Instance.Load(saveData.GameData);
        if (GameManager.Instance.Player != null) GameManager.Instance.Player.Load(saveData.PlayerData);
        if (GameManager.Instance.PerkUpgrades != null) GameManager.Instance.PerkUpgrades.Load(saveData.PerkData);
    }

    public static void ResetDays()
    {
        GameManager.Instance.SetCurrentDay(0);
        Save();
    }

}
