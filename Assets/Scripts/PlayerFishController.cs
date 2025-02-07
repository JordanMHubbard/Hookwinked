using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerFishController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float swimSpeed = 3.0f;
    [SerializeField] private float fastSwimSpeed = 4.5f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] public PlayerInput playerInput;
    
    public InputAction swimAction;
    public InputAction swimFastAction;
    public InputAction swimVerticalAction;
    public InputAction lookAction;
    private Vector3 currentMovement;
    private float verticalRotation;
    private float currentSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = swimSpeed;
        mouseSensitivity /= 10f;

        swimAction = playerInput.actions["Move"];
        swimFastAction = playerInput.actions["Sprint"];
        lookAction = playerInput.actions["Look"];
        swimVerticalAction = playerInput.actions["MoveVertical"];
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRoation();
    }

    private Vector3 CalculateWorldDirection()
    {
        if (swimAction == null) {return Vector3.zero;}

        // Action inputs mapped to vector2 have either x or y inputs
        // These values correlate the direction the player is moving
        float xInput = swimAction.ReadValue<Vector2>().x;
        float zInput = swimAction.ReadValue<Vector2>().y;
        float yInput = swimVerticalAction.ReadValue<Vector2>().y;

        Vector3 inputDirection = new Vector3(xInput, yInput, zInput);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        return worldDirection.normalized;
    }

    private void HandleMovement()
    {
        CalculateSwimSpeed();

        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;
        currentMovement.y = worldDirection.y * currentSpeed;
        
        // Code to allow player to move based on where camera is looking
        if (swimAction.IsPressed())
        {
            currentMovement.y += mainCamera.transform.forward.y * currentSpeed;
        }
        else { currentMovement.y = 0; }

        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRoation()
    {
        if (lookAction == null) {return;}

        float mouseXRotation = lookAction.ReadValue<Vector2>().x * mouseSensitivity;
        float mouseYRotation = lookAction.ReadValue<Vector2>().y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    private void CalculateSwimSpeed()
    {
        if (swimFastAction.IsPressed()) 
        { 
            if (currentSpeed < fastSwimSpeed)
            {
                currentSpeed = fastSwimSpeed;
                //Debug.Log("Speed = " + currentSpeed);
            }
        }
        else
        { 
            if (currentSpeed > swimSpeed) { currentSpeed = swimSpeed; }
        }
    }
}
