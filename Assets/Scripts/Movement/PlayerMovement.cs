using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("References")]
    [SerializeField] private InputVisualizerGizmos inputVisualizer;

    private Vector3 currentMoveInput;
    private Vector3 smoothedMovement;
    private Vector3 lastInputDirection;

    private void Start()
    {
        if (inputVisualizer == null)
            inputVisualizer = FindObjectOfType<InputVisualizerGizmos>();
    }

    private void Update()
    {
        HandleMovement();
        MoveCharacter();
    }


    private void HandleMovement()
    {
        if (inputVisualizer == null) return;

        // Get camera-relative input (already processed by InputVisualizerGizmos)
        Vector3 cameraRelativeInput = inputVisualizer.cameraRelativeInput;

        Vector3 moveDirection;

        // Check if there's active input
        if (cameraRelativeInput.magnitude > 0.1f)
        {
            // Use current input and save it as last direction
            moveDirection = cameraRelativeInput;
            lastInputDirection = cameraRelativeInput.normalized;
        }
        else
        {
            // No input, don't move but keep the last direction for reference
            moveDirection = Vector3.zero;
        }

        // Apply movement speed
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Smooth the movement for better feel
        float smoothTime = targetVelocity.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration;
        smoothedMovement = Vector3.Lerp(smoothedMovement, targetVelocity, Time.deltaTime / smoothTime);

        currentMoveInput = smoothedMovement;
    }


    private void MoveCharacter()
    {
        // Move only on horizontal plane using Transform
        transform.position += currentMoveInput * Time.deltaTime;
    }

    // Public getters for debugging
    public Vector3 GetCurrentVelocity() => currentMoveInput;
    public Vector3 GetSmoothedMovement() => smoothedMovement;
    public Vector3 GetLastInputDirection() => lastInputDirection;

    private void OnDrawGizmosSelected()
    {
        // Draw movement vectors
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, currentMoveInput);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position + Vector3.up * 1.2f, smoothedMovement);

            // Draw last input direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + Vector3.up * 1.4f, lastInputDirection * 2f);
        }
    }
}