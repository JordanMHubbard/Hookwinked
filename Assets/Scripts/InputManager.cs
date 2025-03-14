using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static PlayerInput PlayerInput;
    
    public Vector2 SwimInput { get; private set;}
    public bool SwimIsPressed { get; private set;}
    public bool DashInput { get; private set;}
    public Vector2 LookInput { get; private set;}
    public Vector2 FloatInput { get; private set;}
    public bool FloatIsPressed { get; private set;}
    public bool MenuOpenInput { get; private set;}
    
    private InputAction swimAction;
    private InputAction dashAction;
    private InputAction floatAction;
    private InputAction lookAction;
    private InputAction menuOpenAction;

    private void Awake()
    {
        if (instance == null) instance = this;

        PlayerInput = GetComponent<PlayerInput>();

        swimAction = PlayerInput.actions["Move"];
        lookAction = PlayerInput.actions["Look"];
        dashAction = PlayerInput.actions["Attack"];
        floatAction = PlayerInput.actions["MoveVertical"];
    }

    private void Update()
    {

        SwimInput = swimAction.ReadValue<Vector2>();
        SwimIsPressed = swimAction.IsPressed();
        LookInput = lookAction.ReadValue<Vector2>();
        DashInput = dashAction.IsPressed();
        FloatInput = floatAction.ReadValue<Vector2>();
        FloatIsPressed = floatAction.IsPressed();
        //MenuOpenInput = menuOpenAction.WasPressedThisFrame();
    }
}
