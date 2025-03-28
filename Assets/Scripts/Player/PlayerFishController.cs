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
    private bool isFrozen;
    private Vector3 currentMovement;
    private float verticalRotation;

    // Input
    private Vector3 currentInputVector;
    private Vector3 inputVelocity;

    // Energy
    private FishEnergy energyComp;
    private SoundRandomizer eatSoundComp;

    // Hooked Minigame
    private int timesHooked = 0;
    private int maxTimesHooked = 2;

    private void Awake()
    {
        // Initialize components
        energyComp = GetComponent<FishEnergy>();
        eatSoundComp = GetComponent<SoundRandomizer>();

        // Setup
        mouseSensitivity /= 10f;
        currentSpeed = swimSpeed;
        startPos = mainCamera.transform.localPosition;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnHookedMinigameFinished += OnEndHookedMinigame;
    }

    private void Update()
    {
        ShouldViewBob();
        HandleMovement();
        HandleRoation();
        ShouldTilt();

        if (InputManager.Instance.DashInput && !isDashing)
        {
            StartCoroutine(ChargeDash());
        }
    }

    /* General Swimming Movement */
    private Vector3 CalculateWorldDirection()
    {
        if (InputManager.Instance == null) {return Vector3.zero;}

        // Action inputs mapped to vector2 have either x or y inputs
        // These values correlate the direction the player is moving
        float xInput = InputManager.Instance.SwimInput.x;
        float zInput = InputManager.Instance.SwimInput.y;
        float yInput = InputManager.Instance.FloatInput.y;
        Vector3 inputDirection = new Vector3(xInput, yInput, zInput);

        // Interpolates input vector to desired input so that movement is smoothed
        currentInputVector = Vector3.SmoothDamp(currentInputVector, inputDirection, ref inputVelocity, smoothInputTime);
        Vector3 worldDirection = transform.TransformDirection(currentInputVector);
        
        return worldDirection;
    }

    private void HandleMovement()
    {
        if (isFrozen) return;
        // Calculate current movement vector
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;
        currentMovement.y = worldDirection.y * floatSpeed;
        
        // Allow player to move forward based on where camera is looking
        if (InputManager.Instance.SwimInput.y > 0f)
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
        if (InputManager.Instance == null) {return;}

        float mouseXRotation = InputManager.Instance.LookInput.x * mouseSensitivity;
        float mouseYRotation = InputManager.Instance.LookInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    /* Attacking Prey */
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entered: " + other.name);
        if (other.CompareTag("Prey")) 
        {
            EatPrey(other);
        }
        else if (other.CompareTag("Bait")) 
        {
            if (timesHooked++ < maxTimesHooked)
            {
                BeginHookedMinigame(other);
            }
            else 
            {
                StartCoroutine(Death());
            }
        }
    }

    private void EatPrey(Collider other)
    {
        Debug.Log("We eating prey 2nite");
        float energyProg = other.GetComponent<PreyManager>().GetEnergyValue();
        eatSoundComp.PlayRandomSound();
        energyComp.AddProgress(energyProg);
        GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        
        energyComp.setIsPaused(true);
        GameManager.Instance.DisableHUD();
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.HookedMinigame);
        cameraAnim.Play("death", 0);
    }

    private void BeginHookedMinigame(Collider other)
    {
        eatSoundComp.PlayRandomSound();
        energyComp.setIsPaused(true);
        GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        Debug.Log("Fight for your life!");
        GameManager.Instance.StartHookedMinigame();
    }

    private void OnEndHookedMinigame()
    {
        currentSpeed = 12f;
        isFrozen = true;
        energyComp.AddProgress(-10f);
        energyComp.setIsPaused(false);
        StartCoroutine(PostHookedDash());
    }

    private IEnumerator PostHookedDash()
    {
        float elapsedTime = 0f;
        float taperOffRate = (currentSpeed - swimSpeed) / 1.5f; // subtract desired end speed

        while (elapsedTime < 1.5f)
        {
            transform.position += transform.forward * currentSpeed  * Time.deltaTime;
            if (currentSpeed > swimSpeed) currentSpeed -= taperOffRate * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isFrozen = false;
        currentSpeed = swimSpeed;
    }

    /* Dashing */
    private IEnumerator ChargeDash()
    {
        dashKeybind.alpha = 0.5f;
        
        isDashing = true; 
        currentSpeed = 2f;

        while (InputManager.Instance.DashInput && currentSpeed < 12f)
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
        
        if (InputManager.Instance.SwimIsPressed || InputManager.Instance.FloatIsPressed)
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
        float horiztonalInput = InputManager.Instance.SwimInput.x;

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
