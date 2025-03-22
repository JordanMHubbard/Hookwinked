using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    float desiredY;
    Vector3 desiredPos;
    GameObject player;
   
    void Start()
    {
        player = transform.parent.gameObject;
    }

    // Makes sure spawner starts from ocean floor
    private void Update()
    {
        desiredY = -player.transform.position.y;
        desiredPos = new Vector3(transform.localPosition.x, desiredY, transform.localPosition.z);
        transform.localPosition = desiredPos;
    }
}
