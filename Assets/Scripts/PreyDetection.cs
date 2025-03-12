using System.Collections.Generic;
using UnityEngine;

public class PreyDetection : MonoBehaviour
{
    private List<GameObject> nearbyPrey;
    private AIFishController controller;
    private void Start()
    {
        
        controller = GetComponentInParent<AIFishController>();
        if (controller == null )
        {
            Debug.LogError("ObstacleAvoidance: No AIFishController found on " + gameObject.name);
        }
        
        nearbyPrey = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Prey"))
        {
            if (other.gameObject == null) return;
            nearbyPrey.Add(other.gameObject);
            ApproachClosestPrey();
        }
    }

    private void ApproachClosestPrey()
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
            //Debug.Log("set prey target");
            controller.SetfoundPrey(true);
            Vector3 offset = (closestPrey.transform.position - transform.position).normalized * 2f;
            controller.SetTargetPosition(closestPrey.transform.position + offset);
        }
    }

    public void RemovePrey(GameObject prey)
    {
        if (nearbyPrey.Contains(prey)) nearbyPrey.Remove(prey);
        if (nearbyPrey.Count == 0) controller.SetfoundPrey(false);
    }
}
