using System.Collections;
using UnityEngine;

public class AIFishController : MonoBehaviour
{
    // Variables
    private float swimSpeed;
    [SerializeField] private float defaultSwimSpeed = 3f;
    [SerializeField] private float fasterSwimSpeed = 5f;
    [SerializeField] private float speedUpDuration = 3f;
    [SerializeField] private float speedUpCooldown = 3f;
    private bool isSpeedUpOnCooldown;
    [SerializeField] private LayerMask interactableLayer;
    private bool hasTarget;
    private bool isRepelling;
    private Vector3 repelDirection;
    private Vector3 targetLocation;
    private CharacterController characterController;
    // Getters
    public Vector3 GetTargetLocation() { return targetLocation; }
    public bool GetIsRepelling() { return isRepelling; }
    public bool GetIsSpeedUpOnCooldown() { return isSpeedUpOnCooldown; }

    // Setters
    public void SetTargetPosition(Vector3 position) { targetLocation = position; }
    public void SetIsRepelling(bool shouldRepel ) {isRepelling = shouldRepel; }
    public void SetRepelDirection(Vector3 direction) { repelDirection = direction; }

    protected virtual void Awake()
    {
        swimSpeed = defaultSwimSpeed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (characterController == null)
        {
            Debug.LogWarning($"{gameObject.name}: characterController has not been set!");
            enabled = false;
        }
    }


    // Update is called once per frame
    protected virtual void Update()
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
    private bool IsAreaReachable(Vector3 checkPosition, Vector3 boxSize)
    {
        bool isInReachableArea = false;

        Collider[] colliders = Physics.OverlapBox(checkPosition, boxSize * 0.5f, Quaternion.identity, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                return false;
            }

            if (col.CompareTag("ReachableArea"))
            {
                isInReachableArea = true;
                //Debug.Log("In reachable area yipee");
            }
        }

        return isInReachableArea;
    }

    // Determines whether to generate random target or follow prey
    public bool FindTarget()
    {
        Vector3 locationOffset = new Vector3 (Random.Range(-15,15), Random.Range(-5,5), Random.Range(-15,15));
        targetLocation = transform.position + locationOffset;
        //Debug.Log("targetLocation = " + targetLocation);

        if (!IsAreaReachable(targetLocation, new Vector3(2, 2, 2)))
        {
            //Debug.Log("Area blocked!");
            return false;
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
            hasTarget = false;
        }
    }

    private void ShouldRepel()
    {
        float dot = Vector3.Dot(Vector3.Normalize(repelDirection), Vector3.Normalize(transform.forward));
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle < 1f)
        {
            //Debug.Log("angle: " + angle);
            isRepelling = false;
        }
    }

    public IEnumerator SpeedUp()
    {   
        isSpeedUpOnCooldown = true;
        swimSpeed = fasterSwimSpeed;
        
        float elapsedTime = 0f;
        float currentSpeed;
        float targetSpeed = defaultSwimSpeed;

        while (elapsedTime < speedUpDuration)
        {
            float t = elapsedTime / speedUpDuration;
            currentSpeed = Mathf.Lerp(fasterSwimSpeed, targetSpeed, t);
            swimSpeed = currentSpeed;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //yield return new WaitForSeconds(duration);
        
        swimSpeed = defaultSwimSpeed;
        yield return new WaitForSeconds(speedUpCooldown);

        isSpeedUpOnCooldown = false;
    }
}
