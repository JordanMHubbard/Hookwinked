using System.Collections;
using UnityEngine;

public class SharkSpawner : AIFishSpawner
{
    [SerializeField] private float spawnDelay = 15f;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnSharks());
    }

    private IEnumerator SpawnSharks()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnFish(numFish, false);
    }
}
