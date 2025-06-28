using System.Collections;
using UnityEngine;

public class GrenadeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float paddingXZ = 20f;
    [SerializeField] private int spawnDelay = 20;
    [SerializeField] private int minSpawnCooldown = 6;
    [SerializeField] private int maxSpawnCooldown = 14;
    [SerializeField] private LayerMask interactableLayer;
    private BoxCollider box;

    private void Awake()
    {
        isEnabled = GameManager.Instance.GetCurrentDaySettings().enableGrenades;
    }

    private void Start()
    {
        if (!isEnabled) return;

        box = GetComponent<BoxCollider>();

        if (box == null)
        {
            Debug.LogWarning($"{gameObject.name}: box has not been set!");
            return;
        }

        StartCoroutine(StartSpawningGrenades());
    }

    private Vector3 GetRandomPoint()
    {
        bool isReachable = false;
        Vector3 point = Vector3.zero;
        int MaxAttempts = 10;
        int attempts = 0;

        while (!isReachable && attempts < MaxAttempts)
        {
            Vector3 min = box.bounds.min;
            Vector3 max = box.bounds.max;

            float x = Random.Range(min.x + paddingXZ, max.x - paddingXZ);
            float y = transform.position.y;
            float z = Random.Range(min.z + paddingXZ, max.z - paddingXZ);

            point = new Vector3(x, y, z);
            isReachable = IsAreaReachable(point, new Vector3(2, 2, 2));
            attempts++;
        }

        return point;
    }

    private bool IsAreaReachable(Vector3 checkPosition, Vector3 boxSize)
    {
        bool isInSpawnableArea = false;

        Collider[] colliders = Physics.OverlapBox(checkPosition, boxSize * 0.5f, Quaternion.identity, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle") || col.CompareTag("Boat")) return false;

            if (col.CompareTag("GrenadeSpawnArea")) isInSpawnableArea = true;
        }

        return isInSpawnableArea;
    }

    private void SpawnGrenade(Vector3 spawnPosition)
    {
        if (grenadePrefab == null)
        {
            Debug.LogError("grenadePrefab has not been set!");
            return;
        }

        GameObject grenadeInstance = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator StartSpawningGrenades()
    {
        yield return new WaitForSeconds(spawnDelay);

        while (isEnabled && GameManager.Instance.GetActiveBoatsCount() > 0)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnCooldown, maxSpawnCooldown));
            CalculateSpawnPos();
        }
    }

    private void CalculateSpawnPos()
    {
        Vector3 playerPos = GameManager.Instance.PlayerController.transform.position;
        Vector3 playerForward = GameManager.Instance.PlayerController.GetPlayerCam().transform.forward;
        Vector3 desiredLoc = playerPos + playerForward * 10f;
        desiredLoc.y = transform.position.y;

        bool isReachable = IsAreaReachable(desiredLoc, new Vector3(2, 2, 2));
        if (isReachable) SpawnGrenade(desiredLoc);
    }
    

}
