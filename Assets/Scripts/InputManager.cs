using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public Vector2 SwimInput { get; private set;}
    public bool swimIsPressed { get; private set;}
    public bool dashInput { get; private set;}
    public Vector2 lookInput { get; private set;}
    public Vector2 floatInput { get; private set;}
    public bool floatIsPressed { get; private set;}
    public bool MenuOpenInput { get; private set;}

    public static PlayerInput playerInput;
    
    private InputAction swimAction;
    private InputAction dashAction;
    private InputAction floatAction;
    private InputAction lookAction;
    private InputAction menuOpenAction;

    private void Awake()
    {
        if (instance == null) instance = this;

        playerInput = GetComponent<PlayerInput>();

        swimAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        dashAction = playerInput.actions["Attack"];
        floatAction = playerInput.actions["MoveVertical"];
    }

    private void Update()
    {
        SwimInput = swimAction.ReadValue<Vector2>();
        swimIsPressed = swimAction.WasPressedThisFrame();
        lookInput = lookAction.ReadValue<Vector2>();
        dashInput = dashAction.WasPressedThisFrame();
        floatInput = floatAction.ReadValue<Vector2>();
        floatIsPressed = floatAction.WasPressedThisFrame();
        MenuOpenInput = menuOpenAction.WasPressedThisFrame();
    }
}
