using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float InteractionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCamera;
    private Interactable currentInteractable;
    
    void Awake()
    {
        
    }

    public void OnEnable()
    {
        if (InputManager.instance != null)
        {
            InputManager.PlayerInput.actions["Interact"].performed += OnInteract;
        }
    }

    public void OnDisable()
    {
        if (InputManager.instance != null)
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
               currentInteractable = hit.collider.GetComponent<Interactable>();
            } 
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable = null;
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}
