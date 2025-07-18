using System.Collections;
using UnityEngine;

public class SharkPreyDetection : PreyDetection
{
    private bool isNearPlayer;
    private bool isChasingPlayerOnCooldown;

    private void OnEnable()
    {
        HomeManager.Instance.OnDayFinished += DisablePlayerChase;
    }

    private void OnDisable()
    {
        HomeManager.Instance.OnDayFinished -= DisablePlayerChase;
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
            Debug.Log("Found Player enter");
            if (isChasingPlayerOnCooldown || !canHunt) return;

            Debug.Log("Chasing Player enter");
            StartChasingPlayer(other.gameObject);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StopChasingPlayer());
        }
    }

    private IEnumerator HeadTowardsPlayer()
    {
        yield return new WaitForSeconds(3f);

        while (canHunt)
        {
            yield return new WaitForSeconds(Random.Range(15, 30));

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

    private void StartChasingPlayer(GameObject player)
    {
        closestPrey = player;
        isNearPlayer = true;
        isPursuingPrey = true;
        if (closestPrey != null) StartPursuit();
    }

    private IEnumerator StopChasingPlayer()
    {
        isNearPlayer = false;
        Debug.Log("Stop Chasing Player started");
        yield return new WaitForSeconds(1f);

        if (!isNearPlayer)
        {
            Debug.Log("Leaving player alone");
            closestPrey = null;
            FindClosestPrey();
            if (!isChasingPlayerOnCooldown) StartCoroutine(PlayerChaseCooldown());
        }
    }

    private void DisablePlayerChase()
    {
        Debug.Log("Chasing player disabled!");
        closestPrey = null;
        isNearPlayer = false;
        canHunt = false;
    }
}
