[System.Serializable]
public class PerkInfo
{
    public string perkName;
    public string description;
    public bool isUnlocked;
    public int fragmentCost;

    public PerkInfo(string perkName, string description, int fragmentCost, bool isUnlocked = false)
    {
        this.perkName = perkName;
        this.description = description;
        this.fragmentCost = fragmentCost;
        this.isUnlocked = isUnlocked;
    }

}
