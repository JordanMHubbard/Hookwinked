using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerFishController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float swimSpeed = 4f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float smoothInputTime = 0.1f;
     private float currentSpeed;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.7f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("View Bobbing")]
    [SerializeField] private bool isViewBobEnabled = true;
    [SerializeField] private float bobAmplitude = 0.004f;
    [Range(1f,30f)]
    [SerializeField] private float bobFrequency = 2f;
    [Range(10f, 100f)]
    [SerializeField] private float bobSmoothness = 10f;
    private Vector3 startPos;

    [Header("Tilt")]
    public Animator cameraAnim;
    public LayerMask layers;
    RaycastHit hit;
    private bool isTiltingLeft;
    private bool isTiltingRight;
    private bool isIdle;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashChargeRate = 24f;
    private bool isDashing;
    private float currentVelocity;
    public Slider dashChargeBar;
    public CanvasGroup dashKeybind;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] public PlayerInput playerInput;
    
    // Actions
    public InputAction swimAction;
    public InputAction dashAction;
    public InputAction floatAction;
    public InputAction lookAction;

    // Movement and Rotation
    private Vector3 currentMovement;
    private float verticalRotation;

    // Input
    private Vector3 currentInputVector;
    private Vector3 inputVelocity;

    // Energy
    private FishEnergy energyComp;
    private SoundRandomizer eatSoundComp;

     void Awake()
    {
        energyComp = GetComponent<FishEnergy>();
        eatSoundComp = GetComponent<SoundRandomizer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSensitivity /= 10f;
        currentSpeed = swimSpeed;
        startPos = mainCamera.transform.localPosition;

        swimAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Attack"];
        lookAction = playerInput.actions["Look"];
        floatAction = playerInput.actions["MoveVertical"];
        
    }

    // Update is called once per frame
    void Update()
    {
        ShouldViewBob();
        HandleMovement();
        HandleRoation();
        ShouldTilt();

        if (dashAction.IsPressed() && !isDashing)
        {
            StartCoroutine(ChargeDash());
        }
    }

    /* General Swimming Movement */
    private Vector3 CalculateWorldDirection()
    {
        if (swimAction == null) {return Vector3.zero;}

        // Action inputs mapped to vector2 have either x or y inputs
        // These values correlate the direction the player is moving
        float xInput = swimAction.ReadValue<Vector2>().x;
        float zInput = swimAction.ReadValue<Vector2>().y;
        float yInput = floatAction.ReadValue<Vector2>().y;
        Vector3 inputDirection = new Vector3(xInput, yInput, zInput);

        // Interpolates input vector to desired input so that movement is smoothed
        currentInputVector = Vector3.SmoothDamp(currentInputVector, inputDirection, ref inputVelocity, smoothInputTime);
        Vector3 worldDirection = transform.TransformDirection(currentInputVector);
        
        return worldDirection;
    }

    private void HandleMovement()
    {
        // Calculate current movement vector
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;
        currentMovement.y = worldDirection.y * floatSpeed;
        
        // Allow player to move forward based on where camera is looking
        if (swimAction.ReadValue<Vector2>().y > 0f)
        {
            currentMovement.y += mainCamera.transform.forward.y * currentSpeed;
        }
        
        characterController.Move(currentMovement * Time.deltaTime);
    }

    /* Rotation */
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

    /* Attacking */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered: " + other.name);

        if (other.CompareTag("Prey"))
        {
            float energyProg = other.GetComponent<PreyBaitManager>().GetEnergyValue();
            eatSoundComp.PlayRandomSound();
            energyComp.AddProgress(energyProg);
            other.gameObject.SetActive(false);
        }
    }

     /* Floating */

    /* Dashing */
    IEnumerator ChargeDash()
    {
        dashKeybind.alpha = 0.5f;
        
        isDashing = true; 
        currentSpeed = 2f;

        while (dashAction.IsPressed() && currentSpeed < 12f)
        {
            currentSpeed += dashChargeRate * Time.deltaTime;
            dashChargeBar.value = currentSpeed * 8.33f / 100f;
            //Debug.Log("Dash speed: " + dashSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(EndDash());
    }

    // Old code for charged and release dash
    /*IEnumerator StartDash()
    {
        float elapsedTime = 0f;
        float smoothTime = 0.1f; 
        
        // Interpolates speed to dash speed for smoothTime
        while (elapsedTime < smoothTime) 
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, dashSpeed, ref currentVelocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentSpeed = dashSpeed;
        yield return new WaitForSeconds(0.3f);

        StartCoroutine(EndDash());
    }*/

    IEnumerator EndDash()
    {
        float elapsedTime = 0f;
        float smoothTime = 0.3f; 

        // Interpolates speed to normal speed for smoothTime
        while (elapsedTime < smoothTime) 
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, swimSpeed, ref currentVelocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentSpeed = swimSpeed;

        // Reset charge bar
        while (dashChargeBar.value > 0)
        {
            dashChargeBar.value -= 2f  * Time.deltaTime;
            yield return null;
        }
        dashChargeBar.value = 0f;
        
        yield return new WaitForSeconds(dashCooldown);

        // Reset charge keybind (represents cooldown)
        while (dashKeybind.alpha < 1f)
        {
            dashKeybind.alpha += 2f  * Time.deltaTime;
            yield return null;
        }
        dashKeybind.alpha = 1f;
        
        //Debug.Log("Can Dash Again!");
        isDashing = false;
    }

    /*  View Bobbing */
    void ShouldViewBob()
    {
        if (!isViewBobEnabled) return;
        
        if (swimAction.IsPressed() || floatAction.IsPressed())
        {
            StopViewBob();
        }
        else 
        { 
            ViewBob(); 
        }

    }

    void ViewBob()
    {
        Vector3 pos = Vector3.zero;
        float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        pos.y += Mathf.Lerp(pos.y, bobOffset, bobSmoothness * Time.deltaTime);
        mainCamera.transform.localPosition += pos;
    }

    void StopViewBob()
    {
        if (Vector3.Distance(mainCamera.transform.localPosition, startPos) < 0.01f) { return; }
        mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, startPos, Time.deltaTime);
    }

    /* Tilting */
    void ShouldTilt()
    {
        float horiztonalInput = swimAction.ReadValue<Vector2>().x;

        // Checks if player is pressing a or d or is idling and plays the appropriate tilt animation
        if (horiztonalInput < 0f && !Physics.Raycast(transform.position, -transform.right, out hit, 1f, layers))
        {
            if (isTiltingLeft) { return; }
           
            cameraAnim.ResetTrigger("idle");
            cameraAnim.ResetTrigger("right");
            cameraAnim.SetTrigger("left");

            isTiltingLeft = true;
            isTiltingRight = false;
            isIdle = false;
        }
        else if (horiztonalInput > 0f &&  !Physics.Raycast(transform.position, transform.right, out hit, 1f, layers))
        {
            if (isTiltingRight) { return; }

            cameraAnim.ResetTrigger("idle");
            cameraAnim.ResetTrigger("left");
            cameraAnim.SetTrigger("right");

            isTiltingRight = true;
            isTiltingLeft = false;
            isIdle = false;
        }
        else 
        {
            if (isIdle) {return;}

            cameraAnim.ResetTrigger("left");
            cameraAnim.ResetTrigger("right");
            cameraAnim.SetTrigger("idle");

            isIdle = true;
            isTiltingLeft = false;
            isTiltingRight = false;
        }
    }
 
}
