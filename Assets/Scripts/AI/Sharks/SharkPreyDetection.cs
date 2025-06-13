using UnityEngine;

public class SharkPreyDetection : PreyDetection
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            closestPrey = other.gameObject;
            isPursuingPrey = true;
            if (closestPrey != null) StartPursuit();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            closestPrey = null;
            FindClosestPrey();
        }
    }
}
