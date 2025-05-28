using System.Collections;
using UnityEngine;

public class PredatorEvasion : MonoBehaviour
{
    private bool isPlayerNearby;
    private AIFishController controller;

    // MAYBE ONLY DO PREDATOR EVASION ON REAL PREY??
    private void Start()
    {
        controller = GetComponentInParent<AIFishController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controller == null) return;
        // May have to adjust trigger size
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (controller.GetIsSpeedUpOnCooldown()) return;
            
            SwimAway();
        }
    }

    private void OnTriggerStay()
    {   
        if (controller == null) return;

        if (isPlayerNearby && !controller.GetIsSpeedUpOnCooldown())
        {
            SwimAway();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    // Finds new target location and speeds up temporarily
    private void SwimAway()
    {
        //controller.FindTarget(); 
        StartCoroutine(controller.SpeedUp()); 
    }
}
