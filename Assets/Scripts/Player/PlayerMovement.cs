using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("NavMesh Validation Settings")]
    [SerializeField] private bool useNavMeshValidation = true;
    [SerializeField] private float navMeshSampleDistance = 1f;
    [SerializeField] private float maxNavMeshDistance = 2f;
    [SerializeField] private float playerHeightOffset = 1f;

    [Header("Debug Visualization")]
    [SerializeField] private float gizmoLookAheadTime = 0.5f;

    [Header("References")]
    [SerializeField] private InputVisualizerGizmos inputVisualizer;


    private Vector3 currentMoveInput;
    private Vector3 smoothedMovement;
    private Vector3 lastInputDirection;
    private Vector3 lastValidPosition;

    private void Start()
    {
        if (inputVisualizer == null)
            inputVisualizer = FindObjectOfType<InputVisualizerGizmos>();

        // Initialize last valid position
        lastValidPosition = transform.position;

        // Validate initial position if using NavMesh validation
        if (useNavMeshValidation)
        {
            NavMeshHit hit;
            Vector3 checkPosition = new Vector3(transform.position.x, transform.position.y - playerHeightOffset, transform.position.z);

            if (NavMesh.SamplePosition(checkPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                // Position player at correct height above NavMesh
                Vector3 correctedPosition = new Vector3(hit.position.x, hit.position.y + playerHeightOffset, hit.position.z);
                transform.position = correctedPosition;
                lastValidPosition = correctedPosition;
            }
            else
            {
                Debug.LogWarning("PlayerMovement: Starting position is not on NavMesh! Make sure there's a baked NavMesh in the scene or disable NavMesh validation.");
                lastValidPosition = transform.position;
            }
        }
    }

    private void Update()
    {
        HandleMovement();
        MoveCharacter();
    }


    private void HandleMovement()
    {
        Vector3 cameraRelativeInput;

        // Use InputVisualizer for input
        if (inputVisualizer != null)
        {
            cameraRelativeInput = inputVisualizer.cameraRelativeInput;
        }
        else
        {
            cameraRelativeInput = Vector3.zero;
        }

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
        float smoothSpeed = targetVelocity.magnitude > 0.1f ? acceleration : deceleration;
        smoothedMovement = Vector3.MoveTowards(smoothedMovement, targetVelocity, smoothSpeed * Time.deltaTime);

        currentMoveInput = smoothedMovement;
    }


    private void MoveCharacter()
    {
        if (currentMoveInput.magnitude < 0.01f) return;

        // Calculate desired position
        Vector3 desiredPosition = transform.position + currentMoveInput * Time.deltaTime;

        // Validate position with NavMesh if enabled
        if (useNavMeshValidation)
        {
            Vector3 validatedPosition = ValidatePositionWithNavMesh(desiredPosition);
            transform.position = validatedPosition;
        }
        else
        {
            // Move directly without validation
            transform.position = desiredPosition;
        }

        // Update last valid position
        lastValidPosition = transform.position;
    }

    private Vector3 ValidatePositionWithNavMesh(Vector3 desiredPosition)
    {
        NavMeshHit hit;

        // Create a position on the NavMesh plane (without the height offset)
        Vector3 navMeshCheckPosition = new Vector3(desiredPosition.x, desiredPosition.y - playerHeightOffset, desiredPosition.z);

        // Simple validation: check if desired position is on NavMesh
        if (NavMesh.SamplePosition(navMeshCheckPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            // Check if it's not too far from where we want to go (in XZ plane)
            Vector3 desiredXZ = new Vector3(desiredPosition.x, 0, desiredPosition.z);
            Vector3 hitXZ = new Vector3(hit.position.x, 0, hit.position.z);
            float distance = Vector3.Distance(desiredXZ, hitXZ);

            if (distance <= 0.5f) // Conservative threshold
            {
                // Return the hit position with the player height offset applied
                return new Vector3(hit.position.x, hit.position.y + playerHeightOffset, hit.position.z);
            }
        }

        // If position is invalid, stay at current position
        return transform.position;
    }

    // Public getters for debugging
    public Vector3 GetCurrentVelocity() => currentMoveInput;
    public Vector3 GetSmoothedMovement() => smoothedMovement;
    public Vector3 GetLastInputDirection() => lastInputDirection;
    public Vector3 GetLastValidPosition() => lastValidPosition;
    public bool IsUsingNavMeshValidation() => useNavMeshValidation;
    public float GetPlayerHeightOffset() => playerHeightOffset;

    // Public method to adjust player to NavMesh surface height
    public void SnapToNavMeshSurface()
    {
        if (!useNavMeshValidation) return;

        NavMeshHit hit;
        Vector3 checkPosition = new Vector3(transform.position.x, transform.position.y - playerHeightOffset, transform.position.z);

        if (NavMesh.SamplePosition(checkPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            Vector3 correctedPosition = new Vector3(hit.position.x, hit.position.y + playerHeightOffset, hit.position.z);
            transform.position = correctedPosition;
            lastValidPosition = correctedPosition;
        }
    }


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

            // Draw NavMesh validation gizmos
            if (useNavMeshValidation)
            {
                // Get feet position for NavMesh gizmos
                Vector3 feetPosition = GetFeetPosition();

                // Only draw last valid position if it's different from current position
                float distanceToLastValid = Vector3.Distance(transform.position, lastValidPosition);
                if (distanceToLastValid > 0.5f)
                {
                    // Draw last valid position (bright green)
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z), 0.3f);

                    // Draw a filled sphere to make it more visible
                    Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Green with transparency
                    Gizmos.DrawSphere(new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z), 0.2f);

                    // Draw label only when sphere is visible
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(new Vector3(lastValidPosition.x, feetPosition.y + 0.7f, lastValidPosition.z), "Last Valid Position");
                    #endif
                }

                // Draw line from current position to last valid position (if different)
                if (Vector3.Distance(transform.position, lastValidPosition) > 0.3f)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(feetPosition, new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z));
                }

                // Draw projected movement validation
                if (currentMoveInput.magnitude > 0.1f)
                {
                    // Use input direction for better visualization (not the smoothed movement)
                    Vector3 currentInputDirection = Vector3.zero;
                    if (inputVisualizer != null)
                    {
                        currentInputDirection = inputVisualizer.cameraRelativeInput;
                    }

                    if (currentInputDirection.magnitude > 0.1f)
                    {
                        Vector3 inputDirection = currentInputDirection.normalized;
                        Vector3 desiredPos = transform.position + inputDirection * gizmoLookAheadTime * moveSpeed;

                        NavMeshHit hit;
                        bool isValidPosition = NavMesh.SamplePosition(desiredPos, out hit, navMeshSampleDistance, NavMesh.AllAreas);

                        // Use same height as input cubes (playerPosition + 0.1f up)
                        float cubeHeight = transform.position.y + 0.1f;

                        if (isValidPosition)
                        {
                            // Check if the hit position is close enough to desired position
                            float distance = Vector3.Distance(desiredPos, hit.position);
                            if (distance <= 1f) // Reasonable threshold
                            {
                                // GREEN CUBE: Valid NavMesh position
                                Gizmos.color = Color.green;
                                Gizmos.DrawWireCube(new Vector3(hit.position.x, cubeHeight, hit.position.z), Vector3.one * 0.3f);

                                // Draw line to show the projection
                                Gizmos.color = Color.green;
                                Gizmos.DrawLine(new Vector3(transform.position.x, cubeHeight, transform.position.z),
                                              new Vector3(hit.position.x, cubeHeight, hit.position.z));
                            }
                            else
                            {
                                // RED CUBE: NavMesh found but too far from desired direction
                                Gizmos.color = Color.red;
                                Gizmos.DrawWireCube(new Vector3(desiredPos.x, cubeHeight, desiredPos.z), Vector3.one * 0.3f);

                                // Draw line to show the invalid projection
                                Gizmos.color = Color.red;
                                Gizmos.DrawLine(new Vector3(transform.position.x, cubeHeight, transform.position.z),
                                              new Vector3(desiredPos.x, cubeHeight, desiredPos.z));
                            }
                        }
                        else
                        {
                            // RED CUBE: No NavMesh found at all
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireCube(new Vector3(desiredPos.x, cubeHeight, desiredPos.z), Vector3.one * 0.3f);

                            // Draw line to show the invalid projection
                            Gizmos.color = Color.red;
                            Gizmos.DrawLine(new Vector3(transform.position.x, cubeHeight, transform.position.z),
                                          new Vector3(desiredPos.x, cubeHeight, desiredPos.z));
                        }
                    }
                }
            }
        }
    }

    private Vector3 GetFeetPosition()
    {
        // Try to get collider bounds first
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            return new Vector3(transform.position.x, col.bounds.min.y, transform.position.z);
        }

        // Try to get renderer bounds
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            return new Vector3(transform.position.x, renderer.bounds.min.y, transform.position.z);
        }

        // Fallback: assume standard character controller height offset
        return transform.position + Vector3.down * 1f;
    }
}