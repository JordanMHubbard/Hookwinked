using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float InteractionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCamera;
    private IInteractable currentInteractable;
    private bool hasTarget;
    
    void Awake()
    {
        
    }

    public void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.PlayerInput.actions["Interact"].performed += OnInteract;
        }
    }

    public void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.PlayerInput.actions["Interact"].performed -= OnInteract;
        }
    }


    // Update is called once per frame
    void Update()
    {
        CheckForInteraction();
    }

    void CheckForInteraction()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                var newTarget = hit.collider.GetComponent<IInteractable>();

                if (!hasTarget || currentInteractable != newTarget)
                {   
                    if (hasTarget) currentInteractable.RemoveOutline();
                    currentInteractable = newTarget;
                    currentInteractable.ShowOutline();
                    hasTarget = true;
                }
            } 
        }
        else
        {
            if (hasTarget)
            {
                currentInteractable.RemoveOutline();
                currentInteractable = null;
                hasTarget = false;
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }
}
