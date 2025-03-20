using System.Collections;
using UnityEngine;

public class PreyController : AIFishController
{
    /*[SerializeField] private float speedUpCooldown = 5f;
    private bool isSpeedUpOnCooldown;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // May have to adjust trigger size
            FindTarget(); // Find new target location
            StartCoroutine(SpeedUp());
        }
    }

    private IEnumerator SpeedUp()
    {   
        //change speed
        yield return new WaitForSeconds(2f);
        
        isSpeedUpOnCooldown = true;
        yield return new WaitForSeconds(speedUpCooldown);
        isSpeedUpOnCooldown = false;
    }*/
}
