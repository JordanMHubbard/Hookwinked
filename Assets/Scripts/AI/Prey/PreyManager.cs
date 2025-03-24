using System.Collections;
using UnityEngine;

public class PreyManager : MonoBehaviour
{
    [SerializeField] private float pointValue = 100f;
    [SerializeField] private float energyValue = 10f;
    private PreyController controller;
    private FishAnimManager animManager;

    private void Start()
    {
        controller = GetComponentInParent<PreyController>();
        animManager = transform.parent.GetComponentInChildren<FishAnimManager>();
       
        DetermineBaitStatus();
    }
    private void DetermineBaitStatus()
    {
        // 1/6 chance prey will be bait
        int choice = Random.Range(0, 2);
        
        if (choice == 0) 
        {
            controller.SetIsBait(true);
            gameObject.tag = "Bait";
            animManager.ShouldAnimatorPlay(false);
        }
        else controller.SetIsBait(false);

        //Debug.Log("is Bait? " + gameObject.tag);
        
    }

    public float GetPointValue()
    {
        return pointValue;
    }

    public float GetEnergyValue()
    {
        return energyValue;
    }
}
