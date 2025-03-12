using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class PreyController : MonoBehaviour
{
    [SerializeField] private float floatFrequency = 2f;
    [SerializeField] private float floatAmplitude = 0.01f;
    [SerializeField] private float floatSmoothness = 10f;

    // Update is called once per frame
    void Update()
    {
        Float();
    }

    private void Float()
    {
        Vector3 pos = Vector3.zero;
        float offset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        pos.y += Mathf.Lerp(pos.y, offset, floatSmoothness * Time.deltaTime);
        transform.position += pos;
        //Debug.Log("pos: " + transform.position);
    }
}
