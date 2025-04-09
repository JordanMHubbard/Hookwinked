using UnityEngine;

[System.Serializable]
public class PerkInfo
{
    public string perkName;
    public string description;
    public bool isUnlocked;
    public int fragmentCost;
    public Sprite icon;

    public PerkInfo(string perkName, string description, int fragmentCost, bool isUnlocked = false, Sprite icon = null)
    {
        this.perkName = perkName;
        this.description = description;
        this.fragmentCost = fragmentCost;
        this.isUnlocked = isUnlocked;
        this.icon = icon;
    }

}
