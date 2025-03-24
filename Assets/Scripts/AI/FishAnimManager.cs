using UnityEngine;

public class FishAnimManager : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private bool shouldAnimate = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name}: animator has not been set!");
            enabled = false;
        }

        characterController = GetComponentInParent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogWarning($"{gameObject.name}: characterController has not been set!");
            enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (shouldAnimate) animator.SetFloat("Speed", characterController.velocity.magnitude/3); 
    }

    public void ShouldAnimatorPlay(bool shouldPlay)
    {
        animator.enabled = shouldPlay;
        shouldAnimate = shouldPlay;
    }
}
