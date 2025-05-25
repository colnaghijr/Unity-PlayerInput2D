using System;
using UnityEngine;
// using UnityEditor.UI; // Removed
// using UnityEngine.UIElements; // Removed
// using UnityEngine.InputSystem; // Removed as InputManager handles direct input types

#if UNITY_EDITOR
using UnityEditor; // Moved inside UNITY_EDITOR block
#endif

public class aimController : MonoBehaviour
{
    // Visual, Game Objects
    [Tooltip("The GameObject representing the aim indicator. Must be a child or properly offset.")]
    public GameObject aim; 
    // public static Camera mainCamera; // Removed, GameUtils.GetMouseWorldPosition will use Camera.main

    // Gizmos
    // string debug_text; // Will be removed, InputManager can provide debug info if needed

    // Physics for movement
    private Rigidbody2D rigidBody; 

    // Input control
    private InputManager inputManager;
    Vector2 lastLookDirection; // to maintain target if gamepad is let go and player is moving

    const float aimRadius = 5.0f; // This is already a const, which is fine.
    private const float LookInputThreshold = 0.1f; // Defined constant
    
    // const string Player = "Player"; // Not strictly needed here anymore
    // const string Look = "Look"; // Not strictly needed here anymore
    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene!");
        }

        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogError("Rigidbody2D not found on the same GameObject as aimController!");
        }
        
        // lookAction = playerControls.FindActionMap(nameof(Player)).FindAction(nameof(Look)); // Moved to InputManager
        // if (mainCamera == null) mainCamera = Camera.main; // Removed
    }

    // public static Vector3 getMouseWorldPosition() // Moved to GameUtils.cs
    // {
    //     var mousePosition = Input.mousePosition;
    //     var screenToWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
    //     screenToWorldPosition.z = 0f; //always ignore Z for tilemaps ZasY
    //
    //     return screenToWorldPosition;
    // }

    void Update()
    {
        CheckPlayerLookTarget();
    }

    void CheckPlayerLookTarget()
    {
        if (inputManager == null || rigidBody == null) 
        {
            // Safety check to prevent errors if InputManager or Rigidbody2D is not found
            return;
        }

        Vector2 sourcePosition = rigidBody.position;
        var lookDirection = inputManager.LookDirection; // Get from InputManager

        // CheckInputDevice(); // Removed, InputManager handles this

        // mouse clicking takes precedence and disables gamepad controls
        // if (Input.GetMouseButtonDown(0)) // Removed, InputManager handles this
        // {
            // useGamePad = false; // Handled by InputManager
            // debug_text = "keyboard"; // Handled by InputManager or can be derived from inputManager.IsGamepadUsed
        // }

        // ignore input less than a minimum threshold to avoid 
        // aim jerking around as input gets closer to zero
        if (Math.Abs(lookDirection.x) < LookInputThreshold && Math.Abs(lookDirection.y) < LookInputThreshold) // Used constant
        {
            // just use last known good direction and keep it stationary
            lookDirection = lastLookDirection;
        }
        else
        {
            // update last known valid direction
            lastLookDirection = lookDirection;
        }

        Vector2 targetPosition;
        if (inputManager.IsGamepadUsed) // Get from InputManager
        {
            targetPosition = rigidBody.position + lookDirection;
        }
        else
        {
            targetPosition = GameUtils.GetMouseWorldPosition(); // Updated call
        }

        // visual gizmo that takes shows input intensity (range of movement)
        Debug.DrawLine(sourcePosition, targetPosition, Color.red);

        var relativeAngleToTarget = Mathf.Atan2(targetPosition.y - sourcePosition.y, targetPosition.x - sourcePosition.x);

        float x = (float)(aimRadius * Math.Cos(relativeAngleToTarget));
        float y = (float)(aimRadius * Math.Sin(relativeAngleToTarget));

        // move character at whatever speed or forces needed
        Vector2 newPos = new Vector2(x, y) + rigidBody.position;

        aim.transform.position = newPos;
        aim.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * relativeAngleToTarget) - 90);
    }

    // void CheckInputDevice() // Removed, logic moved to InputManager.cs
    // {
    //     // ...
    // }

#if UNITY_EDITOR
    virtual public void OnDrawGizmos()
    {
        // Example of how you might display info from InputManager if it's available
        if (Application.isPlaying && inputManager != null)
        {
            string deviceType = inputManager.IsGamepadUsed ? "Gamepad" : "Keyboard/Mouse";
            Handles.Label(transform.position + Vector3.down * 1.5f, $"Input: {deviceType}");
        }
        else if (!Application.isPlaying)
        {
            Handles.Label(transform.position + Vector3.down * 1.5f, "InputManager info available at runtime.");
        }

        // Keep existing gizmo logic for aim visualization if needed,
        // for example, showing the aimRadius if it were dynamic.
        // Handles.DrawWireDisc(transform.position, Vector3.forward, aimRadius);
    }
#endif
}
