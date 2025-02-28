using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    private float viewDistance = 3f;
    private bool isNearObstacle;
    private int obstacleCount;
    private AIFishController controller;

    void Start()
    {
        controller = GetComponentInParent<AIFishController>();
        
        if (controller == null )
        {
            Debug.LogError("ObstacleAvoidance: No AIFishController found on " + gameObject.name);
        }
        
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            AvoidObstacle();
            obstacleCount++;
            isNearObstacle = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Vector3 targetDirection = (controller.GetTargetLocation() - transform.position).normalized; 
            IsObstacleInDirection(targetDirection, true, 90f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            obstacleCount--;
            if (obstacleCount == 0) isNearObstacle = false;
        }
    }

    public bool IsObstacleInDirection(Vector3 direction, bool doObstacleAvoidance, float angleThreshold = 30f)
    {
        // Checks if any obstacles are currently in the way in the given direction
        Ray ray = new Ray(transform.position, direction);
        bool obstacleInWay = Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);

        // Calculate angle between current forward direction and the direction of interest (target or forward)
        if (obstacleInWay)
        {
            Vector3 directionToObstacle = (hit.point - transform.position).normalized;
            float dot = Vector3.Dot(directionToObstacle, transform.forward);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            //Debug.Log("angle is: " + angle);
            // If obstacle is in the way and within the given angle threshold
            if (angle < angleThreshold)
            {
                // Maybe make it so it can only fire again once avoid obstacle has been called?
                Debug.DrawLine(transform.position, transform.position + direction * viewDistance, Color.red, 15f);
                if (doObstacleAvoidance && !controller.GetIsRepelling())
                {
                    AvoidObstacle();
                }
                return true;
            }
            else return false;
        }
        else 
        {
            Debug.DrawLine(transform.position, transform.position + direction * viewDistance, Color.green, 15f);
            if (controller.GetIsRepelling()) controller.SetIsRepelling(false);
            return false;
        }
    }

    public void AvoidObstacle()
    {
        int i = 1;
        int numPoints = 50;
        float turnRatio = 1.618f;
        float radius = viewDistance;
        
        while (i < numPoints)
        {
            Debug.Log("Counting: " + i);
            //Projects points on a sphere
            float angle  = 2 * Mathf.PI * turnRatio;
            float t =  (float) i / numPoints;
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angle * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);

            // Checks if obstacle is in way of potential point 
            Vector3 worldOffset = (transform.right * x + transform.up * y + transform.forward * z) * radius;
            Vector3 point = worldOffset + transform.position;
            Vector3 directionToPoint = (point - transform.position).normalized; 
            Ray ray = new Ray(transform.position, directionToPoint);
            bool hitObstacle = Physics.Raycast(ray, out RaycastHit hit, viewDistance, interactableLayer);
            Debug.DrawLine(transform.position, transform.position + directionToPoint * viewDistance, Color.yellow, 15f);
            
            if (!hitObstacle && !isObstacleAtPoint(point))
            {
                // Add a way to reevaluate point if current one too close to an obstacle?   
                controller.SetRepelDirection(directionToPoint);
                controller.SetIsRepelling(true);
                isNearObstacle = true;
                return;
            }
            //Debug.Log("our Location: " + transform.position);
            i++;
        }

        return;
    }

    private bool isObstacleAtPoint (Vector3 checkPoint)
    {
        // Should be roughly a bit bigger than the size of the fish
        Collider[] colliders = Physics.OverlapSphere(checkPoint, 0.5f, interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        return false;
    }



    /* DEPRECATED (Initial version)

    private void OldAvoidObstacle(Collider other)
    {
        GameObject obstacle = other.gameObject;
        if (obstacle == null) return;

        // When overlapping obstacle, check if hit ray more closely aligns with left direction or right direction,
        // and choose the opposite direction to turn towards

        /*Vector3 obstaclePosition = obstacle.transform.position;
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
            repelRate = angleL * 1.7f  / 100;
        }
        else
        {
            repelDirection = leftDir;
            //Debug.Log("Going Left");

            repelRate = angleL * 1.7f  / 100;
        }
        Debug.Log("Repel Rate: " + repelRate);

        if (!isRepelling) isRepelling = true;

    }*/
}
