using UnityEngine;

[System.Serializable]
public class DaySettings
{
    public string Day;
    public int preyCount = 20;
    public int predatorCount = 10;
    public int boatCount = 5;
    public int baitMin = 1;
    public int baitMax = 3;

    public int GetBaitCount()
    {
        return Random.Range(baitMin, baitMax);
    }
}
