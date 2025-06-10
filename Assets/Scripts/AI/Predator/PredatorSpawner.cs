using UnityEngine;

public class PredatorSpawner : AIFishSpawner
{
    private void Awake()
    {
        int predatorCount = GameManager.Instance.GetCurrentDaySettings().predatorCount;
        numFish = predatorCount;
    }
}
