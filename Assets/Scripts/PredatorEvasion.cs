using System.Collections;
using UnityEngine;

public class PredatorEvasion : MonoBehaviour
{
    [SerializeField] private float fasterSwimSpeed = 6f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float speedUpCooldown = 3f;
    private bool isSpeedUpOnCooldown;
    private AIFishController controller;

    // MAYBE ONLY DO PREDATOR EVASION ON REAL PREY??
    private void Start()
    {
        controller = GetComponentInParent<AIFishController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isSpeedUpOnCooldown) return;
            
            // May have to adjust trigger size
            // Finds new target location and speeds up temporarily
            controller.FindTarget(); 
            StartCoroutine(SpeedUp()); 
        }
    }

    private IEnumerator SpeedUp()
    {   
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
        isSpeedUpOnCooldown = true;
        yield return new WaitForSeconds(speedUpCooldown);

        isSpeedUpOnCooldown = false;
    }
}
