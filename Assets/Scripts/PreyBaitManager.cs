using UnityEngine;

public class PreyBaitManager : MonoBehaviour
{
    [SerializeField] private float pointValue = 100f;
    [SerializeField] private float energyValue = 10f;

    public float GetPointValue()
    {
        return pointValue;
    }

    public float GetEnergyValue()
    {
        return energyValue;
    }
}
