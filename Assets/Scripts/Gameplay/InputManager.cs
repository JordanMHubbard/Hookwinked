using UnityEngine.InputSystem;
using UnityEngine;
using System.Diagnostics;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public static PlayerInput PlayerInput;
    
    public enum ActionMap {Player, UI, HookedMinigame}
    private ActionMap currentMap = ActionMap.Player;

    // Player
    public Vector2 SwimInput { get; private set;}
    public bool SwimIsPressed { get; private set;}
    public bool DashInput { get; private set;}
    public Vector2 LookInput { get; private set;}
    public Vector2 FloatInput { get; private set;}
    public bool FloatIsPressed { get; private set;}
    public bool MenuOpenInput { get; private set;}

    // UI
    public bool UIMenuCloseInput { get; private set;}

    // Hooked Minigame
    public Vector2 ShakeInput { get; private set;}
    
    // Player Input
    private InputAction swimAction;
    private InputAction dashAction;
    private InputAction floatAction;
    private InputAction lookAction;
    private InputAction menuOpenAction;

    // UI Input
    private InputAction UIMenuCloseAction;

    // Hooked Minigame Input
    private InputAction shakeAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        PlayerInput = GetComponent<PlayerInput>();

        swimAction = PlayerInput.actions["Move"];
        lookAction = PlayerInput.actions["Look"];
        dashAction = PlayerInput.actions["Sprint"];
        floatAction = PlayerInput.actions["MoveVertical"];
        menuOpenAction =  PlayerInput.actions["MenuOpen"];
        UIMenuCloseAction =  PlayerInput.actions["MenuClose"];
        shakeAction = PlayerInput.actions["Shake"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        switch (currentMap)
        {
            case ActionMap.Player:
                HandlePlayerInput();
                break;
            
            case ActionMap.UI:
                HandleUIInput();
                break;
            
            case ActionMap.HookedMinigame:
                HandleHookedMinigameInput();
                break;
        }
    }

    private void HandlePlayerInput()
    {
        SwimInput = swimAction.ReadValue<Vector2>();
        SwimIsPressed = swimAction.IsPressed();
        LookInput = lookAction.ReadValue<Vector2>();
        DashInput = dashAction.IsPressed();
        FloatInput = floatAction.ReadValue<Vector2>();
        FloatIsPressed = floatAction.IsPressed();
        MenuOpenInput = menuOpenAction.WasPressedThisFrame();
    }

    private void HandleUIInput()
    {
        UIMenuCloseInput =  UIMenuCloseAction.WasPressedThisFrame();
    }

    private void HandleHookedMinigameInput()
    {
        ShakeInput = shakeAction.ReadValue<Vector2>();
    }

    public void SwitchCurrentMap(ActionMap map)
    {
        PlayerInput.SwitchCurrentActionMap(map.ToString());
        FlushInput();
        currentMap = map;
    }

    private void FlushInput()
    {
        // Player
        SwimInput = Vector2.zero;
        SwimIsPressed = false;
        LookInput = Vector2.zero;
        DashInput = false;
        FloatInput = Vector2.zero;
        FloatIsPressed = false;
        MenuOpenInput = false;
        // UI
        UIMenuCloseInput = false;
        // Hooked Minigame
        ShakeInput = Vector2.zero;
    }
}
