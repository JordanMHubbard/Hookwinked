using UnityEngine;

public class PreyManager : MonoBehaviour
{
    [SerializeField] private float pointValue = 100f;
    [SerializeField] private float energyValue = 10f;
    private bool isBait = false;

    private void Start()
    {
        DetermineBaitStatus();
    }
    private void DetermineBaitStatus()
    {
        // 1/4 chance prey will be bait
        int choice = Random.Range(0, 4);
        
        if (choice == 0) isBait = true;
        else isBait = false;
    }

    public float GetPointValue()
    {
        return pointValue;
    }

    public float GetEnergyValue()
    {
        return energyValue;
    }

    public bool GetIsBait()
    {
        return isBait;
    }
}
