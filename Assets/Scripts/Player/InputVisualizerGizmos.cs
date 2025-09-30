using System;
using Player;
using UnityEngine;
using VContainer;

public class InputVisualizerGizmos : MonoBehaviour
{
    [Header("Gizmos Settings")]
    [SerializeField] private bool showInputGizmos = true;
    [SerializeField] private float gizmosScale = 2f;
    [SerializeField] private Color inputColor = Color.green;
    [SerializeField] private Color noInputColor = Color.gray;

    [Header("Input Service Reference")]
    private Services.IInputService inputService;

    [Header("Aim Target Reference")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private bool showAngleGizmos = true;
    [SerializeField] private Color angleColor = Color.cyan;

    [Header("Move Target")]
    [SerializeField] private Transform moveTarget;
    [SerializeField] private bool updateMoveTarget = true;

    [Header("NavMesh Integration")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool showNavMeshTarget = true;
    [SerializeField] private Color navMeshTargetColor = Color.red;

    // Public getters for other scripts (delegating to input service)
    public Vector2 GetCurrentInput() => inputService?.GetCurrentInput() ?? Vector2.zero;
    public Vector3 cameraRelativeInput => inputService?.GetCameraRelativeInput() ?? Vector3.zero;

    private void Start()
    {
        // Get input service from VContainer
        var lifetimeScope = GetComponentInParent<VContainer.Unity.LifetimeScope>();
        if (lifetimeScope != null)
        {
            inputService = lifetimeScope.Container.Resolve<Services.IInputService>();
        }
        else
        {
            Debug.LogWarning("InputVisualizerGizmos: No LifetimeScope found! Gizmos will not work properly.");
        }

        // Auto-assign aim target if not set
        if (aimTarget == null)
        {
            MouseGroundAiming mouseAiming = FindObjectOfType<MouseGroundAiming>();
            if (mouseAiming != null)
                aimTarget = mouseAiming.aimTarget;
        }

        // Auto-assign player movement if not set
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Input processing is now handled by CameraRelativeInputProcessor
        // This class only handles visualization
    }

    void OnDrawGizmos()
    {
        if (!showInputGizmos) return;

        Vector3 playerPosition = transform.position;
        Vector3 gizmosOffset = Vector3.up * 0.1f; // Slightly above ground

        Vector2 currentInput = GetCurrentInput();
        Vector3 cameraRelativeInputVector = cameraRelativeInput;

        // Draw input direction arrow
        if (currentInput.magnitude > 0.1f)
        {
            Gizmos.color = inputColor;
            Vector3 inputDirection = cameraRelativeInput * gizmosScale;
            Vector3 arrowEnd = playerPosition + gizmosOffset + inputDirection;

            // Draw main arrow line
            Gizmos.DrawLine(playerPosition + gizmosOffset, arrowEnd);

            // Draw arrow head
            Vector3 arrowHeadSize = inputDirection.normalized * 0.3f;
            Vector3 arrowLeft = Quaternion.Euler(0, 45, 0) * -arrowHeadSize;
            Vector3 arrowRight = Quaternion.Euler(0, -45, 0) * -arrowHeadSize;

            Gizmos.DrawLine(arrowEnd, arrowEnd + arrowLeft);
            Gizmos.DrawLine(arrowEnd, arrowEnd + arrowRight);

            // Draw input magnitude circle
            Gizmos.color = Color.Lerp(noInputColor, inputColor, currentInput.magnitude);
            DrawWireCircle(playerPosition + gizmosOffset, currentInput.magnitude * gizmosScale, Vector3.up);
        }
        else
        {
            // Draw neutral state
            Gizmos.color = noInputColor;
            DrawWireCircle(playerPosition + gizmosOffset, 0.5f, Vector3.up);
        }

        // Draw input indicators based on current input
        if (inputService != null)
        {
            DrawInputIndicators(playerPosition, gizmosOffset, currentInput);
        }

        // Draw angle between aim target and input direction
        DrawAngleBetweenInputAndAim(playerPosition, gizmosOffset);

        // Draw move target indicator
        DrawMoveTargetGizmo(playerPosition, gizmosOffset);

        // Draw NavMesh target if using NavMesh
        // NOTE: Disabled to avoid duplicate with PlayerMovement gizmos
        // DrawNavMeshTargetGizmo(playerPosition, gizmosOffset);
    }

    private void DrawAngleBetweenInputAndAim(Vector3 playerPosition, Vector3 gizmosOffset)
    {
        if (!showAngleGizmos || aimTarget == null || GetCurrentInput().magnitude < 0.1f) return;

        Vector3 playerPos = playerPosition + gizmosOffset;
        Vector3 aimDirection = (aimTarget.position - playerPos);
        aimDirection.y = 0; // Project to horizontal plane
        aimDirection.Normalize();

        Vector3 inputDirection = cameraRelativeInput.normalized;

        if (aimDirection.magnitude < 0.1f || inputDirection.magnitude < 0.1f) return;

        // Calculate angle between vectors
        float angle = Vector3.SignedAngle(inputDirection, aimDirection, Vector3.up);
        float absoluteAngle = Mathf.Abs(angle);

        // Draw aim target direction line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPos, playerPos + aimDirection * gizmosScale * 0.8f);

        // Draw input direction line (already drawn above, but we'll draw a thicker one for comparison)
        Gizmos.color = inputColor;
        Gizmos.DrawLine(playerPos, playerPos + inputDirection * gizmosScale * 0.8f);

        // Draw angle arc
        Gizmos.color = angleColor;
        DrawAngleArc(playerPos, inputDirection, aimDirection, angle, gizmosScale * 0.6f);

        // Draw angle text
        #if UNITY_EDITOR
        Vector3 midDirection = Vector3.Slerp(inputDirection, aimDirection, 0.5f);
        Vector3 textPosition = playerPos + midDirection * (gizmosScale * 0.4f) + Vector3.up * 0.2f;
        UnityEditor.Handles.Label(textPosition, $"{absoluteAngle:F0}°");
        #endif
    }

    private void DrawAngleArc(Vector3 center, Vector3 from, Vector3 to, float angle, float radius)
    {
        int segments = Mathf.Max(8, Mathf.RoundToInt(Mathf.Abs(angle) / 10f));
        Vector3 lastPoint = center + from * radius;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 direction = Vector3.Slerp(from, to, t);
            Vector3 currentPoint = center + direction * radius;

            Gizmos.DrawLine(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }

        // Draw small indicators at the ends of the arc
        Vector3 fromEnd = center + from * radius;
        Vector3 toEnd = center + to * radius;
        Gizmos.DrawWireSphere(fromEnd, 0.05f);
        Gizmos.DrawWireSphere(toEnd, 0.05f);
    }

    private void DrawMoveTargetGizmo(Vector3 playerPosition, Vector3 gizmosOffset)
    {
        if (moveTarget == null) return;

        // Draw connection line from player to move target
        Gizmos.color = Color.magenta;
        Vector3 playerPos = playerPosition + gizmosOffset;
        Gizmos.DrawLine(playerPos, moveTarget.position);

        // Draw move target indicator
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(moveTarget.position, 0.3f);

        // Draw filled sphere if there's active input
        if (GetCurrentInput().magnitude > 0.1f)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(moveTarget.position, 0.15f);
        }

        // Draw label
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(moveTarget.position + Vector3.up * 0.5f, "Move Target");
        #endif
    }

    private void DrawInputIndicators(Vector3 playerPosition, Vector3 gizmosOffset, Vector2 currentInput)
    {
        // Draw directional input indicators
        Vector3 forward = cameraRelativeInput.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        if (forward.magnitude > 0.1f)
        {
            // Forward/Back
            bool movingForward = currentInput.y > 0.1f;
            bool movingBack = currentInput.y < -0.1f;
            DrawKeyIndicator(playerPosition + gizmosOffset + forward * 1.5f, movingForward, "W");
            DrawKeyIndicator(playerPosition + gizmosOffset + -forward * 1.5f, movingBack, "S");

            // Left/Right
            bool movingLeft = currentInput.x < -0.1f;
            bool movingRight = currentInput.x > 0.1f;
            DrawKeyIndicator(playerPosition + gizmosOffset + -right * 1.5f, movingLeft, "A");
            DrawKeyIndicator(playerPosition + gizmosOffset + right * 1.5f, movingRight, "D");
        }
    }

    private void DrawKeyIndicator(Vector3 position, bool isPressed, string keyLabel)
    {
        Gizmos.color = isPressed ? inputColor : noInputColor;

        // Draw small cube for key
        Gizmos.DrawWireCube(position, Vector3.one * 0.3f);
        if (isPressed)
        {
            Gizmos.DrawCube(position, Vector3.one * 0.2f);
        }

        // Draw label (only visible in Scene view)
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(position + Vector3.up * 0.3f, keyLabel);
        #endif
    }

    private void DrawNavMeshTargetGizmo(Vector3 playerPosition, Vector3 gizmosOffset)
    {
        if (!showNavMeshTarget || playerMovement == null || !playerMovement.IsUsingNavMeshValidation()) return;

        Vector3 lastValidPosition = playerMovement.GetLastValidPosition();

        // Get feet position for NavMesh gizmos
        Vector3 feetPosition = GetFeetPosition();

        // Adjust last valid position to feet level
        Vector3 validPositionAtFeet = new Vector3(lastValidPosition.x, feetPosition.y, lastValidPosition.z);

        // Draw connection line from player feet to last valid position
        Gizmos.color = navMeshTargetColor;
        Gizmos.DrawLine(feetPosition, validPositionAtFeet);

        // Draw last valid position indicator at feet level
        Gizmos.color = navMeshTargetColor;
        Gizmos.DrawWireCube(validPositionAtFeet, Vector3.one * 0.4f);

        // Draw filled cube if there's active input
        if (GetCurrentInput().magnitude > 0.1f)
        {
            Gizmos.color = Color.Lerp(navMeshTargetColor, Color.white, 0.5f);
            Gizmos.DrawCube(validPositionAtFeet, Vector3.one * 0.2f);
        }

        // Draw label
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(validPositionAtFeet + Vector3.up * 0.7f, "Last Valid Position");
        #endif
    }

    private Vector3 GetFeetPosition()
    {
        // Use PlayerMovement's GetFeetPosition if available
        if (playerMovement != null)
        {
            // Access the feet position calculation from PlayerMovement
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                return new Vector3(transform.position.x, col.bounds.min.y, transform.position.z);
            }

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                return new Vector3(transform.position.x, renderer.bounds.min.y, transform.position.z);
            }

            return transform.position + Vector3.down * 1f;
        }

        return transform.position;
    }

    private void DrawWireCircle(Vector3 center, float radius, Vector3 normal)
    {
        Vector3 forward = Vector3.Slerp(normal, -normal, 0.000001f);
        Vector3 right = Vector3.Cross(normal, forward).normalized * radius;
        forward = Vector3.Cross(right, normal).normalized * radius;

        Matrix4x4 matrix = new Matrix4x4();
        matrix[0] = right.x;
        matrix[1] = right.y;
        matrix[2] = right.z;
        matrix[4] = forward.x;
        matrix[5] = forward.y;
        matrix[6] = forward.z;
        matrix[8] = normal.x;
        matrix[9] = normal.y;
        matrix[10] = normal.z;
        matrix[12] = center.x;
        matrix[13] = center.y;
        matrix[14] = center.z;
        matrix[15] = 1;

        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i <= 360; i += 10)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * i);
            thisPoint.z = Mathf.Cos(Mathf.Deg2Rad * i);
            thisPoint = matrix.MultiplyPoint3x4(thisPoint);

            if (i > 0)
                Gizmos.DrawLine(lastPoint, thisPoint);

            lastPoint = thisPoint;
        }
    }
}