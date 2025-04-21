using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    [SerializeField] private GameObject boatParent;
    [SerializeField] private GameObject damageDecal;
    private int timesDamaged;
    private bool canSpawnFish;
    private Vector3 currentTarget;
    private Vector3 directionToTarget;
    [SerializeField] private float maxSpeed = 4f;
    private float currentSpeed = 0.1f;
    float accelerationRate = 0.5f;
    private bool hasTarget;
    private bool shouldRotate;
    [SerializeField] private float turnSpeed = 1f;

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


        // Apply damage
        Debug.Log("We've been hit");
        if (timesDamaged++ >= 2)
        {
            Flee(); 

            // Spawn two new prey
            if (canSpawnFish)
            {
                Debug.Log("We spawning fish boiiii");
                GameManager.Instance.GetPreySpawner().SpawnFish(2);
            }
        } 

        /*Things that need to be done:
            1. Increase damage counter +1 ✔
            2. Show damaged decal on boat ✔
            3. Check if has been damaged 3 times ✔
                - if true, make boat drive off ✔
                - remove add 2 more prey to prey pool ✔
            4. Destroy rock on impact 
            5. Show slight rock explosion effect
            6. Play boat driving sound
        */
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
