using System.Collections;
using UnityEngine;

public class SharkPreyDetection : PreyDetection
{
    private bool isNearPlayer;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(HeadTowardsPlayer());
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            closestPrey = other.gameObject;
            isNearPlayer = true;
            isPursuingPrey = true;
            if (closestPrey != null) StartPursuit();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            closestPrey = null;
            isNearPlayer = false;
            FindClosestPrey();
        }
    }

    private IEnumerator HeadTowardsPlayer()
    {
        yield return new WaitForSeconds(3f);

        while (canHunt)
        {
            yield return new WaitForSeconds(Random.Range(16, 40));

            if (isNearPlayer) continue;

            controller.SetTargetPosition(GameManager.Instance.PlayerController.transform.position);
            Debug.Log("heading towards player's current position");
        }
    }
}
