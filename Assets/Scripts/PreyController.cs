using UnityEngine;

public class PreyController : AIFishController
{
    private Collider characterCollider;

    private void Awake()
    {
        characterCollider = GetComponent<CharacterController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Fish"))
        {
            Physics.IgnoreCollision(characterCollider, other);
        }
    }
}
