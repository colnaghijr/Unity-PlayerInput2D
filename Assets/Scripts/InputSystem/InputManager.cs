using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] InputActionAsset playerControls;

    private InputAction moveAction;
    private InputAction lookAction;

    private InputDevice lastInputDevice;
    private bool useGamePad = false;

    // Public properties to access input data
    public Vector2 MoveDirection => moveAction.ReadValue<Vector2>();
    public Vector2 LookDirection => lookAction.ReadValue<Vector2>();
    public bool IsGamepadUsed => useGamePad;
    public InputDevice ActiveDevice => lastInputDevice; // Optional

    void Awake()
    {
        if (playerControls == null)
        {
            Debug.LogError("PlayerControls InputActionAsset not assigned in InputManager.");
            return;
        }

        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        lookAction = playerControls.FindActionMap("Player").FindAction("Look");

        if (moveAction == null) Debug.LogError("Move action not found!");
        if (lookAction == null) Debug.LogError("Look action not found!");
        
        moveAction.Enable();
        lookAction.Enable();
    }

    void Update()
    {
        PollInputDevice();
    }

    private void PollInputDevice()
    {
        // Check for mouse click first, as it takes precedence
        if (Input.GetMouseButtonDown(0))
        {
            useGamePad = false;
            lastInputDevice = Mouse.current ?? lastInputDevice; // Update last device to mouse
            return; // Exit early as mouse click overrides other checks for this frame
        }

        InputDevice currentDevice = null;
        bool deviceChanged = false;

        // Check moveAction's active control
        if (moveAction.activeControl != null)
        {
            currentDevice = moveAction.activeControl.device;
        }
        // Fallback to lookAction's active control if moveAction is not active
        else if (lookAction.activeControl != null)
        {
            currentDevice = lookAction.activeControl.device;
        }

        if (currentDevice != null && currentDevice != lastInputDevice)
        {
            lastInputDevice = currentDevice;
            deviceChanged = true;
        }

        if (deviceChanged)
        {
            if (lastInputDevice is Gamepad)
            {
                useGamePad = true;
            }
            else if (lastInputDevice is Keyboard || lastInputDevice is Mouse)
            {
                useGamePad = false;
            }
            // Add more device checks if necessary (e.g., Joystick)
        }
    }
}
