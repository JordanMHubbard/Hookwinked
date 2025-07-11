using System.Collections;
using UnityEngine;

public class SharkSpawner : AIFishSpawner
{
    [SerializeField] private float spawnDelay = 15f;

    protected override void Start()
    {
        isEnabled = GameManager.Instance.GetCurrentDaySettings().enableSharks;
        base.Start();

        if (!isEnabled) return;
        Debug.Log("Sharks are enabled");
        numFish = GameManager.Instance.GetCurrentDaySettings().sharkCount;
        StartCoroutine(SpawnSharks());
    }

    private IEnumerator SpawnSharks()
    {
        RandomizeSpawnDelay();
        yield return new WaitForSeconds(spawnDelay);

        SpawnFish(numFish);
    }

    private void RandomizeSpawnDelay()
    {
        spawnDelay += Random.Range(0, 16);
        Debug.Log("New spawn delay: " + spawnDelay);
    }
}
