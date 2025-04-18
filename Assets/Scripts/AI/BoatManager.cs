using UnityEngine;

public class BoatManager : MonoBehaviour
{
    [SerializeField] private GameObject damageDecal;
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

    public void ApplyRockDamage(Vector3 point, Vector3 normal)
    {
        // Show damage decal 
        GameObject decalInstance = Instantiate(damageDecal, point + normal * 0.5f, Quaternion.identity);

        // Apply damage
        Debug.Log("We've been hit, it's goin down, im yelling timber!");
        if (timesDamaged++ >= 3)
        {
            Debug.Log("We need to get outta here cap'n!");
        }

        // Spawn two new prey
        if (canSpawnFish)
        {
            Debug.Log("We spawning fish boiiii");
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
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 contactNormal = collision.GetContact(0).normal;
            ApplyRockDamage(contactPoint, contactNormal);
        }
    }
}
