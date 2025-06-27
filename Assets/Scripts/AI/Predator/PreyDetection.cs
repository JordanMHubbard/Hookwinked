using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PreyDetection : MonoBehaviour
{
    private List<GameObject> nearbyPrey;
    protected GameObject closestPrey;
    protected bool isPursuingPrey;
    protected bool canHunt;
    protected PredatorController controller;
    public void SetIsPursuingPrey(bool isPursuing) { isPursuingPrey = isPursuing; }
    
    protected virtual void Start()
    {
        controller = GetComponentInParent<PredatorController>();
        if (controller == null)
        {
            Debug.LogError("ObstacleAvoidance: No AIFishController found on " + gameObject.name);
        }

        nearbyPrey = new List<GameObject>();
        StartCoroutine(StartHunt());
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            AddPrey(other.gameObject);
            if (closestPrey == null) FindClosestPrey();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            RemovePrey(other.gameObject);
        }
    }

    protected void FindClosestPrey()
    {
        if (isPursuingPrey || !canHunt) return;

        float closestDistance = 50f;

        foreach (GameObject prey in nearbyPrey)
        {
            float dist = Vector3.Distance(transform.position, prey.transform.position);
            if (dist < closestDistance) 
            {
                closestDistance = dist;
                closestPrey = prey;
            }
        }

        if (closestPrey != null) 
        {
            //Debug.Log("set prey target");
            isPursuingPrey = true;
            StartPursuit();
        }
    }

    protected void StartPursuit()
    {
        controller.SetfoundPrey(true);
        StartCoroutine(ApproachClosestPrey());
        StartCoroutine(controller.SpeedUp());
    }

    private IEnumerator ApproachClosestPrey()
    {
        float dist = 50f;
        Vector3 offset;

        while (dist > 1f && isPursuingPrey && closestPrey)
        {
            offset = (closestPrey.transform.position - transform.position).normalized * 3f;
            controller.SetTargetPosition(closestPrey.transform.position + offset);
            yield return null;
        }

        controller.FindTarget();
        closestPrey = null;
        isPursuingPrey = false;
    }

    private void AddPrey(GameObject prey)
    {
        if (nearbyPrey == null) return;
        if (!nearbyPrey.Contains(prey)) nearbyPrey.Add(prey);
    }

    public void RemovePrey(GameObject prey)
    {
        if (nearbyPrey == null) return;
        if (nearbyPrey.Contains(prey)) nearbyPrey.Remove(prey);
        if (nearbyPrey.Count == 0) controller.SetfoundPrey(false);
    }

    private IEnumerator StartHunt()
    {
        yield return new WaitForSeconds(2f);
        canHunt = true;
    }
}
