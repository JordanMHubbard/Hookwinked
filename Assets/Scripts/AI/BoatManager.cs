using UnityEngine;

public class BoatManager : MonoBehaviour
{
    private int timesDamaged;
    private bool canSpawnFish;

    private void Awake()
    {
        if (GameManager.Instance != null)
            {
                if (GameManager.Instance.GetPreySpawner() != null)
                {
                    canSpawnFish = true;
                }
            }
    }

    public void ApplyRockDamage()
    {
        Debug.Log("We've been hit, it's goin down, im yelling timber!");
        if (timesDamaged++ >= 3)
        {
            Debug.Log("We need to get outta here cap'n!");
        }
            
        if (canSpawnFish)
        {
            GameManager.Instance.GetPreySpawner().SpawnFish();
            GameManager.Instance.GetPreySpawner().SpawnFish();
        }

        /*Things that need to be done:
            1. Increase damage counter +1
            2. Show damaged decal on boat
            3. Check if has been damaged 3 times
                - if true, make boat drive off?
                - remove add 2 more prey to prey pool 
            4. Destroy rock on impact
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.CompareTag("Interactable"))
        {
            ApplyRockDamage();
        }
    }
}
