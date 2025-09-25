using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Input Reference")]
    [SerializeField] private InputVisualizerGizmos inputVisualizer;

    [Header("Mouse Aiming Reference")]
    [SerializeField] private MouseGroundAiming mouseAiming;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private Vector2 mouseRelativeMovement;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (mouseAiming == null)
            mouseAiming = FindObjectOfType<MouseGroundAiming>();

        if (inputVisualizer == null)
            inputVisualizer = FindObjectOfType<InputVisualizerGizmos>();
    }

    private void Update()
    {
        CalculateMouseRelativeMovement();
        UpdateAnimatorParameters();
    }


    private void CalculateMouseRelativeMovement()
    {
        if (inputVisualizer == null || mouseAiming == null || mouseAiming.aimTarget == null)
        {
            mouseRelativeMovement = Vector2.zero;
            return;
        }

        // Get camera-relative input from InputVisualizerGizmos
        Vector3 cameraRelativeWorldInput = inputVisualizer.cameraRelativeInput;

        // If no input, return zero
        if (cameraRelativeWorldInput.magnitude < 0.1f)
        {
            mouseRelativeMovement = Vector2.zero;
            return;
        }

        // Get direction from player to mouse target (aim target)
        Vector3 playerPosition = transform.position;
        Vector3 mouseTargetPosition = mouseAiming.aimTarget.position;

        Vector3 toMouseDirection = (mouseTargetPosition - playerPosition);
        toMouseDirection.y = 0; // Project to horizontal plane
        toMouseDirection.Normalize();

        // If no mouse direction, fall back to camera-relative input
        if (toMouseDirection.magnitude < 0.1f)
        {
            mouseRelativeMovement = new Vector2(cameraRelativeWorldInput.x, cameraRelativeWorldInput.z);
            return;
        }

        // Calculate right vector perpendicular to mouse direction
        Vector3 mouseRight = new Vector3(toMouseDirection.z, 0, -toMouseDirection.x);

        // Project camera-relative world input onto mouse direction vectors
        float forwardComponent = Vector3.Dot(cameraRelativeWorldInput, toMouseDirection);
        float rightComponent = Vector3.Dot(cameraRelativeWorldInput, mouseRight);

        mouseRelativeMovement = new Vector2(rightComponent, forwardComponent);

        if (showDebugInfo)
        {
            Debug.Log($"Camera World Input: {cameraRelativeWorldInput} | Mouse Dir: {toMouseDirection} | Mouse Relative: {mouseRelativeMovement}");
        }
    }

    private void UpdateAnimatorParameters()
    {
        if (animator == null) return;

        // Update animator parameters with mouse-relative movement (rounded to integers)
        animator.SetFloat("moveX", Mathf.Round(mouseRelativeMovement.x));
        animator.SetFloat("moveY", Mathf.Round(mouseRelativeMovement.y));

        // Optional: Add movement magnitude parameter
        float movementMagnitude = mouseRelativeMovement.magnitude;
        animator.SetFloat("movementSpeed", movementMagnitude);

        // Optional: Add boolean for any movement
        bool isMoving = movementMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);
    }

    // Public getters for debugging or other scripts
    public Vector2 GetCameraRelativeInput() => inputVisualizer?.cameraRelativeInput ?? Vector2.zero;
    public Vector2 GetMouseRelativeMovement() => mouseRelativeMovement;
    public Vector3 GetMouseDirection()
    {
        if (mouseAiming == null || mouseAiming.aimTarget == null) return Vector3.forward;

        Vector3 toMouse = (mouseAiming.aimTarget.position - transform.position);
        toMouse.y = 0;
        return toMouse.normalized;
    }
}