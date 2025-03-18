using UnityEngine;

public class RandomizeTransform : MonoBehaviour
{
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private Vector3 originalScale = new Vector3 (1f, 1f, 1f);
    
    private void OnValidate()
    {
        randomizeScale();
        randomizeRotation();
    }

    private void randomizeScale()
    {
        float scale = Random.Range(minScale, maxScale);
        transform.localScale = originalScale * scale;
        Debug.Log("randomized that scale curly pop");
    }

    private void randomizeRotation()
    {
        float yRot = Random.Range(0, 181);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
}
