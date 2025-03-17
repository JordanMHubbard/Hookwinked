using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CamParent"))
        {
            GameManager.Instance.ShowDeathScreen();
        }
    }
}
