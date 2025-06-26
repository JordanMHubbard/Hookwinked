using System.Collections;
using UnityEngine;

public class PreyController : AIFishController
{
    [SerializeField] private bool isBait;
    private bool hasBaitMesh;
    public void SetIsBait(bool isFishBait, bool hasMesh)
    {
        isBait = isFishBait;
        hasBaitMesh = hasMesh;
    }
    public bool GetIsBait() { return isBait; }
    [SerializeField] BoxCollider predatorDetectionBox;
    [SerializeField] GameObject hook;
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
            if (hasBaitMesh) return;

            int choice = Random.Range(0, 2);
            if (choice == 0)
            {
                animManager.ShouldAnimatorPlay(false);
            }
            else
            {
                hook.SetActive(true);
            }
        }
        else
        {
            //Debug.Log("Setting to prey");
            preyManager.tag = "Prey";
            animManager.ShouldAnimatorPlay(true);
        }
    }
}
