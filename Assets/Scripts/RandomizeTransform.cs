using UnityEngine;

public class RandomizeTransform : MonoBehaviour
{
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.15f;
    [SerializeField] private Vector3 originalScale = new Vector3 (1f, 1f, 1f);
    private bool hasRandomized;
    
    private void OnValidate()
    {
        if (!hasRandomized)
        {
            randomizeScale();
            randomizeRotation();
            hasRandomized = true;
        }
    }

    private void randomizeScale()
    {
        float scale = Random.Range(minScale, maxScale);
        transform.localScale = originalScale * scale;
    }

    private void randomizeRotation()
    {
        float yRot = Random.Range(0, 181);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
}
