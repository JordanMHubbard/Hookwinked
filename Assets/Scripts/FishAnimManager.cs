using UnityEngine;

public class FishAnimManager : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"{gameObject.name}: animator has not been set!");
            enabled = false;
        }

        controller = GetComponentInParent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning($"{gameObject.name}: controller has not been set!");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", controller.velocity.magnitude/3);
        
        /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("Swim"))
        {
            Debug.Log("Swim");
        }*/
        //Debug.Log("Speed: " + animator.GetFloat("Speed"));
    }
}
