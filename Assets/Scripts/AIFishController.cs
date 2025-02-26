using System.Collections.Generic;
using UnityEngine;

public class AIFishController : MonoBehaviour
{
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private LayerMask interactableLayer;
    private float viewDistance = 5f;
    private bool hasTarget;
    private bool isRepelling;
    private bool foundPrey;
    private bool isReturningFromRepel;
    private Vector3 repelDirection;
    private Vector3 targetLocation;
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
            IsObstacleInTargetDirection();
            Rotate();
            MoveTowardsTarget();
        }
    }

    // Checks if randomly selected target area is accessible
    bool IsAreaReachable(Vector3 checkPosition, Vector3 boxSize)
    {
        bool isInReachableArea = false;

        Collider[] colliders = Physics.OverlapBox(checkPosition, boxSize * 0.5f, Quaternion.identity, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                return true;
            }

            if (col.CompareTag("ReachableArea"))
            {
                isInReachableArea = true;
                Debug.Log("In reachable area yipee");
            }
        }

        if (!isInReachableArea) return true;

        return false;
    }

    // Determines whether to generate random target or follow prey
    private bool FindTarget()
    {
        if (foundPrey)
        {
            // targetLocation = Prey.location;
        }
        else 
        {
            // Later need to consider only getting points within a defined space 
            Vector3 locationOffset = new Vector3 (Random.Range(-15,15), Random.Range(-5,5), Random.Range(-15,15));
            targetLocation = transform.position + locationOffset;
            Debug.Log("targetLocation = " + targetLocation);

            if (IsAreaReachable(targetLocation, new Vector3(2, 2, 2)))
            {
                Debug.Log("Area blocked!");
                return false;
            } 
        }
        
        return true;
    }

    // Determines fish orientation based on target location and obstacles
    private void Rotate()
    {   

        float turnSpeed = swimSpeed * Random.Range(1f, 2f);
        Vector3 targetDirection = targetLocation - transform.position;
        Vector3 lookAt;

        if (isRepelling) 
        {
            Debug.Log("Repelling now");
            lookAt = repelDirection;
            Debug.DrawLine(transform.position, transform.position + repelDirection * viewDistance, Color.red);
        }
        else
        {
            lookAt = targetDirection;
        }
         
        if (isReturningFromRepel) return;
        //Vector3 lookAt = isRepelling ? (targetDirection + repelDirection) / 2 : targetDirection;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAt), 
            turnSpeed * Time.deltaTime);
    }

    // Moves fish towards target and evaluates if target has been reached
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
            AvoidObstacle(other);
        }
        
    }

    private void AvoidObstacle(Collider other)
    {
        GameObject obstacle = other.gameObject;
        if (obstacle == null) return;

        // When overlapping obstacle, check if hit ray more closely aligns with left direction or right direction,
        // and choose the opposite direction to turn towards

        Vector3 obstaclePosition = obstacle.transform.position;
        Vector3 directionToObstacle = obstaclePosition - transform.position; 
        Ray ray = new Ray(transform.position, directionToObstacle);
        Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);

       
        Vector3 leftDir = Quaternion.Euler(0, -60, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, 60, 0) * transform.forward;
        Debug.DrawLine(transform.position, transform.position + leftDir * viewDistance, Color.green);
        Debug.DrawLine(transform.position, transform.position + rightDir * viewDistance, Color.green);
        
        float dotL = Vector3.Dot(Vector3.Normalize(directionToObstacle), Vector3.Normalize(leftDir));
        float angleL = Mathf.Acos(dotL) * Mathf.Rad2Deg;

        float dotR = Vector3.Dot(Vector3.Normalize(directionToObstacle), Vector3.Normalize(rightDir));
        float angleR = Mathf.Acos(dotR) * Mathf.Rad2Deg;

        if (angleL < angleR)
        {
            repelDirection = rightDir;
            //Debug.Log("Going Right");
        }
        else
        {
            repelDirection = leftDir;
            //Debug.Log("Going Left");
        }

        if (!isRepelling) isRepelling = true;
    }

    private void IsObstacleInTargetDirection()
    {
        if (!isRepelling) return;
        
        Vector3 desiredDirection = targetLocation - transform.position; 
        Ray ray = new Ray(transform.position, desiredDirection);
        Debug.DrawLine(transform.position, transform.position + desiredDirection * viewDistance, Color.magenta);
        bool obstacleInWay = Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);

        // Checks to see if direction of obstacle is
        float dot = Vector3.Dot(Vector3.Normalize(desiredDirection), Vector3.Normalize(transform.forward));
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (obstacleInWay && angle < 90f) return;
        

        isRepelling = false; 
        if (!isReturningFromRepel) StartCoroutine(RepelTimer());
    }

    System.Collections.IEnumerator RepelTimer()
    {
        isReturningFromRepel = true;
        Debug.Log("Timer Start");

        yield return new WaitForSeconds(1f);
        
        isReturningFromRepel = false;

        Debug.Log("Timer End");
        
        yield return null;
    }
}
