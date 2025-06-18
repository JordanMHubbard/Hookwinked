using System.Collections;
using UnityEngine;

public class SharkPreyDetection : PreyDetection
{
    private bool isNearPlayer;
    private bool isChasingPlayerOnCooldown;

    private void OnEnable()
    {
        HomeManager.Instance.OnDayFinished += StopChasingPlayer;
    }

    private void OnDisable()
    {
         HomeManager.Instance.OnDayFinished -= StopChasingPlayer;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(HeadTowardsPlayer());
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isChasingPlayerOnCooldown || !canHunt) return;

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
            if (!isChasingPlayerOnCooldown) StartCoroutine(PlayerChaseCooldown());

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
        }
    }

    private IEnumerator PlayerChaseCooldown()
    {
        isChasingPlayerOnCooldown = true;
        Debug.Log("On cooldown");
        yield return new WaitForSeconds(2f);

        isChasingPlayerOnCooldown = false;
        Debug.Log("Cooldown off");
    }

    private void StopChasingPlayer()
    {
        closestPrey = null;
        isNearPlayer = false;
        canHunt = false;
    }
}
