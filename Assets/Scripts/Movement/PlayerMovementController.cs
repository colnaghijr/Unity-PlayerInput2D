using UnityEngine;
// using UnityEngine.InputSystem; // Removed as InputManager handles direct input types

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D rigidBody;

    private InputManager inputManager;
    // private PlayerControls playerControls; // Removed, InputManager handles this
    // private InputAction moveAction; // Removed, InputManager handles this

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>(); 
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene!");
        }

        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        // Temporary initialization for moveAction and playerControls // Removed
        // playerControls = new PlayerControls(); // Removed
        // moveAction = playerControls.Player.Move; // Removed
        // playerControls.Player.Enable(); // Removed
    }

    void FixedUpdate()
    {
        if (inputManager == null || rigidBody == null)
        {
            Debug.LogError("InputManager or Rigidbody2D not assigned/found in PlayerMovementController.");
            return;
        }

        // Get move direction from InputManager
        Vector2 moveDirection = inputManager.MoveDirection;

        // Calculate and apply movement
        if (moveDirection.sqrMagnitude > 0.1f) // Check if there is significant input
        {
            Vector2 currentPosition = rigidBody.position;
            Vector2 movement = moveDirection * movementSpeed * Time.fixedDeltaTime;
            Vector2 newPosition = currentPosition + movement;

            rigidBody.MovePosition(newPosition);

            // Debug line drawing for movement
#if UNITY_EDITOR
            Debug.DrawLine(currentPosition, newPosition, Color.green);
#endif
        }
    }
}
