using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    [SerializeField] private GameObject boatParent;
    [SerializeField] private GameObject damageDecal;
    [SerializeField] private GameObject boatFragment;
    private int timesDamaged;
    private bool canSpawnFish;
    private Vector3 currentTarget;
    private Vector3 directionToTarget;

    // Movement
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float turnSpeed = 1f;
    private float currentSpeed = 0.1f;
    float accelerationRate = 0.5f;
    private bool hasTarget;
    private bool shouldRotate;

    // Baited Fish 
    private List<GameObject> BaitedPrey = new List<GameObject>();

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.GetPreySpawner() != null)
            {
                canSpawnFish = true;
            }
        }

        if (boatParent == null)
        {
            Debug.LogWarning("boatparent has not been set!");
        }

        // Spawn random number of bait
        BaitedPrey = GameManager.Instance.GetPreySpawner().SpawnFish(Random.Range(1,3), true);
        foreach (GameObject fish in BaitedPrey)
        {
           Debug.Log("Just spawned some bait yay!");
        }
    }

    private void Update()
    {
        if (shouldRotate) Rotate();
        if (hasTarget) MoveTowardsTarget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.CompareTag("Interactable"))
        { 
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 contactNormal = collision.GetContact(0).normal;
            ApplyRockDamage(contactPoint, contactNormal, collision.gameObject);
        }
    }

    public void ApplyRockDamage(Vector3 point, Vector3 normal, GameObject rock)
    {
        // Show damage decal 
        Quaternion rotation = Quaternion.LookRotation(normal);
        GameObject decalInstance = Instantiate(damageDecal, point + normal * -0.1f, rotation, boatParent.transform);
        
        // Destroy rock
        //rock.SetActive(false);
        
        // Drop fragment
        DropFragment(decalInstance.transform.position);

        // Apply damage
        Debug.Log("We've been hit");
        if (timesDamaged++ >= 2)
        {
            Flee(); 

            // Spawn two new prey
            if (canSpawnFish)
            {
                Debug.Log("We changing bait to prey yeupppp"); 
                foreach (GameObject fish in BaitedPrey)
                {
                   PreyController controller = fish.GetComponent<PreyController>();
                   if (controller != null) 
                   {
                        controller.SetIsBait(false);
                        controller.SetBaitStatus();
                        GameManager.Instance.PreyConsumed(controller.gameObject);
                   }
                }
            }
        } 

        /*Things that need to be done:
            1. Increase damage counter +1 ✔
            2. Show damaged decal on boat ✔
            3. Check if has been damaged 3 times ✔
                - if true, make boat drive off ✔
                - remove bait from pool ✔
                - Play boat driving sound
            4. Destroy rock on impact 
            5.
            6. Show slight rock explosion effect
        */
    }

    private void DropFragment(Vector3 position)
    {
        if (boatFragment != null)
        {
            GameObject fragment = Instantiate(boatFragment, position, Quaternion.identity);
        }
    }

    private void Flee()
    {
        Debug.Log("We need to get outta here cap'n!");
        currentTarget = GameManager.Instance.GetRandomBoatWaypoint();
        
        StartRotate();
        MoveTowardsTarget();
    }

    private void StartRotate()
    {
        directionToTarget = (currentTarget - boatParent.transform.position).normalized;
        turnSpeed = Random.Range(0.5f, 1.5f);
        shouldRotate = true;
        hasTarget = true;
    }

    private void Rotate()
    {
        boatParent.transform.rotation = Quaternion.Slerp(boatParent.transform.rotation, Quaternion.LookRotation(directionToTarget), 
            turnSpeed * Time.deltaTime); 
        
        if (Quaternion.Angle(boatParent.transform.rotation, Quaternion.LookRotation(directionToTarget)) < 0.5f)
        {
            shouldRotate = false;
        }
    }

    private void MoveTowardsTarget()
    {   
        if (currentSpeed < maxSpeed) currentSpeed *= 1f + (accelerationRate * Time.deltaTime);
        boatParent.transform.position += directionToTarget * currentSpeed * Time.deltaTime;

        float dist = Vector3.Distance(boatParent.transform.position, currentTarget);
        if (dist < 1f) hasTarget = false;
    }
}
