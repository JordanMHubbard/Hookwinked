using System.Collections;
using UnityEngine;

public class PreySpawner : AIFishSpawner
{
    private void OnEnable()
    {
        GameManager.Instance.OnPreyConsumed += SpawnNewPrey;
    }

    private void RespawnPrey(GameObject prey)
    {
        if (fishPrefab == null) 
        {
            Debug.LogError("preyPrefab has not been set!");
            return;
        }

        Vector3 spawnPosition = GetRandomPoint();
        prey.transform.position = spawnPosition;
        prey.SetActive(true);
        
    }


    IEnumerator SpawnPreyDelayed(GameObject prey)
    {
        float waitTime = Random.Range(3f, 10f);
        yield return new WaitForSeconds(waitTime);

        RespawnPrey(prey);
    }

    private void SpawnNewPrey(GameObject prey)
    {
        //Debug.Log("We about to spawn new prey.");
        StartCoroutine(SpawnPreyDelayed(prey));
    }
}
