using System.Collections;
using UnityEngine;

public class OldPreySpawner : MonoBehaviour
{
    [SerializeField] private GameObject preyPrefab;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private int numPrey = 40;
    [SerializeField] private float paddingXZ = 20f;
    [SerializeField] private float paddingY = 1f;
    [SerializeField] private LayerMask interactableLayer;
    private BoxCollider box;

    private void OnEnable()
    {
        GameManager.Instance.OnPreyConsumed += OldSpawnNewPrey;
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

        InitialSpawnPrey();
    }

    private Vector3 GetRandomPoint()
    {
        bool isReachable = false;
        Vector3 point = Vector3.zero;
        int MaxAttempts = 20;
        int attempts = 0;

        while (!isReachable && attempts < MaxAttempts)
        {
            Vector3 min = box.bounds.min;
            Vector3 max = box.bounds.max;

            float x = Random.Range(min.x + paddingXZ, max.x - paddingXZ);
            float y = Random.Range(min.y + paddingY, max.y - paddingY);
            float z = Random.Range(min.z + paddingXZ, max.z - paddingXZ);

            point = new Vector3(x,y,z);
            isReachable = IsAreaReachable(point, new Vector3(2, 2, 2));
            attempts++;
        }

        return point;
    }

    private bool IsAreaReachable(Vector3 checkPosition, Vector3 boxSize)
    {
        bool isInReachableArea = false;

        Collider[] colliders = Physics.OverlapBox(checkPosition, boxSize * 0.5f, Quaternion.identity, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle")) return false;

            if (col.CompareTag("ReachableArea")) isInReachableArea = true;
        }

        return isInReachableArea;
    }

    private void InitialSpawnPrey()
    {
        float count = numPrey;
        while (count > 0)
        {
            Vector3 spawnPosition = GetRandomPoint();
            GameObject preyInstance = Instantiate(preyPrefab, spawnPosition, Quaternion.identity); 
            count--;
        }
    }

    private void RespawnPrey(GameObject prey)
    {
        if (preyPrefab == null) 
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

    private void OldSpawnNewPrey(GameObject prey)
    {
        //Debug.Log("We about to spawn new prey.");
        StartCoroutine(SpawnPreyDelayed(prey));
    }
}
