using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // Moved inside UNITY_EDITOR block
#endif

public class aimController : MonoBehaviour
{
    // Visual, Game Objects
    [Tooltip("The GameObject representing the aim indicator. Must be a child or properly offset.")]
    public GameObject aim; 

    // Physics for movement
    private Rigidbody2D rigidBody; 

    // Input control
    private InputManager inputManager;
    Vector2 lastLookDirection; // to maintain target if gamepad is let go and player is moving

    const float aimRadius = 5.0f;
    private const float LookInputThreshold = 0.1f;
    
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
    }

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

        // ignore input less than a minimum threshold to avoid 
        // aim jerking around as input gets closer to zero
        if (Math.Abs(lookDirection.x) < LookInputThreshold && Math.Abs(lookDirection.y) < LookInputThreshold)
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

#if UNITY_EDITOR
    virtual public void OnDrawGizmos()
    {
        if (Application.isPlaying && inputManager != null)
        {
            string deviceType = inputManager.IsGamepadUsed ? "Gamepad" : "Keyboard/Mouse";
            Handles.Label(transform.position + Vector3.down * 1.5f, $"Input: {deviceType}");
        }
        else if (!Application.isPlaying)
        {
            Handles.Label(transform.position + Vector3.down * 1.5f, "InputManager info available at runtime.");
        }
    }
#endif
}
