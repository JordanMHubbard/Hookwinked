using System.Collections.Generic;
using UnityEngine;

public class BoatSpawner : MonoBehaviour
{
   
    [SerializeField] protected GameObject boatPrefab;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private int numBoats = 5;
    [SerializeField] private float paddingXZ = 20f;
    [SerializeField] private List<GameObject> boatMeshes;
    [SerializeField] private LayerMask interactableLayer;
    private BoxCollider box;

    private void Awake()
    {
        numBoats = GameManager.Instance.GetCurrentDaySettings().boatCount;
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
        
        SpawnBoat();
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
            float y = transform.position.y;
            float z = Random.Range(min.z + paddingXZ, max.z - paddingXZ);

            point = new Vector3(x,y,z);
            isReachable = IsAreaReachable(point, new Vector3(15, 5, 15));
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
            if (col.CompareTag("Obstacle") || col.CompareTag("Boat") ) return false;

            if (col.CompareTag("BoatSpawnArea")) isInSpawnableArea = true;
        }

        return isInSpawnableArea;
    }

    private Quaternion GetRandomRotation()
    {
        int yRot = Random.Range(0, 360);
        Vector3 newRotation = new Vector3(0f, yRot, 0f);
        
        return Quaternion.Euler(newRotation);
    }

    private void SpawnBoat()
    {
        if (boatPrefab == null) 
        {
            Debug.LogError("boatPrefab has not been set!");
            return;
        }

        GameManager.Instance.SetBoatCount(numBoats);
        
        while (numBoats > 0)
        {
            Vector3 spawnPosition = GetRandomPoint();
            GameObject boatInstance = Instantiate(boatPrefab, spawnPosition, GetRandomRotation());

            Transform meshHolder = boatInstance.transform.Find("BoatMesh");
            if (meshHolder == null)
            {
                Debug.LogError("boatMesh child not found on boat prefab!");
                return;
            }

            GameObject meshInstance = Instantiate(GetRandomMesh(), meshHolder);
            meshInstance.transform.localPosition = Vector3.zero;
            meshInstance.transform.localRotation = Quaternion.identity;

            numBoats--;
        }
    }

    private GameObject GetRandomMesh()
    {
        int total = boatMeshes.Count;
        int index = Random.Range(0, total);
    
        return boatMeshes[index];
    }

}
