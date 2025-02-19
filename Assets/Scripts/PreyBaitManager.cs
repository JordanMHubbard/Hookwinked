using UnityEngine;

public class PreyBaitManager : MonoBehaviour
{
    [SerializeField] private float pointValue = 100f;
    [SerializeField] private float energyValue = 10f;
    private bool isPrey = true;

    public float GetPointValue()
    {
        return pointValue;
    }

    public float GetEnergyValue()
    {
        return energyValue;
    }

    public bool GetIsPrey()
    {
        return isPrey;
    }
}
