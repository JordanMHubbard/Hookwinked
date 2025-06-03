using UnityEngine;

public class EditorOnlyObject : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }
}
