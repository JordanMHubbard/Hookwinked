using System;
using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashChargeRate = 50f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private AudioClip[] dashSounds;
    private bool isDashOnCooldown;
    private bool isDashing;
    private float currentVelocity;
    public Slider dashChargeBar;
    public CanvasGroup dashKeybind;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    public Camera GetPlayerCam() { return mainCamera; }
    private ScreenShake shake;
    public void StartShake() { if (shake) shake.start = true; }
    public void EndShake() { if (shake) shake.inProgress = false; }

    // Movement and Rotation
    private bool isFrozen;
    private Vector3 currentMovement;
    private float verticalRotation;

    // Input
    private Vector3 currentInputVector;
    private Vector3 inputVelocity;

    // Energy
    private FishEnergy energyComp;
    public FishEnergy GetEnergyComp() { return energyComp; }

    // Audio
    [SerializeField] private AudioClip[] eatSounds;
    [SerializeField] private AudioClip[] swimSounds;
    private bool isSwimAudioOnCooldown;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerController = this;
            if (GameManager.Instance.GetIsPerkUnlocked(0))
            {
                dashSpeed = 12f;
                Debug.Log("Dash is faster, need to set this up still!");
            }
        }

        // Initialize components
        energyComp = GetComponent<FishEnergy>();
        shake = mainCamera.GetComponent<ScreenShake>();

        // Setup
        currentSpeed = swimSpeed;
        startPos = mainCamera.transform.localPosition;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnHookedMinigameFinished += OnEndHookedMinigame;
    }

    private void Update()
    {
        if (InputManager.Instance == null) return;

        ShouldViewBob();
        HandleMovement();
        HandleRoation();
        ShouldTilt();

        if (InputManager.Instance.DashInput && !isDashOnCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    /* General Swimming Movement */
    private Vector3 CalculateWorldDirection()
    {
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

        if (currentMovement.magnitude > 1f && !isSwimAudioOnCooldown && !isDashing)
        {
            StartCoroutine(HandleSwimAudioCooldown());
            SoundFXManager.Instance.PlayRandomSoundFXClip(swimSounds, transform, 0.3f, 1f, 0f, 0.05f);
        }
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
        if (isFrozen) return;
        
        float mouseXRotation = InputManager.Instance.LookInput.x * InputManager.Instance.mouseSensitivity;
        float mouseYRotation = InputManager.Instance.LookInput.y * InputManager.Instance.mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    public void PauseEnergy(bool shouldPause)
    {
        energyComp.SetIsPaused(shouldPause);
    }

    public void PausePlayer()
    {
        isFrozen = true;
        energyComp.SetIsPaused(true);
        GameManager.Instance.DisableHUD();
        InputManager.Instance.isInputPaused = true;
    }


    private IEnumerator HandleSwimAudioCooldown()
    {
        isSwimAudioOnCooldown = true;
        yield return new WaitForSeconds(1.5f);

        isSwimAudioOnCooldown = false;
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
            if (!isFrozen) BeginHookedMinigame(other);
        }
    }

    private void EatPrey(Collider other)
    {
        //Debug.Log("We eating prey 2nite");
        float energyProg = other.GetComponent<PreyManager>().GetEnergyValue();
        SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, 1f, 1f, 0.2f, 0.1f);
        energyComp.AddProgress(energyProg);
        GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
    }

    private IEnumerator Death(Collider other)
    {
        SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, 1f, 1f, 0.2f, 0.1f);
        yield return new WaitForSeconds(1f);

        PausePlayer();
        GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        cameraAnim.Play("death", 0);
    }

    public IEnumerator DeathSimple()
    {
        PausePlayer();
        cameraAnim.Play("death", 0);
        yield break; 
    }

    private void BeginHookedMinigame(Collider other)
    {
        SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, 1f, 1f, 0.2f, 0.1f);
        energyComp.SetIsPaused(true);
        GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        Debug.Log("Fight for your life!");
        GameManager.Instance.StartHookedMinigame();
    }

    private void OnEndHookedMinigame()
    {
        currentSpeed = 12f;
        isFrozen = true;
        energyComp.AddProgress(-10f);
        energyComp.SetIsPaused(false);
        StartCoroutine(PostHookedDash());
    }

    private IEnumerator PostHookedDash()
    {
        float duration = 1f;
        float taperOffRate = (currentSpeed - swimSpeed) / duration; // subtract desired end speed

        while (currentSpeed > swimSpeed)
        {
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
            currentSpeed -= taperOffRate * Time.deltaTime;

            yield return null;
        }

        isFrozen = false;
        currentSpeed = swimSpeed;
    }

    /* Dashing */
    private IEnumerator Dash()
    {
        dashKeybind.alpha = 0.5f;

        float ogDepRate = energyComp.GetDepreciationRate();
        energyComp.OnDash();
        SoundFXManager.Instance.PlayRandomSoundFXClip(dashSounds, transform, 1f, 1f, 0.06f, 0.06f);

        isDashOnCooldown = true;
        isDashing = true;
        currentSpeed = 2f;
        
        while (currentSpeed < dashSpeed)
        {
            currentSpeed += dashChargeRate * Time.deltaTime;
            dashChargeBar.value = currentSpeed / dashSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        energyComp.SetDepreciationRate(ogDepRate);
        StartCoroutine(EndDash());
    }

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
        isDashOnCooldown = false;
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


