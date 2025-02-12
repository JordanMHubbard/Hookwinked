using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerFishController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float swimSpeed = 4f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float smoothInputSpeed = 0.1f;


    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] public PlayerInput playerInput;
    
    public InputAction swimAction;
    public InputAction dashAction;
    public InputAction floatAction;
    public InputAction lookAction;
    private Vector3 currentMovement;
    private float verticalRotation;
    private float currentSpeed;
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private float currentVelocity;
    private bool isDashing;
    private bool shouldEndDash;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = swimSpeed;
        mouseSensitivity /= 10f;

        swimAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Sprint"];
        lookAction = playerInput.actions["Look"];
        floatAction = playerInput.actions["MoveVertical"];
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRoation();

        if (dashAction.IsPressed() && !isDashing)
        {
            StartCoroutine(StartDash());
        }
    }

    private Vector3 CalculateWorldDirection()
    {
        if (swimAction == null) {return Vector3.zero;}

        // Action inputs mapped to vector2 have either x or y inputs
        // These values correlate the direction the player is moving
        float xInput = swimAction.ReadValue<Vector2>().x;
        float zInput = swimAction.ReadValue<Vector2>().y;
        float yInput = floatAction.ReadValue<Vector2>().y;
        Vector3 inputDirection = new Vector3(xInput, yInput, zInput);

        currentInputVector = Vector3.SmoothDamp(currentInputVector, inputDirection, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 worldDirection = transform.TransformDirection(currentInputVector);
        
        return worldDirection;
    }

    private void HandleMovement()
    {
        //CalculateSwimSpeed();

        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;
        currentMovement.y = worldDirection.y * floatSpeed;
        
        // Code to allow player to move based on where camera is looking
        if (swimAction.ReadValue<Vector2>().y > 0f)
        {
            currentMovement.y += mainCamera.transform.forward.y * currentSpeed;
        }
        
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

    IEnumerator StartDash()
    {
        isDashing = true; 

        float elapsedTime = 0f;
        float smoothTime = 0.2f; 

        while (elapsedTime < smoothTime) 
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, dashSpeed, ref currentVelocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = dashSpeed;
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(EndDash());
    }

    IEnumerator EndDash()
    {
        float elapsedTime = 0f;
        float smoothTime = 0.2f; 

        while (elapsedTime < smoothTime) 
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, swimSpeed, ref currentVelocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = swimSpeed;
        isDashing = false;
    }
}
