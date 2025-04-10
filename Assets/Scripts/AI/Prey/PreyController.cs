using System.Collections;
using UnityEngine;

public class PreyController : AIFishController
{
    private bool isBait;
    public void SetIsBait(bool isFishBait) { isBait = isFishBait; }
    public bool GetIsBait() { return isBait; }
    [SerializeField] BoxCollider predatorDetectionBox;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetIsPerkUnlocked(3))
        {
            predatorDetectionBox.size = new Vector3(5.5f, 4f, 5.5f);
            Debug.Log("Add Silent Assassin functionality");
        }
    }
}
