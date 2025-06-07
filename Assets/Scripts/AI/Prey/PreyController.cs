using System.Collections;
using UnityEngine;

public class PreyController : AIFishController
{
    [SerializeField] private bool isBait;
    public void SetIsBait(bool isFishBait) { isBait = isFishBait; }
    public bool GetIsBait() { return isBait; }
    [SerializeField] BoxCollider predatorDetectionBox;
    private FishAnimManager animManager;
    private PreyManager preyManager;

    protected override void Awake()
    {
        base.Awake();

        if (GameManager.Instance != null && GameManager.Instance.GetIsPerkUnlocked(3))
        {
            predatorDetectionBox.size = new Vector3(5.5f, 4f, 5.5f);
            Debug.Log("Add Silent Assassin functionality");
        }

        animManager = transform.GetComponentInChildren<FishAnimManager>();
        preyManager = transform.GetComponentInChildren<PreyManager>();
    }

    protected override void Start()
    {
        base.Start();

        SetBaitStatus();
    }

    public void SetBaitStatus()
    {
        if (isBait) 
        {
            //Debug.Log("Setting to bait");
            preyManager.tag = "Bait";
            animManager.ShouldAnimatorPlay(false);
        }
        else 
        {
            //Debug.Log("Setting to prey");
            preyManager.tag = "Prey";
            animManager.ShouldAnimatorPlay(true);
        }
    }
}
