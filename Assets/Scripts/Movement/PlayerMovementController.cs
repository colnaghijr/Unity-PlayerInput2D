using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D rigidBody;

    private InputManager inputManager;

    void Start()
    {
        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<InputManager>();
            if (inputManager == null)
            {
                Debug.LogError("InputManager not found in the scene!");
            }
        }

        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }
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

#if UNITY_EDITOR
            Debug.DrawLine(currentPosition, newPosition, Color.green);
#endif
        }
    }
}
