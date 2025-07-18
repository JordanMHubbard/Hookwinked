using System.Collections;
using UnityEngine;

public class PreyController : AIFishController
{
    [SerializeField] private bool isLure;
    public void SetIsLure(bool isFishingLure) { isLure = isFishingLure; }
    public bool GetIsLure() { return isLure; }
    [SerializeField] BoxCollider predatorDetectionBox;
    [SerializeField] GameObject hook;
    private FishAnimManager animManager;
    private PreyManager preyManager;

    protected override void Awake()
    {
        base.Awake();

        if (GameManager.Instance != null && GameManager.Instance.GetIsPerkUnlocked(3))
        {
            predatorDetectionBox.size = new Vector3(4f, 2f, 4f);
            Debug.Log("Add Silent Assassin functionality");
        }

        animManager = transform.GetComponentInChildren<FishAnimManager>();
        preyManager = transform.GetComponentInChildren<PreyManager>();
    }

    protected override void Start()
    {
        base.Start();

        SetLureStatus();
    }

    public void SetLureStatus()
    {
        if (isLure)
        {
            //Debug.Log("Setting to bait");
            preyManager.tag = "Bait";

            int choice = Random.Range(0, 3);
            if (choice == 0)
            {
                hook.SetActive(true);
            }
            else
            {
                animManager.ShouldAnimatorPlay(false);
            }
        }
        else
        {
            //Debug.Log("Setting to prey");
            preyManager.tag = "Prey";
            animManager.ShouldAnimatorPlay(true);
            hook.SetActive(false);
        }
    }
}
