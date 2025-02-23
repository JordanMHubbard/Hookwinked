using System.Collections.Generic;
using UnityEngine;

public class AIFishController : MonoBehaviour
{
    private bool hasTarget;
    private bool isRepelling;
    private bool foundPrey;
    private List<GameObject> currentObstacles;
    private Vector3 repelDirection;
    private Vector3 targetLocation;
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

        currentObstacles = new List<GameObject>();
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

    bool IsAreaBlocked(Vector3 checkPosition, Vector3 boxSize)
    {
        Collider[] colliders = Physics.OverlapBox(checkPosition, boxSize * 0.5f, Quaternion.identity, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        return false;
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
            Vector3 locationOffset = new Vector3 (Random.Range(-10,10), 0, Random.Range(-10,10));
            targetLocation = transform.position + locationOffset;
            Debug.Log("targetLocation = " + targetLocation);

            if (IsAreaBlocked(targetLocation, new Vector3(2, 2, 2)))
            {
                Debug.Log("Area blocked!");
                return false;
            } 
        }
        
        return true;
    }

    private void Rotate()
    {   
        //if (isRepelling) return;
        
        
        float turnSpeed = swimSpeed * Random.Range(1f, 2f);
        Vector3 targetDirection = targetLocation - transform.position;
        Vector3 lookAt;
        if (isRepelling) 
        {
            Debug.Log("We are repelling now");
            lookAt = repelDirection;
            Debug.DrawLine(transform.position, transform.position + repelDirection * viewDistance, Color.red);
        }
        else{
            lookAt = targetDirection;
        }
        //Vector3 lookAt = isRepelling ? (targetDirection + repelDirection) / 2 : targetDirection;
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
        //Debug.DrawLine(transform.position, transform.position + directionToObstacle * viewDistance, Color.red);
        Ray ray = new Ray(transform.position, directionToObstacle);
        Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);

       
        Vector3 leftDir = Quaternion.Euler(0, -45, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, 45, 0) * transform.forward;
        Debug.DrawLine(transform.position, transform.position + leftDir * viewDistance, Color.green);
        Debug.DrawLine(transform.position, transform.position + rightDir * viewDistance, Color.green);
        
        float dotL = Vector3.Dot(Vector3.Normalize(directionToObstacle), Vector3.Normalize(leftDir));
        float angleL = Mathf.Acos(dotL) * Mathf.Rad2Deg;

        float dotR = Vector3.Dot(Vector3.Normalize(directionToObstacle), Vector3.Normalize(rightDir));
        float angleR = Mathf.Acos(dotR) * Mathf.Rad2Deg;

        if (angleL < angleR)
        {
            repelDirection = rightDir;
            Debug.Log("Going Right");
        }
        else
        {
            repelDirection = leftDir;
            Debug.Log("Going Left");
        }

        if (!isRepelling) isRepelling = true;
    }

    private void IsObstacleInTargetDirection()
    {
        //Maybe fix this so it covers a wider range of area?
        
        if (!isRepelling) 
        {
            //Debug.Log("I will not check if obstacle in target direction");
            return;
        }
        
        Vector3 desiredDirection = targetLocation - transform.position; 
        Ray ray = new Ray(transform.position, desiredDirection);
        Debug.DrawLine(transform.position, transform.position + desiredDirection * viewDistance, Color.magenta);
        bool obstacleInWay = Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);

        if (obstacleInWay) 
        {
            //AddObstacle(hit.collider);
            return;
        }

        //RemoveObstacle(hit.collider);
        isRepelling = false; 
    }


    void AddObstacle(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (!currentObstacles.Contains(other.gameObject))
            {
                currentObstacles.Add(other.gameObject);
            }
        }
    }

    void RemoveObstacle(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (currentObstacles.Contains(other.gameObject))
            {
                currentObstacles.Remove(other.gameObject);
            }
        }
    }

}
