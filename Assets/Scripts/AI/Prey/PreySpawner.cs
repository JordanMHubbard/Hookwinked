using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : AIFishSpawner
{
    [SerializeField] private List<GameObject> baitMeshes;
    private void Awake()
    {
        int preyCount = GameManager.Instance.GetCurrentDaySettings().preyCount;
        int adjustedPreyCount = preyCount + GameManager.Instance.GetNumDayRetries() * 2;
        if (adjustedPreyCount > preyCount && adjustedPreyCount < (1.5 * preyCount))
        {
            Debug.Log("og prey count: " + preyCount + ", new prey count:" + adjustedPreyCount);
            numFish = adjustedPreyCount;
        }
        else
        {
            Debug.Log(" no prey count change");
            numFish = preyCount;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPreyConsumed += SpawnNewPrey;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPreyConsumed -= SpawnNewPrey;
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

    public List<GameObject> SpawnBait(int fishCount)
    {
        List<GameObject> fish = new List<GameObject>();

        if (fishPrefab == null)
        {
            Debug.LogError("fishPrefab has not been set!");
            return null;
        }

        if (fishCount <= 0) fishCount = numFish;

        while (fishCount > 0)
        {
            Vector3 spawnPosition = useSpawnLocations ? GetRandomSpawnLoc() : GetRandomPoint();
            GameObject fishInstance = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);

            Transform meshHolder = fishInstance.transform.Find("FishMesh");
            if (meshHolder == null)
            {
                Debug.LogError("FishMesh child not found on FishNPC prefab!");
                return null;
            }

            GameObject meshInstance;

            int choice = Random.Range(0, 4);
            if (choice > 0)
            {
                meshInstance = Instantiate(GetRandomMesh(fishMeshes), meshHolder);
                PreyController controller = fishInstance.GetComponent<PreyController>();
                if (controller != null) controller.SetIsBait(true, false);
            }
            else
            {
                meshInstance = Instantiate(GetRandomMesh(baitMeshes), meshHolder);
                PreyController controller = fishInstance.GetComponent<PreyController>();
                if (controller != null)
                {
                    controller.SetIsBait(true, true);
                    Debug.Log("Using Bait Mesh");
                }
            }
        
            // Play animation
            Animator animator = meshHolder.GetComponent<Animator>();
            animator.Rebind();

            meshInstance.transform.localPosition = Vector3.zero;
            meshInstance.transform.localRotation = Quaternion.identity;

            fish.Add(fishInstance);
            fishCount--;
        }

        return fish;
    }
}
