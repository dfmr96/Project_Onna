using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float stoppingDistance = 0.1f;

    [Header("NavMesh Settings")]
    [SerializeField] private bool useNavMesh = true;
    [SerializeField] private float navMeshSampleDistance = 1f;
    [SerializeField] private float lookAheadDistance = 2f;

    [Header("References")]
    [SerializeField] private InputVisualizerGizmos inputVisualizer;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private Vector3 currentMoveInput;
    private Vector3 smoothedMovement;
    private Vector3 lastInputDirection;
    private Vector3 targetPosition;

    private void Start()
    {
        if (inputVisualizer == null)
            inputVisualizer = FindObjectOfType<InputVisualizerGizmos>();

        // Initialize NavMeshAgent
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent != null && useNavMesh)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.acceleration = acceleration;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.updateRotation = false; // We'll handle rotation separately if needed

            // Ensure the agent is enabled and on the NavMesh
            navMeshAgent.enabled = true;

            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("PlayerMovement: NavMeshAgent is not on NavMesh! Make sure there's a baked NavMesh in the scene.");
            }
        }

        targetPosition = transform.position;
    }

    private void Update()
    {
        HandleMovement();

        if (useNavMesh && navMeshAgent != null)
        {
            MoveCharacterNavMesh();
        }
        else
        {
            MoveCharacter();
        }
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

            // For NavMesh, calculate target position based on input direction
            if (useNavMesh && navMeshAgent != null)
            {
                // Calculate target position further ahead for better NavMesh movement
                Vector3 desiredPosition = transform.position + moveDirection.normalized * lookAheadDistance;

                // Sample the NavMesh to find a valid position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(desiredPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                }
                else
                {
                    // Fallback: try closer position if far position fails
                    desiredPosition = transform.position + moveDirection.normalized * 0.5f;
                    if (NavMesh.SamplePosition(desiredPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                    {
                        targetPosition = hit.position;
                    }
                }
            }
        }
        else
        {
            // No input, don't move but keep the last direction for reference
            moveDirection = Vector3.zero;

            if (useNavMesh && navMeshAgent != null)
            {
                targetPosition = transform.position; // Stop at current position
            }
        }

        // For non-NavMesh movement, keep the original smooth movement calculation
        if (!useNavMesh || navMeshAgent == null)
        {
            // Apply movement speed
            Vector3 targetVelocity = moveDirection * moveSpeed;

            // Smooth the movement for better feel
            float smoothTime = targetVelocity.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration;
            smoothedMovement = Vector3.Lerp(smoothedMovement, targetVelocity, Time.deltaTime / smoothTime);

            currentMoveInput = smoothedMovement;
        }
        else
        {
            // For NavMesh, update currentMoveInput based on actual movement
            currentMoveInput = moveDirection * moveSpeed;
        }
    }


    private void MoveCharacter()
    {
        // Move only on horizontal plane using Transform
        transform.position += currentMoveInput * Time.deltaTime;
    }

    private void MoveCharacterNavMesh()
    {
        if (navMeshAgent == null) return;

        // Update speed settings in case they changed
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.acceleration = acceleration;

        // Check if we have input to move
        if (inputVisualizer != null && inputVisualizer.cameraRelativeInput.magnitude > 0.1f)
        {
            // Set the destination for the NavMeshAgent
            if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
            {
                navMeshAgent.SetDestination(targetPosition);
            }
        }
        else
        {
            // No input, stop the agent
            navMeshAgent.ResetPath();
        }

        // Update currentMoveInput for debug visualization
        if (navMeshAgent.hasPath && navMeshAgent.velocity.magnitude > 0.1f)
        {
            currentMoveInput = navMeshAgent.velocity;
        }
        else
        {
            currentMoveInput = Vector3.zero;
        }
    }

    // Public getters for debugging
    public Vector3 GetCurrentVelocity() => currentMoveInput;
    public Vector3 GetSmoothedMovement() => smoothedMovement;
    public Vector3 GetLastInputDirection() => lastInputDirection;
    public Vector3 GetTargetPosition() => targetPosition;
    public bool IsUsingNavMesh() => useNavMesh && navMeshAgent != null;

    private void OnDrawGizmosSelected()
    {
        // Draw movement vectors
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, currentMoveInput);

            if (!useNavMesh || navMeshAgent == null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(transform.position + Vector3.up * 1.2f, smoothedMovement);
            }

            // Draw last input direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + Vector3.up * 1.4f, lastInputDirection * 2f);

            // Draw NavMesh specific gizmos
            if (useNavMesh && navMeshAgent != null)
            {
                // Get feet position for NavMesh gizmos only
                Vector3 feetPosition = GetFeetPosition();

                // Draw target position at ground level
                Vector3 targetFeet = new Vector3(targetPosition.x, feetPosition.y, targetPosition.z);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(targetFeet, 0.3f);

                // Draw line to target from feet
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(feetPosition, targetFeet);

                // Draw NavMesh path if available
                if (navMeshAgent.hasPath)
                {
                    Gizmos.color = Color.green;
                    Vector3[] path = navMeshAgent.path.corners;
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        Vector3 pathPoint1 = new Vector3(path[i].x, feetPosition.y, path[i].z);
                        Vector3 pathPoint2 = new Vector3(path[i + 1].x, feetPosition.y, path[i + 1].z);
                        Gizmos.DrawLine(pathPoint1, pathPoint2);
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