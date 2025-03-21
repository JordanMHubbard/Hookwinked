using System.Collections;
using UnityEngine;

public class PredatorEvasion : MonoBehaviour
{
    [SerializeField] private float fasterSwimSpeed = 6f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float speedUpCooldown = 3f;
    private bool isSpeedUpOnCooldown;
    private bool isPlayerNearby;
    private AIFishController controller;

    // MAYBE ONLY DO PREDATOR EVASION ON REAL PREY??
    private void Start()
    {
        controller = GetComponentInParent<AIFishController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // May have to adjust trigger size
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (isSpeedUpOnCooldown) return;
            
            SwimAway();
        }
    }

    private void OnTriggerStay()
    {
        if (isPlayerNearby && !isSpeedUpOnCooldown)
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

    // Immediately boosts prey and slows down over time
    private IEnumerator SpeedUp()
    {   
        isSpeedUpOnCooldown = true;
        controller.SetSwimSpeed(fasterSwimSpeed);
        
        float elapsedTime = 0f;
        float currentSpeed;
        float targetSpeed = controller.GetDefaultSwimSpeed();

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            currentSpeed = Mathf.Lerp(fasterSwimSpeed, targetSpeed, t);
            controller.SetSwimSpeed(currentSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //yield return new WaitForSeconds(duration);
        
        controller.SetSwimSpeed(targetSpeed);
        yield return new WaitForSeconds(speedUpCooldown);

        isSpeedUpOnCooldown = false;
    }

    // Finds new target location and speeds up temporarily
    private void SwimAway()
    {
        controller.FindTarget(); 
        StartCoroutine(SpeedUp()); 
    }
}
