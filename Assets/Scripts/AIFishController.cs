
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class AIFishController : MonoBehaviour
{
    private bool hasTarget;
    private bool iTurning;
    private bool foundPrey;
    private bool foundObstacle;
    private List<string> currentObstacles;
    private Vector3 repelDirection;
    private Vector3 targetLocation;
    private Vector3 currentVelocity;
    [SerializeField] private float swimSpeed = 3f;
    private float viewDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;

    private CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (characterController == null)
        {
            Debug.LogWarning($"{gameObject.name}: characterController has not been set!");
            enabled = false;
        }

        currentObstacles = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.DrawLine(transform.position, transform.position + transform.forward * 2f, Color.blue);

        if (!hasTarget)
        {
            hasTarget = FindTarget();
        }
        else if (hasTarget) 
        {
            Rotate();
            MoveTowardsTarget();
        }
    }

    private bool FindTarget()
    {
        if (foundPrey)
        {
            // targetLocation = Prey.location;
        }
        else 
        {
            // Later need to consider 1.unreachable points, 2.only getting points within a defined space 
            Vector3 locationOffset = new Vector3 (Random.Range(-5,5), 0, Random.Range(-5,5));
            targetLocation = transform.position + locationOffset;
            Debug.Log("targetLocation = " + targetLocation);
            hasTarget = true;
        }
        
        return true;
    }

    private void Rotate()
    {   
        float turnSpeed = swimSpeed * Random.Range(1f, 2f);
        Vector3 lookAt;

        if (foundObstacle)
        {
            lookAt = repelDirection;
        }
        else
        {
           lookAt = targetLocation - transform.position;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAt), turnSpeed * Time.deltaTime);
    }

    private void MoveTowardsTarget()
    {
        characterController.Move(transform.forward * swimSpeed * Time.deltaTime);
        
        float dist = Vector3.Distance(transform.position, targetLocation);
        
        if (dist < 1f)
        {
            if (foundPrey) //this will get unset once fish collides with prey
            {   
                targetLocation += transform.forward * 5f;
                return;
            }

            hasTarget = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == gameObject)
        {
            return;
        }
        
        if (other.CompareTag("Obstacle"))
        {
            AvoidObstacle(other.gameObject);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        removeObstacle(other);
    }

    private void AvoidObstacle(GameObject obstacle)
    {
        if (obstacle == null) return;

        Vector3 obstaclePosition = obstacle.transform.position;
        Vector3 directionToObstacle = obstaclePosition - transform.position; 

        Ray ray = new Ray(transform.position, directionToObstacle);
        Debug.DrawLine(transform.position, transform.position + directionToObstacle * viewDistance, Color.red);
        bool shouldAvoid = Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);
    
        if (shouldAvoid)
        {
            if (!currentObstacles.Contains(obstacle.name))
            {
                currentObstacles.Add(obstacle.name);
            }

            repelDirection = hit.normal * 2f; 
            
            if (!foundObstacle)
            {
                foundObstacle = true;
            }
        }
        else 
        {
            removeObstacle(obstacle.GetComponent<Collider>());
        }   
        
    }

    private void removeObstacle(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (currentObstacles.Contains(other.gameObject.name))
            {
                currentObstacles.Remove(other.gameObject.name);
            }
            
            if (currentObstacles.Count == 0)
            {
                StartCoroutine(unsetFoundObstacle());
            }
        }
    }

    IEnumerator unsetFoundObstacle()
    {
        yield return new WaitForSeconds(1f);
        foundObstacle = false;
    }


}
