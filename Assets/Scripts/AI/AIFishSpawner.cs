using System.Collections.Generic;
using UnityEngine;

public class AIFishSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject fishPrefab;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool useSpawnLocations = false;
    [SerializeField] private List<GameObject> spawnLocations;
    [SerializeField] protected int numFish = 20;
    [SerializeField] private float paddingXZ = 20f;
    [SerializeField] private float paddingY = 5f;
    [SerializeField] private List<GameObject> fishMeshes;
    [SerializeField] private LayerMask interactableLayer;
    private BoxCollider box;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (!isEnabled) return;

        box = GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogWarning($"{gameObject.name}: box has not been set!");
            return;
        }

        if (spawnOnStart) SpawnFish(0, false);
    }

    public Vector3 GetRandomPoint()
    {
        bool isReachable = false;
        Vector3 point = GetRandomSpawnLoc();
        int MaxAttempts = 10;
        int attempts = 0;

        while (!isReachable && attempts < MaxAttempts)
        {
            Vector3 min = box.bounds.min;
            Vector3 max = box.bounds.max;

            float x = Random.Range(min.x + paddingXZ, max.x - paddingXZ);
            float y = Random.Range(min.y + paddingY, max.y - paddingY);
            float z = Random.Range(min.z + paddingXZ, max.z - paddingXZ);

            point = new Vector3(x, y, z);
            isReachable = IsAreaReachable(point, new Vector3(2, 2, 2));
            attempts++;
        }

        return point;
    }

    private Vector3 GetRandomSpawnLoc()
    {
        if (spawnLocations.Count > 0)
        {
            return spawnLocations[Random.Range(0, spawnLocations.Count)].transform.position;
        }
        return Vector3.zero;
    }

    bool IsAreaReachable(Vector3 checkPosition, Vector3 boxSize)
    {
        bool isInReachableArea = false;

        Collider[] colliders = Physics.OverlapBox(checkPosition, boxSize * 0.5f, Quaternion.identity, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle") || col.CompareTag("Fish")) return false;

            if (col.CompareTag("ReachableArea")) isInReachableArea = true;
        }

        return isInReachableArea;
    }

    public List<GameObject> SpawnFish(int fishCount, bool isBait)
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

            GameObject meshInstance = Instantiate(GetRandomMesh(), meshHolder);

            // Play animation
            Animator animator = meshHolder.GetComponent<Animator>();
            animator.Rebind();

            meshInstance.transform.localPosition = Vector3.zero;
            meshInstance.transform.localRotation = Quaternion.identity;

            if (isBait) InitializeBait(fishInstance);

            fish.Add(fishInstance);
            fishCount--;
        }

        return fish;
    }

    private GameObject GetRandomMesh()
    {
        int total = fishMeshes.Count;
        int index = Random.Range(0, total);

        return fishMeshes[index];
    }

    private void InitializeBait(GameObject obj)
    {
        PreyController controller = obj.GetComponent<PreyController>();
        if (controller != null) controller.SetIsBait(true);
    }
}
