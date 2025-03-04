using UnityEngine;

public class AIFishController : MonoBehaviour
{
    // Variables
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private LayerMask interactableLayer;
    private bool hasTarget;
    private bool isRepelling;
    private bool foundPrey;
    private Vector3 repelDirection;
    private Vector3 targetLocation;
    private CharacterController characterController;
    // Getters
    public Vector3 GetTargetLocation() { return targetLocation; }
    public bool GetIsRepelling() { return isRepelling; }
    public bool GetFoundPrey() { return foundPrey; }

    // Setters
    public void SetTargetPosition(Vector3 position) { targetLocation = position; }
    public void SetIsRepelling(bool shouldRepel ) {isRepelling = shouldRepel; }
    public void SetRepelDirection(Vector3 direction) { repelDirection = direction; }
    public void SetfoundPrey(bool isFound) { foundPrey = isFound; }


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
                //Debug.Log("In reachable area yipee");
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
            //Debug.Log("targetLocation = " + targetLocation);

            if (IsAreaReachable(targetLocation, new Vector3(2, 2, 2)))
            {
                //Debug.Log("Area blocked!");
                return false;
            } 
        }
        
        return true;
    }

    // Determines fish orientation based on target location and obstacles
    private void Rotate()
    {   
        //Debug.Log("isRepelling: " + isRepelling + " isNearObstacle: " + isNearObstacle);

        float turnSpeed = swimSpeed * Random.Range(1f, 2f);
        Vector3 targetDirection = targetLocation - transform.position;
        Vector3 lookAt;

        if (isRepelling) 
        {
            
            ShouldRepel();
            lookAt = repelDirection;
        }
        else
        {
            lookAt = targetDirection;
        }
        
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

    private void ShouldRepel()
    {
        float dot = Vector3.Dot(Vector3.Normalize(repelDirection), Vector3.Normalize(transform.forward));
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle < 1f)
        {
            Debug.Log("angle: " + angle);
            isRepelling = false;
        }
    }
}
