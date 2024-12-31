using System;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.InputSystem;

public class aimController : MonoBehaviour
{
    // Visual, Game Objects
    public GameObject aim; // must be a child GO; if external, vector calculations need to take it into consideration
    public static Camera mainCamera;

    // Gizmos
    string debug_text;

    // Physics for movement
    [SerializeField] float movementSpeed;
    [SerializeField] Rigidbody2D rigidBody;

    // Input control
    [SerializeField] InputActionAsset playerControls;
    InputAction moveAction;
    InputAction lookAction;
    bool useGamePad = false;
    Vector2 lastLookDirection; // to maintain target if gamepad is let go and player is moving
    InputDevice lastInputDevice;

    const float aimRadius = 5.0f;
    const string Player = "Player";
    const string Move = "Move";
    const string Look = "Look";
    void Start()
    {
        moveAction = playerControls.FindActionMap(nameof(Player)).FindAction(nameof(Move));
        lookAction = playerControls.FindActionMap(nameof(Player)).FindAction(nameof(Look));
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public static Vector3 getMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        var screenToWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        screenToWorldPosition.z = 0f; //always ignore Z for tilemaps ZasY

        return screenToWorldPosition;
    }

    void Update()
    {
        CheckPlayerLookTarget();
        CheckPlayerMoveTarget();
    }

    void CheckPlayerMoveTarget()
    {
        Vector2 sourcePosition = gameObject.transform.position;
        var moveDirection = moveAction.ReadValue<Vector2>();

        CheckInputDevice();

        // mouse clicking takes precedence and disables gamepad controls
        if (Input.GetMouseButtonDown(0))
        {
            useGamePad = false;
            debug_text = "keyboard";
        }

        // move character at whatever speed or forces needed
        Vector2 movement = moveDirection * movementSpeed;
        Vector2 newPos = rigidBody.position + movement * Time.fixedDeltaTime;
        rigidBody.MovePosition(newPos);

        // visual gizmo that takes shows input intensity (range of movement)
        Debug.DrawLine(sourcePosition, rigidBody.position + movement, Color.green);
    }

    void CheckPlayerLookTarget()
    {
        Vector2 sourcePosition = rigidBody.position;
        var lookDirection = lookAction.ReadValue<Vector2>();

        CheckInputDevice();

        // mouse clicking takes precedence and disables gamepad controls
        if (Input.GetMouseButtonDown(0))
        {
            useGamePad = false;
            debug_text = "keyboard";
        }

        // ignore input less than a minimum threshold to avoid 
        // aim jerking around as input gets closer to zero
        if (Math.Abs(lookDirection.x) < 0.1 && Math.Abs(lookDirection.y) < 0.1)
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
        if (useGamePad)
        {
            targetPosition = rigidBody.position + lookDirection;
        }
        else
        {
            targetPosition = getMouseWorldPosition();
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

    void CheckInputDevice()
    {
        if (moveAction.activeControl != null && moveAction.activeControl.device != lastInputDevice)
        {
            // keep track of last input type
            lastInputDevice = moveAction.activeControl.device;
            if (moveAction.activeControl?.device is Keyboard)
            {
                useGamePad = false;
                debug_text = "keyboard";
            }
            else
            {
                useGamePad = true;
                debug_text = "gamepad";
            }
        }
    }

    virtual public void OnDrawGizmos()
    {
        var c = gameObject.transform;
        float x = c.position.x - 1;
        float y = c.position.y - 1;
        float z = c.position.z;
        var textPosition = new Vector3(x, y, z);

        var logString = string.Format("{0}", debug_text);
        Handles.Label(textPosition, logString);
    }
}
