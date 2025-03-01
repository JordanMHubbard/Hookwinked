using System.Collections.Generic;
using UnityEngine;

public class PreyDetection : MonoBehaviour
{
    private List<GameObject> nearbyPrey;
    private AIFishController controller;
    void Start()
    {
        
        controller = GetComponentInParent<AIFishController>();
        if (controller == null )
        {
            Debug.LogError("ObstacleAvoidance: No AIFishController found on " + gameObject.name);
        }
        
        nearbyPrey = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            nearbyPrey.Add(other.gameObject);
            ApproachClosestPrey();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            nearbyPrey.Remove(other.gameObject);
            if (nearbyPrey.Count == 0) controller.SetfoundPrey(false);
        }
    }

    void ApproachClosestPrey()
    {
        float closestDistance = 50f;
        GameObject closestPrey = null;

        foreach (GameObject prey in nearbyPrey)
        {
            float dist = Vector3.Distance(transform.position, prey.transform.position);
            if (dist < closestDistance) 
            {
                closestDistance = dist;
                closestPrey = prey;
            }
        }

        if (closestPrey != null ) 
        {
            controller.SetfoundPrey(true);
            controller.SetTargetPosition(closestPrey.transform.position);
        }
    }
}
