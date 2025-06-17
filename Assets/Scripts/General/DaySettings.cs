using UnityEngine;

[System.Serializable]
public class DaySettings
{
    public string Day;
    public float daySpeed = 1f;
    public int preyCount = 20;
    public int predatorCount = 10;
    public int boatCount = 5;
    public int baitMin = 1;
    public int baitMax = 3;
    public bool enableSharks = false;
    public int sharkCount = 1;
    public bool enableGrenades = false;

    public int GetBaitCount()
    {
        return Random.Range(baitMin, baitMax);
    }
}
