using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PreyDetection : MonoBehaviour
{
    private List<GameObject> nearbyPrey;
    private GameObject closestPrey;
    private bool isPursuingPrey;
    private bool canHunt;
    private PredatorController controller;
    [SerializeField] private bool isShark;

    public void SetIsPursuingPrey(bool isPursuing) { isPursuingPrey = isPursuing; }
    

    private void Start()
    {
        if (!isShark) { controller = GetComponentInParent<PredatorController>();}
        else controller = GetComponentInParent<SharkController>();
        
        if (controller == null)
            {
                Debug.LogError("ObstacleAvoidance: No AIFishController found on " + gameObject.name);
            }
        
        nearbyPrey = new List<GameObject>();
        StartCoroutine(StartHunt());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey") || other.CompareTag("Player"))
        {
            AddPrey(other.gameObject);
            if (closestPrey == null) FindClosestPrey();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Prey") || other.CompareTag("Player"))
        {
            RemovePrey(other.gameObject);
        }
    }

    private void FindClosestPrey()
    {
        if (isPursuingPrey || !canHunt) return;
        isPursuingPrey = true;

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
            controller.SetfoundPrey(true);
            StartCoroutine(ApproachClosestPrey());
            StartCoroutine(controller.SpeedUp());
        }
    }
    
    private IEnumerator ApproachClosestPrey()
    {
        float dist = 50f;
        Vector3 offset;

        while (dist > 0.5f && isPursuingPrey)
        {
            offset = (closestPrey.transform.position - transform.position).normalized * 2f;
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
