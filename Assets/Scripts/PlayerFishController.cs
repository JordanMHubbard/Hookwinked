using System;
using System.Collections;
using UnityEngine;
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

    // Movement and Rotation
    private Vector3 currentMovement;
    private float verticalRotation;

    // Input
    private Vector3 currentInputVector;
    private Vector3 inputVelocity;

    // Energy
    private FishEnergy energyComp;
    private SoundRandomizer eatSoundComp;

    private void Awake()
    {
        // Initialize components
        energyComp = GetComponent<FishEnergy>();
        eatSoundComp = GetComponent<SoundRandomizer>();

        // Setup
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSensitivity /= 10f;
        currentSpeed = swimSpeed;
        startPos = mainCamera.transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        ShouldViewBob();
        HandleMovement();
        HandleRoation();
        ShouldTilt();

        if (InputManager.instance.DashInput && !isDashing)
        {
            StartCoroutine(ChargeDash());
        }
    }

    /* General Swimming Movement */
    private Vector3 CalculateWorldDirection()
    {
        if (InputManager.instance == null) {return Vector3.zero;}

        // Action inputs mapped to vector2 have either x or y inputs
        // These values correlate the direction the player is moving
        float xInput = InputManager.instance.SwimInput.x;
        float zInput = InputManager.instance.SwimInput.y;
        float yInput = InputManager.instance.FloatInput.y;
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
        if (InputManager.instance.SwimInput.y > 0f)
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
        if (InputManager.instance == null) {return;}

        float mouseXRotation = InputManager.instance.LookInput.x * mouseSensitivity;
        float mouseYRotation = InputManager.instance.LookInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    /* Attacking */
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entered: " + other.name);
        if (other.CompareTag("Prey")) EatPrey(other);
        else if (other.CompareTag("Bait")) StartStruggleMinigame(other);
    }

    private void EatPrey (Collider other)
    {
        float energyProg = other.GetComponent<PreyManager>().GetEnergyValue();
        eatSoundComp.PlayRandomSound();
        energyComp.AddProgress(energyProg);
        GameManager.Instance.PreyConsumed(other.gameObject);
    }

    private void StartStruggleMinigame (Collider other)
    {
        GameManager.Instance.PreyConsumed(other.gameObject);
        Debug.Log("Fight for your life!");
        GameManager.Instance.StartHookedMinigame();
        energyComp.AddProgress(-10f);
    }

     /* Floating */

    /* Dashing */
    private IEnumerator ChargeDash()
    {
        dashKeybind.alpha = 0.5f;
        
        isDashing = true; 
        currentSpeed = 2f;

        while (InputManager.instance.DashInput && currentSpeed < 12f)
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

    private IEnumerator EndDash()
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
    private void ShouldViewBob()
    {
        if (!isViewBobEnabled) return;
        
        if (InputManager.instance.SwimIsPressed || InputManager.instance.FloatIsPressed)
        {
            StopViewBob();
        }
        else 
        { 
            ViewBob(); 
        }

    }

    private void ViewBob()
    {
        Vector3 pos = Vector3.zero;
        float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        pos.y += Mathf.Lerp(pos.y, bobOffset, bobSmoothness * Time.deltaTime);
        mainCamera.transform.localPosition += pos;
    }

    private void StopViewBob()
    {
        if (Vector3.Distance(mainCamera.transform.localPosition, startPos) < 0.01f) { return; }
        mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, startPos, Time.deltaTime);
    }

    /* Tilting */
    private void ShouldTilt()
    {
        float horiztonalInput = InputManager.instance.SwimInput.x;

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
